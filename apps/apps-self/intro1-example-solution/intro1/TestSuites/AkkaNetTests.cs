using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Pattern;
using intro1.Actions;
using intro1.ActionTypes;
using intro1.Actors;
using intro1.Entities;

namespace intro1.TestSuites
{
    public class AkkaNetTests : ITestSuite
    {

        public void ActorSystemLog(ApplicationEnvironment applicationEnvironment)
        {
            ActorSystem actorSystem = ActorSystem.Create("app");
            actorSystem.Log.Debug("Debug Log");
            actorSystem.Log.Info("Info Log {0}", DateTime.Now);
            actorSystem.Log.Warning("Warning Log {0}", DateTime.Now);
            actorSystem.Log.Error(new Exception("Sample exception"), "Error Log {0}", DateTime.Now);
        }

        public void CreateActorExample(ApplicationEnvironment applicationEnvironment)
        {
            ActorSystem actorSystem = ActorSystem.Create("app");
            IActorRef userActor = actorSystem.ActorOf<UserActor>();
            IActorRef registerActor = actorSystem.ActorOf<RegisterActor>();
            IActorRef emailActor = actorSystem.ActorOf<EmailActor>();
            UserAction userAction = new UserAction(UserActionTypes.GenerateRandom);
            Task<TestUser> testUser = userActor.Ask<TestUser>(userAction);
            testUser.Wait();
            TestUser testUserResult = testUser.Result;
            Task<object> registerTask = registerActor.Ask(testUserResult);
            registerTask.Wait();
            emailActor.Tell(testUserResult);
        }

        public void TellException(ApplicationEnvironment applicationEnvironment)
        {
            ActorSystem actorSystem = ActorSystem.Create("app");
            IActorRef actorRef = actorSystem.ActorOf<EchoActor>();
            actorRef.Tell("EchoMessage1");
            actorRef.Tell(new Exception("File not found exception"));
            actorRef.Tell("EchoMessage2");
        }


        public void AskException(ApplicationEnvironment applicationEnvironment)
        {
            ActorSystem actorSystem = ActorSystem.Create("app");
            IActorRef userActor = actorSystem.ActorOf<UserActor>();
            IActorRef registerActor = actorSystem.ActorOf<RegisterActor>();
            UserAction userAction = new UserAction(UserActionTypes.GenerateRandom);
            userAction.ExceptionToThrow = new Exception("Cannot make SQL connection");
            Task<TestUser> testUser = userActor.Ask<TestUser>(userAction);
            testUser.Wait();
            Task<object> registerTask = registerActor.Ask(userAction);
            try
            {
                registerTask.Wait();
            }
            catch (Exception exception)
            {
                TestUtilities.ConsoleWriteJson(new
                {
                    exception
                });
            }
            userAction.ExceptionToThrow = null;
            registerTask = registerActor.Ask(userAction);
            registerTask.Wait();
        }
    }
}
