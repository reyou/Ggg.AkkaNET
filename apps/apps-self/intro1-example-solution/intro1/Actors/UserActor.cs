using System;
using Akka.Actor;
using intro1.Actions;
using intro1.ActionTypes;
using intro1.Entities;

namespace intro1.Actors
{
    public class UserActor : UntypedActor
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
            }
        }

        private void HandleUserAction(UserAction userAction, IActorRef sender)
        {
            switch (userAction.ActionType)
            {
                case UserActionTypes.GenerateRandom:
                    GenerateRandomUser(sender);
                    break;
                case UserActionTypes.Create:
                    break;
                case UserActionTypes.Delete:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void GenerateRandomUser(IActorRef sender)
        {
            string userName = Guid.NewGuid().ToString().Substring(0, 5);
            string password = Guid.NewGuid().ToString().Substring(0, 5);
            string email = Guid.NewGuid().ToString().Substring(0, 5) + "@" + "example.com";
            TestUser testUser = new TestUser(userName, password, email);
            sender.Tell(testUser);
        }
    }
}