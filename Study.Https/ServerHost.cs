// Copyright (c) 2021 YuanRui
// GitHub: https://github.com/yuanrui
// License: Apache-2.0

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Study.Https
{
    public class ServerHost
    {
        const String DEFAULT_URL_PREFIX = "http://+:8902/";

        const String URL_Console = "System/Console";

        protected Int32 _port;
        protected String _uriPrefix;
        protected HttpListener _httpListener;
        protected Thread _serverThread;
        SemaphoreSlim _semaphoreSlim;
        private static Object _syncObject = new Object();

        public ServerHost(Int32 port)
        {
            _port = port;
            _semaphoreSlim = new SemaphoreSlim(100, 100);
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
            _uriPrefix = "https://+:" + _port + "/";
            _httpListener = new HttpListener();
            _httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            _httpListener.Prefixes.Add(_uriPrefix);
        }

        public void Start()
        {
            _serverThread = new Thread(obj => {
                HttpListener listener = (HttpListener)obj;
                Run(listener);
            });

            _serverThread.Name = "ServerHost";
            _serverThread.IsBackground = false;

            _httpListener.Start();
            _serverThread.Start(_httpListener);
            Trace.WriteLine(_uriPrefix + " 已经启动监听。");
        }

        public void Stop()
        {
            _httpListener.Stop();
        }

        public void Run(HttpListener listerner)
        {
            while (listerner.IsListening)
            {
                Console.WriteLine("Http等待请求中...");
                HttpListenerContext ctx = listerner.GetContext();
                Task.Factory.StartNew(obj =>
                {
                    HttpListenerContext httpCtx = obj as HttpListenerContext;

                    try
                    {
                        DoHandler(httpCtx);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ServerHost异常:" + ex + Environment.NewLine + "Url:" + httpCtx.Request.Url);
                    }
                    finally
                    {
                        Thread.Sleep(1);
                    }
                }, ctx, TaskCreationOptions.LongRunning);

                Thread.Sleep(1);
            }
        }

        protected void DoHandler(HttpListenerContext ctx)
        {
            String url = (ctx.Request.RawUrl ?? String.Empty).Trim('/');
            Console.WriteLine(String.Format("客户端:{0} {1} 请求地址:{2}", ctx.Request.RemoteEndPoint.Address, ctx.Request.HttpMethod, ctx.Request.Url));
            Stopwatch stopwatch = new Stopwatch();

            if (url.StartsWith(URL_Console, StringComparison.OrdinalIgnoreCase))
            {
                DoConsole(ctx, url);
                return;
            }

            StartInfo(ctx);
        }

        private void StartInfo(HttpListenerContext ctx)
        {
            const Int32 MB_SIZE = 1048576;
            Process process = Process.GetCurrentProcess();

            PerformanceCounter pcMemory = new PerformanceCounter("Process", "Working Set - Private", process.ProcessName);
            PerformanceCounter pcCpuLoad = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            pcCpuLoad.NextValue();

            CounterSample cs1 = pcCpuLoad.NextSample();
            Thread.Sleep(200);
            CounterSample cs2 = pcCpuLoad.NextSample();
            Single finalCpuCounter = CounterSample.Calculate(cs1, cs2);

            using (StreamWriter writer = new StreamWriter(ctx.Response.OutputStream, Encoding.UTF8))
            {
                writer.WriteLine(_serverThread.Name + " 已经启动!");
                writer.WriteLine("启动时间:" + process.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));
                writer.WriteLine("系统时间:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                writer.WriteLine("请求地址:" + ctx.Request.RawUrl);
                writer.WriteLine("客户端IP:" + ctx.Request.RemoteEndPoint.Address);
                writer.WriteLine("线程数量:" + process.Threads.Count);
                writer.WriteLine("程序内存:" + Math.Round(pcMemory.NextValue() / MB_SIZE, 2) + "MB");
                writer.WriteLine("系统CPU:" + Math.Round(finalCpuCounter, 0) + "%");
                writer.WriteLine("OS版本:" + Environment.OSVersion.VersionString);
                ctx.Response.StatusCode = (Int32)HttpStatusCode.OK;
                ctx.Response.ContentType = "text/plain";
                ctx.Response.ContentEncoding = Encoding.UTF8;
                writer.Close();
            }
        }

        private void DoDownloadImage(HttpListenerContext ctx, String imgUrl)
        {
            Byte[] fileBuffer = new Byte[0];
            ctx.Response.StatusCode = (Int32)HttpStatusCode.OK;
            ctx.Response.ContentType = "image/jpeg";
            ctx.Response.ContentEncoding = Encoding.UTF8;

            if (String.IsNullOrWhiteSpace(imgUrl))
            {
                ctx.Response.StatusCode = (Int32)HttpStatusCode.NotFound;
                ctx.Response.Close();
                return;
            }
            var uri = new Uri(imgUrl);
            var url = uri.AbsoluteUri.Replace("#", "%23");
            var waitStatus = false;
            try
            {
                waitStatus = _semaphoreSlim.Wait(TimeSpan.FromSeconds(3));
                if (!waitStatus)
                {
                    throw new Exception("客户端并发请求过多");
                }

                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Timeout = 3000;
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                using (Stream responseStream = response.GetResponseStream())
                {
                    var buffer = new byte[65536];//64kb
                    var totalSize = 0;
                    var size = responseStream.Read(buffer, 0, (int)buffer.Length);

                    using (BinaryWriter writer = new BinaryWriter(ctx.Response.OutputStream))
                    {
                        while (size > 0)
                        {
                            totalSize += size;
                            writer.Write(buffer, 0, size);
                            size = responseStream.Read(buffer, 0, (int)buffer.Length);
                        }
                        writer.Close();
                    }

                    responseStream.Close();
                }
            }
            catch (Exception ex)
            {
                if (ex is WebException)
                {
                    ctx.Response.StatusCode = (Int32)HttpStatusCode.NotFound;
                }
                else
                {
                    ctx.Response.StatusCode = (Int32)HttpStatusCode.InternalServerError;
                }

                Console.WriteLine("下载图片异常:{0} 请求地址Url:{1} 原始Url:{2}", ex, ctx.Request.Url, imgUrl);
            }
            finally
            {
                if (waitStatus)
                {
                    _semaphoreSlim.Release();
                }
            }

            ctx.Response.Close();
        }

        protected void DoConsole(HttpListenerContext ctx, String url)
        {
            const Int32 DEFAULT_TIMEOUT = 60;
            var id = "http." + ctx.Request.RequestTraceIdentifier.ToString();
            var timeOut = GetTimeOut(url, DEFAULT_TIMEOUT);
            var queue = new ConcurrentQueue<String>();
            var listener = new HttpTraceListener(queue);
            listener.Name = id;

            var now = DateTime.Now;
            try
            {
                ctx.Response.KeepAlive = true;
                ctx.Response.StatusCode = 200;
                ctx.Response.ContentType = "text/html";
                using (StreamWriter writer = new StreamWriter(ctx.Response.OutputStream, Encoding.UTF8))
                {
                    Console.WriteLine("客户端:" + ctx.Request.RemoteEndPoint.Address + ", 开启Web控制台.");
                    writer.Write("<style type='text/css'>body { background-color:black;color:white; }</style>");
                    writer.Write(listener.GetFormatedMessage("即将显示:" + timeOut + "分钟内的系统运行信息。"));

                    Trace.Listeners.Add(listener);

                    while ((DateTime.Now - now).TotalMinutes < timeOut)
                    {
                        var msgs = listener.GetMessages(100);
                        foreach (var msg in msgs)
                        {
                            writer.Write(listener.GetFormatedMessage(msg));
                            Thread.Sleep(1);
                        }
                        writer.Flush();

                        Thread.Sleep(20);
                    }
                    Trace.Listeners.Remove(listener);
                    writer.Write(listener.GetFormatedMessage("显示运行信息结束。"));
                }
            }
            catch (Exception ex)
            {
                Trace.Listeners.Remove(listener);
                Console.WriteLine("Url:" + url + ", 出现异常:" + ex.ToString());
            }
            finally
            {
                Trace.Listeners.Remove(id);
                Console.WriteLine("客户端:" + ctx.Request.RemoteEndPoint.Address + ", 关闭Web控制台.");

                queue = null;
            }
        }

        private Int32 GetTimeOut(String url, Int32 defaultTimeOut)
        {
            var timeOutParam = url.ToLower().Replace(URL_Console.ToLower(), String.Empty).Trim('/');
            var timeOut = defaultTimeOut;
            if (!Int32.TryParse(timeOutParam, out timeOut))
            {
                timeOut = defaultTimeOut;
            }

            return timeOut;
        }

        protected class HttpTraceListener : TraceListener
        {
            protected ConcurrentQueue<String> Queue;

            public HttpTraceListener(ConcurrentQueue<String> queue)
            {
                Queue = queue;
            }

            public override void Write(String message)
            {
                if (Queue == null)
                {
                    return;
                }

                Queue.Enqueue(message);
            }

            public override void WriteLine(String message)
            {
                if (Queue == null)
                {
                    return;
                }

                Queue.Enqueue(message);
            }

            public String GetFormatedMessage(String message)
            {
                var msg = String.Format("{0}>>{1}</br>", DateTime.Now.ToString("HH:mm:ss"), message);

                return msg;
            }

            public List<String> GetMessages(Int32 count = 100)
            {
                var list = new List<String>(count);
                while (!Queue.IsEmpty)
                {
                    String msg = null;
                    Queue.TryDequeue(out msg);
                    if (msg == null)
                    {
                        continue;
                    }

                    list.Add(msg);

                    if (list.Count >= count)
                    {
                        break;
                    }
                }

                return list;
            }
        }

    }
}
