using System;
using System.Threading;
using Akka.Actor;

namespace intro
{
    public class SupervisedActor : UntypedActor
    {
        //     Is called when an Actor is started.
        //     Actors are automatically started asynchronously when created.
        //     Empty default implementation.
        protected override void PreStart() => Console.WriteLine("supervised actor started");
        //     Is called asynchronously after 'actor.stop()' is invoked.
        //     Empty default implementation.
        protected override void PostStop() => Console.WriteLine("supervised actor stopped");

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case "fail":
                    Console.WriteLine("supervised actor fails now. thread id: {0}", Thread.CurrentThread.ManagedThreadId);
                    throw new Exception("I failed!");
            }
        }
    }
}