using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Akka.Actor;
using Akka.Event;
using intro.Models;

namespace intro.UntypedActors
{
    public class DeviceManager : UntypedActor
    {
        private readonly Dictionary<string, IActorRef> _groupIdToActor = new Dictionary<string, IActorRef>();
        private readonly Dictionary<IActorRef, string> _actorToGroupId = new Dictionary<IActorRef, string>();

        protected override void PreStart() => Log.Info("DeviceManager started");
        protected override void PostStop() => Log.Info("DeviceManager stopped");

        protected ILoggingAdapter Log { get; } = Context.GetLogger();

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case RequestTrackDevice trackMsg:
                    if (_groupIdToActor.TryGetValue(trackMsg.GroupId, out IActorRef actorRef))
                    {
                        actorRef.Forward(trackMsg);
                    }
                    else
                    {
                        Log.Info($"Creating device group actor for {trackMsg.GroupId}");
                        IActorRef groupActor = Context.ActorOf(DeviceGroup.Props(trackMsg.GroupId), $"group-{trackMsg.GroupId}");
                        Context.Watch(groupActor);
                        groupActor.Forward(trackMsg);
                        _groupIdToActor.Add(trackMsg.GroupId, groupActor);
                        _actorToGroupId.Add(groupActor, trackMsg.GroupId);
                    }
                    break;
                case RequestDeviceList requestDeviceList:
                    foreach (KeyValuePair<IActorRef, string> keyValuePair in _actorToGroupId)
                    {
                        Log.Info("RequestDeviceList: {0}", keyValuePair.Value);
                        IActorRef key = keyValuePair.Key;
                        key.Forward(requestDeviceList);
                    }
                    break;

                case Terminated t:
                    string groupId = _actorToGroupId[t.ActorRef];
                    Log.Info($"Device group actor for {groupId} has been terminated");
                    _actorToGroupId.Remove(t.ActorRef);
                    _groupIdToActor.Remove(groupId);
                    break;
            }
        }

        public static Props Props(string groupId) => Akka.Actor.Props.Create<DeviceManager>();
    }
}
