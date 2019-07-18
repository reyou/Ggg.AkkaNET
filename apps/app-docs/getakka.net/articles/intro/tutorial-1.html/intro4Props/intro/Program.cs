using System;
using Akka.Actor;
using Akka.TestKit.NUnit;

namespace intro
{
    class Program
    {
        static void Main(string[] args)
        {
            IotApp.Init();
            Console.WriteLine("app is finished.");
            // Exit the system after ENTER is pressed
            Console.ReadLine();
        }
    }
}
