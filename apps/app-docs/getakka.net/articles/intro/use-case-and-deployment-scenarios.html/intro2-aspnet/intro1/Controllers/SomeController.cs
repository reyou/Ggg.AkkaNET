using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using intro1.ActorMessages;
using intro1.WorkItems;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace intro1.Controllers
{
    /// <summary>
    /// http://localhost:5000/Some
    /// </summary>
    public class SomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            Console.Write(JsonConvert.SerializeObject(new
            {
                Method = $"{GetType().Name}.Index",
                Thread.CurrentThread.ManagedThreadId,
            }, Formatting.Indented));

            //send a message based on your incoming arguments to one of the actors you created earlier
            //and await the result by sending the message to `Ask`
            SomeRequest someRequest = new SomeRequest(DateTime.Now, Guid.NewGuid().ToString());
            SomeMessage someMessage = new SomeMessage(someRequest.Date, someRequest.Guid);
            SomeResult result = await Program.MyActor.Ask<SomeResult>(someMessage);
            return View(result);
        }
    }
}