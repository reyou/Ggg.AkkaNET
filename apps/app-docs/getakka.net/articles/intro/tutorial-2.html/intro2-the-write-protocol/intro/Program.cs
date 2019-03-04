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
                // TestKit-based probe which allows sending, reception and reply.
                TestProbe probe = testKit.CreateTestProbe("test-probe");
                IActorRef deviceActor = actorSystem.ActorOf(Device.Props("group", "device"));

                deviceActor.Tell(new RecordTemperature(requestId: 1, value: 24.0), probe.Ref);
                probe.ExpectMsg<TemperatureRecorded>(s => s.RequestId == 1);

                deviceActor.Tell(new ReadTemperature(requestId: 2), probe.Ref);
                RespondTemperature response1 = probe.ExpectMsg<RespondTemperature>();
                response1.RequestId.Should().Be(2);
                response1.Value.Should().Be(24.0);

                deviceActor.Tell(new RecordTemperature(requestId: 3, value: 55.0), probe.Ref);
                probe.ExpectMsg<TemperatureRecorded>(s => s.RequestId == 3);

                deviceActor.Tell(new ReadTemperature(requestId: 4), probe.Ref);
                RespondTemperature response2 = probe.ExpectMsg<RespondTemperature>();
                response2.RequestId.Should().Be(4);
                response2.Value.Should().Be(55.0);
                Console.WriteLine("UserMessage: App is finished.");
                // Exit the system after ENTER is pressed
                Console.ReadLine();
            }

        }
    }


}
