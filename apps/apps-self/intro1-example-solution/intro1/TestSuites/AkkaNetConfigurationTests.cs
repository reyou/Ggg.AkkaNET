using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Akka.Configuration;
using intro1.Actors;

namespace intro1.TestSuites
{
    public class AkkaNetConfigurationTests : ITestSuite
    {
        public void ConfigurationFactoryTest(ApplicationEnvironment environment)
        {
            Config config = ConfigurationFactory.ParseString(@"
akka.remote.dot-netty.tcp {
    transport-class = ""Akka.Remote.Transport.DotNetty.DotNettyTransport, Akka.Remote""
    transport-protocol = tcp
    port = 8091
    hostname = ""127.0.0.1""
}");

            ActorSystem actorSystem = ActorSystem.Create("MyActorSystem", config);
            IActorRef actor1 = actorSystem.ActorOf<EchoActor>("echoActor1");
            IActorRef actor2 = actorSystem.ActorOf<EchoActor>("echoActor2");
            actor1.Tell("Actor1 message");
            actor2.Tell("Actor2 message");
            TestUtilities.MethodEnds();
        }
    }
}
