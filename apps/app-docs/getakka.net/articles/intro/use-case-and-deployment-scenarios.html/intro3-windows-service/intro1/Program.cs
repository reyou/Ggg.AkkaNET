using System;
using Akka.Actor;
using Topshelf;
using Topshelf.ServiceConfigurators;

namespace intro1
{
    /// <summary>
    /// https://getakka.net/articles/intro/use-case-and-deployment-scenarios.html#windows-service
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(configureCallback =>
           {
               configureCallback.Service<MyActorService>(Callback);
               configureCallback.RunAsLocalSystem();
               configureCallback.UseAssemblyInfoForServiceInfo();
           });

        }

        private static void Callback(ServiceConfigurator<MyActorService> actorService)
        {
            actorService.ConstructUsing(n => new MyActorService());
            actorService.WhenStarted(service => service.Start());
            actorService.WhenStopped(service => service.Stop());
            //continue and restart directives are also available
        }
    }

    internal class MyActorService
    {
        private ActorSystem mySystem;

        public void Start()
        {
            //this is where you setup your actor system and other things
            mySystem = ActorSystem.Create("MySystem");
        }

        public async void Stop()
        {
            //this is where you stop your actor system
            await mySystem.Terminate();
        }
    }
}
