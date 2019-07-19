using System;
using Akka;
using Akka.Actor;
using Akka.Configuration;
using Akka.Configuration.Hocon;

namespace intro1
{
    class Program
    {
        static void Main(string[] args)
        {


            using (ActorSystem system = ActorSystem.Create("my-actor-server"))
            {
                // start two services
                IActorRef service1 = system.ActorOf<Service1>("service1");
                IActorRef service2 = system.ActorOf<Service2>("service2");
            }
            Console.WriteLine("App is finished.");
            Console.ReadLine();
        }
    }
}
