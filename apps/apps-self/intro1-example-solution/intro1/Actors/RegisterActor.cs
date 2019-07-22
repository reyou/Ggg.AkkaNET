using System;
using Akka.Actor;
using intro1.Actions;
using intro1.Entities;

namespace intro1.Actors
{
    public class RegisterActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            TestUtilities.ConsoleWriteJson(new
            {
                Message = $"{GetType().Name} received message",
                message
            });
            switch (message)
            {
                case UserAction userAction:
                    HandleUserAction(userAction, Sender);
                    break;
                case TestUser user:
                    HandleUser(user, Sender);
                    break;
            }
        }

        private void HandleUserAction(UserAction userAction, IActorRef sender)
        {
            if (userAction.ExceptionToThrow != null)
            {
                throw userAction.ExceptionToThrow;
            }
            TestUtilities.ConsoleWriteJson(new
            {
                Message = $"{GetType().Name}.HandleUser user registered into the system successfully",
                userAction
            });
            sender.Tell(true);
        }

        private void HandleUser(TestUser user, IActorRef sender)
        {
            TestUtilities.ConsoleWriteJson(new
            {
                Message = $"{GetType().Name}.HandleUser user registered into the system successfully",
                user
            });
            sender.Tell(true);
        }
    }
}