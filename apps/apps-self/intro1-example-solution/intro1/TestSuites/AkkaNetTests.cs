using System;
using System.Collections.Generic;
using System.Diagnostics;
using Akka.Actor;

namespace intro1.TestSuites
{
    public class AkkaNetTests : ITestSuite
    {
        protected static ActorSystem ActorSystem;
        public void ActorSystemLog(ApplicationEnvironment applicationEnvironment)
        {
            ActorSystem = ActorSystem.Create("app");
            ActorSystem.Log.Debug("Debug Log");
            ActorSystem.Log.Info("Info Log {0}", DateTime.Now);
            ActorSystem.Log.Warning("Warning Log {0}", DateTime.Now);
            ActorSystem.Log.Error(new Exception("Sample exception"), "Error Log {0}", DateTime.Now);
        }

        public void qqq(ApplicationEnvironment applicationEnvironment)
        {
            ActorSystem = ActorSystem.Create("app");
            //here you would register your toplevel actors
            IActorRef userActor = ActorSystem.ActorOf<UserActor>();
        }
    }

    public class UserActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            throw new NotImplementedException();
        }
    }
}
