using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace intro1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ActorSystem system = ActorSystem.Create("MySystem"); //automatically loads App/Web.config
                Console.WriteLine("ActorSystem automatically loads App/Web.config");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.ReadLine();
        }
    }
}
