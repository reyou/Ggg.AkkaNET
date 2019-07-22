using System;
using System.Collections.Generic;
using System.Linq;

namespace intro1
{
    class Program
    {
        private static ApplicationEnvironment applicationEnvironment = ApplicationEnvironment.Local;
        // ReSharper disable once FunctionRecursiveOnAllPaths
        public static void Main(string[] args)
        {
            Console.WriteLine(@"Environment Settings:");
            Console.WriteLine($"Application Environment: {applicationEnvironment}");
            Console.WriteLine();
            Console.WriteLine(@"Choose an option below:");
            Console.WriteLine();
            // list options
            List<TestSuiteMethod> testRunOptions = TestRunManager.GetTestRunOptions(applicationEnvironment);
            foreach (TestSuiteMethod testRunOption in testRunOptions)
            {
                Console.WriteLine($"{testRunOption.Order}- {testRunOption.Title} ({testRunOption.Order})");
            }
            // pick an option
            string option = Console.ReadLine();
            int optionInt;
            int.TryParse(option, out optionInt);
            TestSuiteMethod actionToRun = testRunOptions.FirstOrDefault(q => q.Order.Equals(optionInt));
            if (actionToRun == null)
            {
                Console.WriteLine();
                Console.WriteLine(@"Invalid option: {0}", option);
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine();
                TestUtilities.ConsoleWriteJson(new
                {
                    Message = $"Executing: {actionToRun.Title}",
                });
                Console.WriteLine();
                try
                {
                    actionToRun.TaskToRun.Invoke();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            Console.WriteLine();
            Console.WriteLine();
            Main(null);
            Console.ReadLine();
        }
    }
}
