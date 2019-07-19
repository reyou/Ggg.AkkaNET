using System;
using Akka.Actor;
using Akka.Event;

namespace intro
{
    public class IotSupervisor : UntypedActor
    {
        /// <summary>
        /// This interface describes the methods used to log events within the system
        /// protected static IUntypedActorContext Context
        /// </summary>
        public ILoggingAdapter Log { get; } = Context.GetLogger();

        protected override void PreStart() => Log.Info("IoT Application started");
        protected override void PostStop() => Log.Info("IoT Application stopped");

        public IotSupervisor()
        {
            Console.WriteLine("logger type: {0}", Log.GetType().FullName);
        }
        // No need to handle any messages
        protected override void OnReceive(object message)
        {
            Log.Info("Message received: {0}", message);
        }

        public static Props Props()
        {
            Context.GetLogger().Info("Props creating an IotSupervisor instance");
            return Akka.Actor.Props.Create<IotSupervisor>();
        }
    }
}