using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
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
    /// <summary>
    /// https://getakka.net/articles/concepts/supervision.html
    /// </summary>
    public class AkkaNetSupervisorTests : ITestSuite
    {
        public void BackoffSupervisorOnStop(ApplicationEnvironment applicationEnvironment)
        {
            ActorSystem actorSystem = ActorSystem.Create("app");
            // This class represents a configuration object used in creating an
            // ActorBase actor
            Props childProps = Props.Create<EchoActor>();
            //
            TimeSpan minBackoff = TimeSpan.FromSeconds(3);
            TimeSpan maxBackoff = TimeSpan.FromSeconds(30);
            double randomFactor = 0.2;
            int maxNrOfRetries = 2;
            // Builds back-off options for creating a back-off supervisor.
            BackoffOptions backoffOptions = Backoff.OnStop(childProps, "myEcho", minBackoff, maxBackoff, randomFactor, maxNrOfRetries);
            Props supervisor = BackoffSupervisor.Props(backoffOptions);
            IActorRef supervisorActor = actorSystem.ActorOf(supervisor, "echoSupervisor");
            supervisorActor.Tell("EchoMessage1");
            supervisorActor.Tell(new Exception("File not found exception"));
            supervisorActor.Tell("EchoMessage2");
        }

        public void BackoffSupervisorOnFailure(ApplicationEnvironment applicationEnvironment)
        {
            ActorSystem actorSystem = ActorSystem.Create("app");
            // This class represents a configuration object used in creating an
            // ActorBase actor
            Props childProps = Props.Create<EchoActor>();
            //
            TimeSpan minBackoff = TimeSpan.FromSeconds(3);

            TimeSpan maxBackoff = TimeSpan.FromSeconds(30);
            double randomFactor = 0.2;
            int maxNrOfRetries = 2;
            // Builds back-off options for creating a back-off supervisor.
            BackoffOptions backoffOptions = Backoff.OnFailure(childProps, "myEcho", minBackoff, maxBackoff, randomFactor, maxNrOfRetries);
            Props supervisor = BackoffSupervisor.Props(backoffOptions);
            IActorRef supervisorActor = actorSystem.ActorOf(supervisor, "echoSupervisor");
            supervisorActor.Tell("EchoMessage1");
            supervisorActor.Tell(new Exception("File not found exception"));
            TestUtilities.ThreadSleepSeconds(5);
            supervisorActor.Tell("EchoMessage2");
            TestUtilities.ThreadSleepSeconds(5);
            supervisorActor.Tell("EchoMessage3");
            TestUtilities.MethodEnds();
        }

        public void BackoffSupervisorOnFailureDeadLetter(ApplicationEnvironment applicationEnvironment)
        {
            ActorSystem actorSystem = ActorSystem.Create("app");
            // This class represents a configuration object used in creating an
            // ActorBase actor
            Props childProps = Props.Create<EchoActor>();
            //
            TimeSpan minBackoff = TimeSpan.FromSeconds(3);
            string childName = "myEcho";
            TimeSpan maxBackoff = TimeSpan.FromSeconds(30);
            double randomFactor = 0.2;
            int maxNrOfRetries = 2;
            // Builds back-off options for creating a back-off supervisor.
            BackoffOptions backoffOptions = Backoff.OnFailure(childProps, childName, minBackoff, maxBackoff, randomFactor, maxNrOfRetries);
            Props supervisor = BackoffSupervisor.Props(backoffOptions);
            IActorRef supervisorActor = actorSystem.ActorOf(supervisor, "echoSupervisor");
            supervisorActor.Tell("EchoMessage1");
            supervisorActor.Tell(new Exception("File not found exception"));
            supervisorActor.Tell("EchoMessage2");
            supervisorActor.Tell("EchoMessage3");
        }

        public void BackoffSupervisorWithAutoResetWithSupervisorStrategy(ApplicationEnvironment applicationEnvironment)
        {
            ActorSystem actorSystem = ActorSystem.Create("app");
            // This class represents a configuration object used in creating an
            // ActorBase actor
            Props childProps = Props.Create<EchoActor>();
            //
            TimeSpan minBackoff = TimeSpan.FromSeconds(3);
            string childName = "myEcho";
            TimeSpan maxBackoff = TimeSpan.FromSeconds(30);
            double randomFactor = 0.2;
            int maxNrOfRetries = 2;
            // Builds back-off options for creating a back-off supervisor.
            OneForOneStrategy supervisorStrategy = new OneForOneStrategy(exception =>
            {
                TestUtilities.ConsoleWriteJson(new
                {
                    Message = $"{GetType().Name} OneForOneStrategy",
                    ExceptionMessage = exception.Message
                });
                TestUtilities.ThreadSleepSeconds(10);
                if (exception is FileNotFoundException)
                {
                    return Directive.Restart;
                }

                return Directive.Escalate;
            });
            BackoffOptions backoffOptions = Backoff.OnStop(childProps, childName, minBackoff, maxBackoff, randomFactor, maxNrOfRetries).WithAutoReset(TimeSpan.FromSeconds(10)).WithSupervisorStrategy(supervisorStrategy);
            Props supervisor = BackoffSupervisor.Props(backoffOptions);
            IActorRef supervisorActor = actorSystem.ActorOf(supervisor, "echoSupervisor");
            supervisorActor.Tell("EchoMessage1");
            supervisorActor.Tell(new FileNotFoundException("File not found exception"));
            supervisorActor.Tell("EchoMessage2");
            supervisorActor.Tell("EchoMessage3");
            TestUtilities.MethodEnds();
        }
    }
}
