using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using intro1.Actors;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace intro1
{
    /// <summary>
    /// https://getakka.net/articles/intro/use-case-and-deployment-scenarios.html#aspnet
    /// </summary>
    public class Program
    {
        /*As you can see the main point here is keeping a static reference to your ActorSystem .
         This ensures it won't be accidentally garbage collected and gets disposed and 
         created with the start and stop events of your web application.*/
        protected static ActorSystem ActorSystem;
        //here you would store your toplevel actor-refs
        public static IActorRef MyActor;

        public static void Main(string[] args)
        {
            //your mvc config. Does not really matter if you initialise
            //your actor system before or after

            ActorSystem = ActorSystem.Create("app");
            //here you would register your toplevel actors
            MyActor = ActorSystem.ActorOf<MyActor>();
            CreateWebHostBuilder(args).Build().Run();

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
