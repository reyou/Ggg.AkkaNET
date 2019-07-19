using System;
using System.Threading;
using Akka.Actor;
using Newtonsoft.Json;

namespace intro1
{
    internal class Service2 : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            Console.WriteLine(JsonConvert.SerializeObject(new
            {
                Method = $"{GetType().Name}.OnReceive",
                Thread.CurrentThread.ManagedThreadId,
                message
            }, Formatting.Indented));
        }
    }
}