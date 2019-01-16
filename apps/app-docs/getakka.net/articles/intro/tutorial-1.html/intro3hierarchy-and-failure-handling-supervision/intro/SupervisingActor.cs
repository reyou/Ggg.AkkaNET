using System;
using System.Threading;
using Akka.Actor;

namespace intro
{
    public class SupervisingActor : UntypedActor
    {
        private IActorRef child = Context.ActorOf(Props.Create<SupervisedActor>(), "supervised-actor");

        protected override void OnReceive(object message)
        {
            Console.WriteLine("SupervisingActor received. thread id: {0}", Thread.CurrentThread.ManagedThreadId);
            switch (message)
            {
                case "failChild":
                    child.Tell("fail");
                    break;
            }
        }
    }
}