using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using StackExchange.Redis;

namespace Learning.Redis
{
    class Program
    {
        //protected readonly static ConnectionMultiplexer _redisClient = ConnectionMultiplexer.Connect("localhost");
        
        static void Main(string[] args)
        {
            RedisServerBootmgr.Start();
            
            //var opts = new ConfigurationOptions();
            //opts.ClientName = "Test-Clinet";
            //opts.KeepAlive = 180;
            //opts.Password = "123456";
            //Console.WriteLine(opts.ToString());
            //Process.GetProcessesByName("redis-server.exe");
            
            //var db = _redisClient.GetDatabase();
            //db.StringSet("Key", Guid.NewGuid().ToString());
            //var value = db.StringGet("Key");
            //Console.WriteLine(value);

            ConnectionMultiplexer _redisClient = ConnectionMultiplexer.Connect("localhost:6399");
            var inputCmd = Console.ReadLine();
            RedisChannel channel = new RedisChannel("pub/sub", RedisChannel.PatternMode.Auto);
            ISubscriber sub = _redisClient.GetSubscriber();

            if (inputCmd == "sub")
            {
                sub.Subscribe(channel, (c, v) =>
                {
                    if (!v.IsNull)
                    {
                        Console.WriteLine(v.GetHashCode());
                        //Console.WriteLine(v.ToString());
                    }
                });
            }
            else
            {
                do
                {
                    for (int i = 0; i < 100; i++)
                    {
                        var input = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "." + i.ToString().PadLeft(3, '0') + "." + Guid.NewGuid().ToString();
                        for (int j = 0; j < 100; j++)
                        {
                            input += Guid.NewGuid();
                        }

                        if (sub.IsConnected())
                        {
                            var number = sub.Publish(channel, input);
                            Console.WriteLine(number);
                        }
                        
                    }
                    
                    Console.WriteLine("Input 'q' to break publish.");
                    inputCmd = Console.ReadLine();
                } while (inputCmd != "q");
            }

            Console.WriteLine("\nPress Any Key To Exit...");
            Console.ReadLine();
        }
    }
}
