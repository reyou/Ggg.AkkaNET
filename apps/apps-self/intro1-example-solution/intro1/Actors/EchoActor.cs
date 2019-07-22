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
            });
        }

        protected override void PostStop()
        {
            base.PostStop();
            TestUtilities.ConsoleWriteJson(new
            {
                Message = $"{GetType().Name} PostStop",
            });
        }

        protected override void OnReceive(object message)
        {
            TestUtilities.ConsoleWriteJson(new
            {
                Message = $"{GetType().Name} received message",
                message
            });
            switch (message)
            {
                case Exception exception:
                    throw exception;
            }
        }
    }
}