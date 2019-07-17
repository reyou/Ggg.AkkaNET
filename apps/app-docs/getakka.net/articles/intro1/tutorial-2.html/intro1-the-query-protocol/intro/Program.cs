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
    /// https://getakka.net/articles/intro/tutorial-2.html#message-ordering-delivery-guarantees
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            TestKit testKit = new TestKit();
            ActorSystem actorSystem = testKit.Sys;
            using (actorSystem)
            {
                // TestKit-based probe which allows sending, reception and reply.
                TestProbe probe = testKit.CreateTestProbe("test-probe");
                IActorRef deviceActor = actorSystem.ActorOf(Device.Props("group", "device"));
                IActorRef probeRef = probe.Ref;
                deviceActor.Tell(new MainDevice.ReadTemperature(requestId: 42), probeRef);
                RespondTemperature response = probe.ExpectMsg<RespondTemperature>();
                response.RequestId.Should().Be(42);
                response.Value.Should().BeNull();
                Console.WriteLine("UserMessage: App is finished.");
                // Exit the system after ENTER is pressed
                Console.ReadLine();
            }

        }
    }


}
