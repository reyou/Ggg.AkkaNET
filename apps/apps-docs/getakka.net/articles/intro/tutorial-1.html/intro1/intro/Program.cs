using System;
using Akka.Actor;
using Akka.TestKit.NUnit;

namespace intro
{
    class Program
    {
        static void Main(string[] args)
        {
            TestKit testKit = new TestKit();
            //     An actor system is a hierarchical group of actors which share common
            //     configuration, e.g. dispatchers, deployments, remote capabilities and
            //     addresses. It is also the entry point for creating or looking up actors.
            ActorSystem actorSystem = testKit.Sys;
            // This class represents a configuration object used in creating an actor
            Props props = Props.Create<PrintMyActorRefActor>();
            IActorRef firstRef = actorSystem.ActorOf(props, "first-actor");
            Console.WriteLine($"First: {firstRef}");
            firstRef.Tell("printit", ActorRefs.NoSender);
            Console.WriteLine("app is finished.");
            Console.ReadLine();
        }
    }
}
