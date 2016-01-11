using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace Learning.EF6
{
    public class SimpleSpider
    {
        private readonly Random _random;
        private readonly Thread[] _spiderlThreads;
        private readonly Queue<string> _urlQueues;
        private readonly ConcurrentQueue<KeyValuePair<string, string>> _urlResponses;

        public ConcurrentQueue<KeyValuePair<string, string>> UrlResponses
        {
            get 
            {
                return _urlResponses;
            }
        }

        public SimpleSpider(int threadCount, IEnumerable<string> urls)
        {
            if (urls == null)
	        {
		         urls = new List<string>();
	        }
            _random = new Random(Guid.NewGuid().GetHashCode());
            _spiderlThreads = new Thread[threadCount];
            _urlQueues = new Queue<string>(urls.Count());
            _urlResponses = new ConcurrentQueue<KeyValuePair<string, string>>();
            for (int i = 0; i < threadCount; i++)
            {
                _spiderlThreads[i] = new Thread(DoRequest);
            }

            foreach (var url in urls)
            {
                _urlQueues.Enqueue(url);
            }
        }

        public void Start()
        {
            for (int i = 0; i < _spiderlThreads.Count(); i++)
            {
                _spiderlThreads[i].Start();
            }   
        }

        public void Stop()
        {
            foreach (var thread in _spiderlThreads)
            {
                thread.Abort();
            }
        }

        private void DoRequest()
        {
            while (true)
            {
                var url = string.Empty;
                var html = string.Empty;
                if (_urlQueues.Count == 0)
                {
                    break;
                }

                lock (this)
                {
                    if (_urlQueues.Count == 0)
                    {
                        break;
                    }

                    url = _urlQueues.Dequeue();
                }
                try
                {
                    Trace.WriteLine(string.Format("Thread:{0} Begin Request Url:{1}", Thread.CurrentThread.ManagedThreadId, url));

                    var request = WebRequest.Create(url);
                    using (var response = request.GetResponse())
                    {
                        using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                        {
                            html = reader.ReadToEnd();
                        }
                        response.Close();
                    }

                    if (!string.IsNullOrEmpty(html) && !string.IsNullOrEmpty(url))
                    {
                        Trace.WriteLine(string.Format("Thread:{0} Request Url:{1} Complete", Thread.CurrentThread.ManagedThreadId, url));

                        _urlResponses.Enqueue(new KeyValuePair<string, string>(url, html));
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(string.Format("Thread:{0} Url:{1} Error:{2}", Thread.CurrentThread.ManagedThreadId, url, ex.Message));
                }
                
                Thread.Sleep(_random.Next(1000, 5000));
            }
        }
    }
}
