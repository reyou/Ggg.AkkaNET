using System;
using Newtonsoft.Json;


namespace intro1
{
    public class TestUtilities
    {
        public static void ConsoleWriteJson(object item)
        {
            Console.WriteLine();
            Console.WriteLine(JsonConvert.SerializeObject(item, Formatting.Indented));
            Console.WriteLine();
        }
    }
}