// Copyright (c) 2018 YuanRui
// GitHub: https://github.com/yuanrui
// License: Apache-2.0

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace _Local.ConsoleApp
{
    public class HttpListenerDemo
    {
        public static void RunHost()
        { 
            const string URI_PREFIX_FORMAT = "http://{0}:12345/HelloWorld/";
            var listerner = CreateHttpListener(URI_PREFIX_FORMAT);
            listerner.Start();

            while (true)
            {
                Console.WriteLine("waiting request...");
                var ctx = listerner.GetContext();
                Task.Factory.StartNew(obj =>
                {
                    var httpCtx = obj as HttpListenerContext;
                    
                    Console.WriteLine(httpCtx.Request.Url);

                    using (var reader = new StreamReader(httpCtx.Request.InputStream, Encoding.UTF8))
                    {
                        //read post data
                        var body = reader.ReadToEnd() ?? string.Empty;
                        var list = HttpUtility.ParseQueryString(body);//key value list
                        Console.WriteLine(body);
                    }

                    //Response
                    using (var writer = new StreamWriter(httpCtx.Response.OutputStream))
                    {
                        writer.WriteLine("Hello World! " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        writer.Close();
                    }

                    httpCtx.Response.StatusCode = 200;
                    httpCtx.Response.ContentType = "application/json";
                    httpCtx.Response.ContentEncoding = Encoding.UTF8;
                    httpCtx.Response.Close();
                }, ctx);
            }
        }

        private static HttpListener CreateHttpListener(string uriPrefixFormat)
        {
            var listerner = new HttpListener();
            listerner.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily != AddressFamily.InterNetwork)
                {
                    continue;
                }
                listerner.Prefixes.Add(string.Format(uriPrefixFormat, ip.ToString()));
            }
            listerner.Prefixes.Add(string.Format(uriPrefixFormat, "localhost"));
            listerner.Prefixes.Add(string.Format(uriPrefixFormat, "127.0.0.1"));

            return listerner;
        }

        public static void RunClient()
        {
            WebClient webClient = new WebClient();
            System.Collections.Specialized.NameValueCollection list = new System.Collections.Specialized.NameValueCollection();
            list.Add("Name", "abc");
            list.Add("Password", "123456");
            var rsp = webClient.UploadValues("http://localhost:12345/HelloWorld", "POST", list);
            Console.WriteLine(Encoding.UTF8.GetString(rsp));
        }
    }
}
