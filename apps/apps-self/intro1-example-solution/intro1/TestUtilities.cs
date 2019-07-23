using System;
using System.Threading;
using Newtonsoft.Json;


namespace intro1
{
    public class TestUtilities
    {
        public static void ConsoleWriteJson(object item)
        {
            Console.WriteLine();
            Console.WriteLine(JsonConvert.SerializeObject(new
            {
                Thread.CurrentThread.ManagedThreadId,
                ThreadName = Thread.CurrentThread.Name,
                DateTime = DateTime.Now.ToString("F"),
                item
            }, Formatting.Indented));
            Console.WriteLine();
        }

        public static void ThreadSleepSeconds(int seconds)
        {
            Console.WriteLine($"ManagedThreadId {Thread.CurrentThread.ManagedThreadId} ({Thread.CurrentThread.Name}) sleeps {seconds} sec...");
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
        }

        public static void MethodEnds()
        {
            Console.WriteLine();
            Console.WriteLine("=========== Method Ends =================");
            Console.WriteLine();
        }
    }
}