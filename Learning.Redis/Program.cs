using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange.Redis;

namespace Learning.Redis
{
    class Program
    {
        protected readonly static ConnectionMultiplexer _redisClient = ConnectionMultiplexer.Connect("localhost");
        
        static void Main(string[] args)
        {
            var opts = new ConfigurationOptions();
            opts.ClientName = "Test-Clinet";
            opts.KeepAlive = 180;
            opts.Password = "123456";
            Console.WriteLine(opts.ToString());

            var db = _redisClient.GetDatabase();
            db.StringSet("Key", Guid.NewGuid().ToString());
            var value = db.StringGet("Key");
            Console.WriteLine(value);
            
            Console.WriteLine("\nPress Any Key To Exit...");
            Console.ReadLine();
        }
    }
}
