using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace Learning.EF6
{
    class Program
    {
        static void Main(string[] args)
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
            var list = new List<Category>();
            var ctx = new LocalContext();
            const string urlFormat = "";

            var urlList = new List<string>();

            for (int i = 0; i < 2000; i++)
            {
                urlList.Add(string.Format(urlFormat, i, ToTimestamp(DateTime.Now.AddSeconds(i))));
            }
            SimpleSpider spider = new SimpleSpider(5, urlList);

            spider.Start();

            while (true)
            {
                Console.WriteLine("Queue Count:{0}", spider.UrlResponses.Count);
                var input = Console.ReadLine();
                if (input == "Q")
                {
                    break;
                }
            }

            //for (int i = 0; i < 10; i++)
            //{
            //    var category = new Category();
            //    category.Name = "Test" + i + category.Id;
            //    category.SortId = i;
            //    ctx.Categories.Add(category);
                
            //}
            //ctx.SaveChanges();

            //ctx.Categories.RemoveRange(ctx.Categories);
            //ctx.SaveChanges();
            //foreach (var item in ctx.Categories)
            //{
            //    Console.WriteLine(item.Id);
            //}
            
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
    }
}
