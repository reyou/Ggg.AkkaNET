using System;
using Akka.Actor;
using Akka.Configuration;
using Akka.TestKit;
using Akka.TestKit.Xunit2;
using FluentAssertions;
using intro.Models;
using intro.UntypedActors;

namespace intro
{
    /// <summary>
    /// https://getakka.net/articles/intro/tutorial-4.html
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {


        }

        static void Main2(string[] args)
        {
            TestKit testKit = new TestKit();
            ActorSystem actorSystem = testKit.Sys;
            using (actorSystem)
            {
                TestProbe probe = testKit.CreateTestProbe("test-probe");

                /*In the first, we just test that we get back the list of
                 proper IDs once we have added a few devices.*/
                Action test1 = new Action(() =>
                {
                    IActorRef deviceManagerActor = actorSystem.ActorOf(DeviceManager.Props("device-manager"));

                    deviceManagerActor.Tell(new RequestTrackDevice("group", "device1"), probe.Ref);
                    probe.ExpectMsg<DeviceRegistered>();

                    deviceManagerActor.Tell(new RequestTrackDevice("group", "device2"), probe.Ref);
                    probe.ExpectMsg<DeviceRegistered>();

                    deviceManagerActor.Tell(new RequestDeviceList(requestId: 0), probe.Ref);
                    probe.ExpectMsg<ReplyDeviceList>(s => s.RequestId == 0
                                                          && s.Ids.Contains("device1")
                                                          && s.Ids.Contains("device2"));
                    Console.WriteLine("Test 1 passed.");
                    Console.WriteLine("");
                });
                test1.Invoke();

                /*The second test case makes sure that the device ID
                 is properly removed after the device actor has been stopped.*/
                Action test2 = new Action(() =>
                {
                    IActorRef deviceManagerActor = actorSystem.ActorOf(DeviceManager.Props("device-manager"));

                    deviceManagerActor.Tell(new RequestTrackDevice("group", "device1"), probe.Ref);
                    probe.ExpectMsg<DeviceRegistered>();
                    IActorRef toShutDown = probe.LastSender;

                    deviceManagerActor.Tell(new RequestTrackDevice("group", "device2"), probe.Ref);
                    probe.ExpectMsg<DeviceRegistered>();

                    deviceManagerActor.Tell(new RequestDeviceList(requestId: 0), probe.Ref);
                    probe.ExpectMsg<ReplyDeviceList>(s => s.RequestId == 0
                                                          && s.Ids.Contains("device1")
                                                          && s.Ids.Contains("device2"));

                    probe.Watch(toShutDown);
                    toShutDown.Tell(PoisonPill.Instance);
                    probe.ExpectTerminated(toShutDown);

                    // using awaitAssert to retry because it might take longer for the groupActor
                    // to see the Terminated, that order is undefined
                    probe.AwaitAssert(() =>
                    {
                        deviceManagerActor.Tell(new RequestDeviceList(requestId: 1), probe.Ref);
                        probe.ExpectMsg<ReplyDeviceList>(s => s.RequestId == 1 && s.Ids.Contains("device2"));
                    });

                    Console.WriteLine("Test 2 passed.");
                    Console.WriteLine("");
                });
                test2.Invoke();
                Console.WriteLine("UserMessage: App is finished.");
                // Exit the system after ENTER is pressed
                Console.ReadLine();
            }

        }
    }


}
