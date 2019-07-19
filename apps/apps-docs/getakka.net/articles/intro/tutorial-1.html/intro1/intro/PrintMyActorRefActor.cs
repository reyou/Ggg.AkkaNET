using System;
using Akka.Actor;

namespace intro
{
    public class PrintMyActorRefActor : UntypedActor
    {
        /// <summary>
        /// To be implemented by concrete UntypedActor, this defines the behavior of the UntypedActor
        /// </summary>
        /// <param name="message"></param>
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case "printit":
                    // Creates a new child actor of this context with the given name
                    IActorRef secondRef = Context.ActorOf(Props.Empty, "second-actor");
                    Console.WriteLine($"Second: {secondRef}");
                    break;
            }
        }
    }
}