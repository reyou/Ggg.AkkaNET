using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Akka.Actor;
using intro1.Actions;
using intro1.ActionTypes;
using intro1.Actors;
using intro1.Entities;

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

        public void CreateActorExample(ApplicationEnvironment applicationEnvironment)
        {
            ActorSystem = ActorSystem.Create("app");
            //here you would register your toplevel actors
            IActorRef userActor = ActorSystem.ActorOf<UserActor>();
            IActorRef registerActor = ActorSystem.ActorOf<RegisterActor>();
            IActorRef emailActor = ActorSystem.ActorOf<EmailActor>();
            UserAction userAction = new UserAction(UserActionTypes.GenerateRandom);
            Task<TestUser> testUser = userActor.Ask<TestUser>(userAction);
            testUser.Wait();
            TestUser testUserResult = testUser.Result;
            Task<object> registerTask = registerActor.Ask(testUserResult);
            registerTask.Wait();
            emailActor.Tell(testUserResult);
        }
    }
}
