using System;
using Akka.Actor;

namespace intro
{
    public class StartStopActor2 : UntypedActor
    {
        protected override void PreStart() => Console.WriteLine("second started");
        protected override void PostStop() => Console.WriteLine("second stopped");

        protected override void OnReceive(object message)
        {
            Console.WriteLine("second message: {0}", message);
        }
    }
}