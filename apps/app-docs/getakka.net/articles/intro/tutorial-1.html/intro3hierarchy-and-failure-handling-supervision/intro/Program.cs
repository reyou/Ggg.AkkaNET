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
            IActorRef supervisingActor = actorSystem.ActorOf(Props.Create<SupervisingActor>(), "supervising-actor");
            supervisingActor.Tell("failChild");
            Console.WriteLine("\nI will try again!...\n");
            supervisingActor.Tell("failChild");
            Console.WriteLine("app is finished.");
            Console.ReadLine();
        }
    }
}
