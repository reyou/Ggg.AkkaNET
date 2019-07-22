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
    }
}