using System;
using Akka.Actor;

namespace intro
{
    public class IotApp
    {
        public static void Init()
        {
            //     An actor system is a hierarchical group of actors which share common
            //     configuration, e.g. dispatchers, deployments, remote capabilities and
            //     addresses. It is also the entry point for creating or looking up actors.
            //     There are several possibilities for creating actors (see <see cref="T:Akka.Actor.Props" />
            //     for details on `props`):
            using (ActorSystem system = ActorSystem.Create("iot-system"))
            {
                // Create top level supervisor
                IActorRef supervisor = system.ActorOf(Props.Create<IotSupervisor>(), "iot-supervisor");
                // Asynchronously tells a message to an IActorRef
                supervisor.Tell("telling");

            }
        }
    }
}