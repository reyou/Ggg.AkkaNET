using System;
using System.Threading;
using Akka.Actor;
using intro1.ActorMessages;
using intro1.WorkItems;
using Newtonsoft.Json;

namespace intro1.Actors
{
    public class MyActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            Console.Write(JsonConvert.SerializeObject(new
            {
                Method = $"{GetType().Name}.OnReceive",
                Thread.CurrentThread.ManagedThreadId,
                message
            }, Formatting.Indented));
            switch (message)
            {
                case SomeMessage someMessage:
                    Console.Write(JsonConvert.SerializeObject(new
                    {
                        Method = $"{GetType().Name}.OnReceive",
                        Message = $"Message Arrived",
                        Thread.CurrentThread.ManagedThreadId,
                        someMessage
                    }, Formatting.Indented));
                    SomeResult result = new SomeResult(DateTime.Now);
                    Sender.Tell(result);
                    break;
            }
        }
    }
}