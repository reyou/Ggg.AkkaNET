using System;
using Akka.Actor;
using Akka.Configuration;

namespace intro1
{
    class Program
    {
        static void Main(string[] args)
        {
            Config config = ConfigurationFactory.ParseString(@"
akka.remote.dot-netty.tcp {
    transport-class = ""Akka.Remote.Transport.DotNetty.DotNettyTransport, Akka.Remote""
    transport-protocol = tcp
    port = 8091
    hostname = ""127.0.0.1""
}");

            ActorSystem system = ActorSystem.Create("MyActorSystem", config);
            Console.WriteLine("ActorSystem has been created.");
            Console.ReadLine();
        }
    }
}
