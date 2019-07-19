using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Akka.Event;
using intro.Models;
using Newtonsoft.Json;

namespace intro.UntypedActors
{
    public class DeviceGroup : UntypedActor
    {
        private readonly Dictionary<string, IActorRef> _deviceIdToActor = new Dictionary<string, IActorRef>();
        private readonly Dictionary<IActorRef, string> _actorToDeviceId = new Dictionary<IActorRef, string>();
        private long nextCollectionId = 0L;
        public DeviceGroup(string groupId)
        {
            GroupId = groupId;
        }

        protected override void PreStart() => Log.Info($"Device group {GroupId} started");
        protected override void PostStop() => Log.Info($"Device group {GroupId} stopped");

        protected ILoggingAdapter Log { get; } = Context.GetLogger();
        protected string GroupId { get; }

        protected override void OnReceive(object message)
        {
            Console.WriteLine(JsonConvert.SerializeObject(new
            {
                Message = $"{GetType().Name}.OnReceive",
                message
            }, Formatting.Indented));
            switch (message)
            {
                case RequestAllTemperatures r:
                    Context.ActorOf(DeviceGroupQuery.Props(_actorToDeviceId, r.RequestId, Sender, TimeSpan.FromSeconds(3)));
                    break;
                case RequestTrackDevice trackMsg when trackMsg.GroupId.Equals(GroupId):
                    if (_deviceIdToActor.TryGetValue(trackMsg.DeviceId, out IActorRef actorRef))
                    {
                        actorRef.Forward(trackMsg);
                    }
                    else
                    {
                        Log.Info($"Creating device actor for {trackMsg.DeviceId}");
                        IActorRef deviceActor = Context.ActorOf(Device.Props(trackMsg.GroupId, trackMsg.DeviceId), $"device-{trackMsg.DeviceId}");
                        // Monitors the specified actor for termination.
                        // When the actor terminates the instance watching
                        // will receive a Terminated message.
                        Context.Watch(deviceActor);
                        _actorToDeviceId.Add(deviceActor, trackMsg.DeviceId);
                        _deviceIdToActor.Add(trackMsg.DeviceId, deviceActor);
                        // Forwards the message using the current Sender.
                        deviceActor.Forward(trackMsg);
                    }
                    break;
                case RequestDeviceList deviceList:
                    Sender.Tell(new ReplyDeviceList(deviceList.RequestId, new HashSet<string>(_deviceIdToActor.Keys)));
                    break;
                case RequestTrackDevice trackMsg:
                    Log.Warning($"Ignoring TrackDevice request for {trackMsg.GroupId}. This actor is responsible for {GroupId}.");
                    break;
                case Terminated t:
                    string deviceId = _actorToDeviceId[t.ActorRef];
                    Log.Info($"Device actor for {deviceId} has been terminated");
                    _actorToDeviceId.Remove(t.ActorRef);
                    _deviceIdToActor.Remove(deviceId);
                    break;
            }
        }

        public static Props Props(string groupId)
        {
            Console.WriteLine(JsonConvert.SerializeObject(new
            {
                Message = $"{typeof(DeviceGroup).Name}.Props",
                groupId
            }, Formatting.Indented));
            return Akka.Actor.Props.Create(() => new DeviceGroup(groupId));
        }
    }


}
