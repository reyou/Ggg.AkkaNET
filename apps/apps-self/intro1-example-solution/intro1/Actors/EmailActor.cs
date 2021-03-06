﻿using System;
using Akka.Actor;
using intro1.Entities;

namespace intro1.Actors
{
    public class EmailActor : UntypedActor
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
                case TestUser user:
                    HandleUser(user, Sender);
                    break;
            }
        }

        private void HandleUser(TestUser user, IActorRef sender)
        {
            TestUtilities.ConsoleWriteJson(new
            {
                Message = $"{GetType().Name}.HandleUser sent email to user successfully",
                user
            });
        }
    }
}