using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Learning.EF6
{
    class Program
    {
        static void Main(string[] args)
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
            var ctx = new LocalContext();
            var list = GetCategories(ctx);

            const string urlFormat = "";
            //ctx.Database.Initialize(true);
            
            var urlList = new List<string>();
            foreach (var cate in list)
            {
                foreach (var id in cate.RawIds)
                {
                    urlList.Add(string.Format(urlFormat, id));
                }                
            }
            
            SimpleSpider spider = new SimpleSpider(5, urlList);

            spider.Start();
            KeyValuePair<string, string> kv;

            while (true)
            {
                Console.WriteLine("Queue Count:{0}", spider.UrlResponses.Count);

                var isDeqed = spider.UrlResponses.TryDequeue(out kv);

                if (! isDeqed)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                AddJoke(kv, ctx);
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }

        public static DateTime FromTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp / 1000); // convert from milliseconds to seconds
        }

        public static double ToTimestamp(DateTime time)
        {
            return time.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        static List<Category> GetCategories(LocalContext ctx)
        {
            return ctx.Categories.ToList();
        }

        static void AddJoke(KeyValuePair<string, string> kv, LocalContext ctx)
        {
            string content = kv.Value;

            if (content == "no")
            {
                return;
            }

            var jokes = content.Split('@');

            foreach (var joke in jokes)
            {
                var ent = new Joke();
                var jokeArray = joke.Split('#');
                if (jokeArray != null && jokeArray.Length >= 3)
                {
                    ent.Id = jokeArray[0];
                    ent.Title = jokeArray[1];
                    ent.Content = jokeArray[2];
                    ent.OriginalUrl = kv.Key;
                    ent.Valid = true;
                    ent.GrabTime = Convert.ToInt64(ToTimestamp(DateTime.Now));
                }

                if (string.IsNullOrEmpty(ent.Id))
                {
                    continue;
                }

                if (ctx.Jokes.All(m => m.Id != ent.Id))
                {
                    ctx.Jokes.Add(ent);
                }
            }

            ctx.SaveChanges();
        }
    }
}
