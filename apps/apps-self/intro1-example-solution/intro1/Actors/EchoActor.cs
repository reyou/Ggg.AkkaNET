using System;
using Akka.Actor;

namespace intro1.Actors
{
    public class EchoActor : UntypedActor
    {
        protected override void PreStart()
        {
            base.PreStart();
            TestUtilities.ConsoleWriteJson(new
            {
                Message = $"{GetType().Name} PreStart",
                Address = Self.ToString(),
            });
        }

        protected override void PostStop()
        {
            base.PostStop();
            TestUtilities.ConsoleWriteJson(new
            {
                Message = $"{GetType().Name} PostStop",
                Address = Self.ToString(),
            });
        }

        protected override void OnReceive(object message)
        {
            TestUtilities.ConsoleWriteJson(new
            {
                Message = $"{GetType().Name} received message",
                Address = Self.ToString(),
                message
            });
            switch (message)
            {
                case Exception exception:
                    throw exception;
                case string messageRef:
                    HandleMessage(messageRef, Sender);
                    break;

            }
        }

        private void HandleMessage(string message, IActorRef sender)
        {
            TestUtilities.ConsoleWriteJson(new
            {
                Message = $"{GetType().Name}.HandleMessage processed",
                Address = Self.ToString(),
            });
            sender.Tell(true);
        }
    }
}