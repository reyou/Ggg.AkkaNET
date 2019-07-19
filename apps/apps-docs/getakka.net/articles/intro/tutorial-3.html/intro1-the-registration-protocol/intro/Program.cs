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
    /// https://getakka.net/articles/intro/tutorial-2.html#the-write-protocol
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
                    // TestKit-based probe which allows sending, reception and reply.
                    IActorRef deviceActor = actorSystem.ActorOf(Device.Props("group", "device"));
                    IActorRef probeRef = probe.Ref;
                    deviceActor.Tell(new RequestTrackDevice("group", "device"), probeRef);
                    probe.ExpectMsg<DeviceRegistered>();
                    probe.LastSender.Should().Be(deviceActor);
                    Console.WriteLine("Test 1 passed.");
                });
                test1.Invoke();
                Action test2 = new Action(() =>
                {
                    // TestKit-based probe which allows sending, reception and reply.
                    var deviceActor = actorSystem.ActorOf(Device.Props("group", "device"));
                    deviceActor.Tell(new RequestTrackDevice("wrongGroup", "device"), probe.Ref);
                    probe.ExpectNoMsg(TimeSpan.FromMilliseconds(500));
                    deviceActor.Tell(new RequestTrackDevice("group", "Wrongdevice"), probe.Ref);
                    probe.ExpectNoMsg(TimeSpan.FromMilliseconds(500));
                    Console.WriteLine("Test 2 passed.");
                });
                test2.Invoke();
                Console.WriteLine("UserMessage: App is finished.");
                // Exit the system after ENTER is pressed
                Console.ReadLine();
            }

        }
    }


}
