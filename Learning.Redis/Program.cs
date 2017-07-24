using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange.Redis;
using System.Threading;

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
            RedisChannel channel = new RedisChannel("pub/sub", RedisChannel.PatternMode.Auto);
            ISubscriber sub = _redisClient.GetSubscriber();

            sub.Subscribe(channel, (c, v) => {
                if (! v.IsNull)
                {
                    Console.WriteLine(v.ToString());
                }
            });

            for (int i = 0; i < 100; i++)
            {
                var input = i.ToString().PadLeft(3, '0') + "." + Guid.NewGuid().ToString();
                sub.Publish(channel, input);
            }
            
            Console.WriteLine("\nPress Any Key To Exit...");
            Console.ReadLine();
        }
    }
}
