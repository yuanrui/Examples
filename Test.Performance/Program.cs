using BenchmarkDotNet.Running;
using System;

namespace Test.Performance
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Pbkdf2Test>();

            Console.WriteLine(summary);
            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }
    }
}
