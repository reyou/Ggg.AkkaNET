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
    /// https://getakka.net/articles/intro/tutorial-3.html#device-group
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            TestKit testKit = new TestKit();
            ActorSystem actorSystem = testKit.Sys;
            using (actorSystem)
            {
                TestProbe probe = testKit.CreateTestProbe("test-probe");
                Action test1 = new Action(() =>
                {
                    IActorRef groupActor = actorSystem.ActorOf(DeviceGroup.Props("group"));

                    groupActor.Tell(new RequestTrackDevice("group", "device1"), probe.Ref);
                    probe.ExpectMsg<DeviceRegistered>();
                    IActorRef deviceActor1 = probe.LastSender;

                    groupActor.Tell(new RequestTrackDevice("group", "device2"), probe.Ref);
                    probe.ExpectMsg<DeviceRegistered>();
                    IActorRef deviceActor2 = probe.LastSender;
                    deviceActor1.Should().NotBe(deviceActor2);

                    // Check that the device actors are working
                    deviceActor1.Tell(new RecordTemperature(requestId: 0, value: 1.0), probe.Ref);
                    probe.ExpectMsg<TemperatureRecorded>(s => s.RequestId == 0);
                    deviceActor2.Tell(new RecordTemperature(requestId: 1, value: 2.0), probe.Ref);
                    probe.ExpectMsg<TemperatureRecorded>(s => s.RequestId == 1);
                    Console.WriteLine("Test 1 passed.");
                    Console.WriteLine("");
                });
                test1.Invoke();
                Action test2 = new Action(() =>
                {
                    IActorRef groupActor = actorSystem.ActorOf(DeviceGroup.Props("group"));
                    groupActor.Tell(new RequestTrackDevice("wrongGroup", "device1"), probe.Ref);
                    probe.ExpectNoMsg(TimeSpan.FromMilliseconds(500));
                    Console.WriteLine("Test 2 passed.");
                    Console.WriteLine("");
                });
                test2.Invoke();

                /*It might be, that a device actor already exists for the registration request.
                 In this case, we would like to use the existing actor instead of a new one. 
                 We have not tested this yet, so we need to fix this:*/
                Action test3 = new Action(() =>
                {
                    IActorRef groupActor = actorSystem.ActorOf(DeviceGroup.Props("group"));

                    groupActor.Tell(new RequestTrackDevice("group", "device1"), probe.Ref);
                    probe.ExpectMsg<DeviceRegistered>();
                    IActorRef deviceActor1 = probe.LastSender;

                    groupActor.Tell(new RequestTrackDevice("group", "device1"), probe.Ref);
                    probe.ExpectMsg<DeviceRegistered>();
                    IActorRef deviceActor2 = probe.LastSender;

                    deviceActor1.Should().Be(deviceActor2);
                    Console.WriteLine("Test 3 passed.");
                    Console.WriteLine("");
                });
                test3.Invoke();
                Console.WriteLine("UserMessage: App is finished.");
                // Exit the system after ENTER is pressed
                Console.ReadLine();
            }

        }
    }


}
