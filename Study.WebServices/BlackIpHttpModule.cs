using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace Study.WebServices
{
    public class BlackIpHttpModule : IHttpModule
    {
        protected string BlockedIpPath { get; private set; }

        public BlackIpHttpModule()
        {
            BlockedIpPath = GetBlockedIpPath();
        }

        public void Dispose()
        {
            
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(context_BeginRequest);
        }

        void context_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication app = sender as HttpApplication;
            if (app == null || app.Context == null)
            {
                return;
            }

            var clientIP = GetClientIP(app.Context.Request);

            if (string.IsNullOrWhiteSpace(clientIP) 
                || clientIP == "::1" 
                || clientIP == "127.0.0.1" 
                || clientIP == "localhost")
            {
                return;
            }

            var badIPs = GetBlockedIPs(app.Context);
            if (badIPs == null || badIPs.Count == 0 || ! badIPs.Contains(clientIP))
            {
                return;
            }

            app.Context.Response.StatusCode = 404;
            app.Context.Response.SuppressContent = true;
            app.Context.Response.End();
        }

        private HashSet<string> GetBlockedIPs(HttpContext context)
        {
            const string BLOCKED_IP_KEY = "blockedips";
            var ips = context.Cache[BLOCKED_IP_KEY] as HashSet<string>;
            if (ips == null)
            {
                var configPath = GetBlockedIpPath();
                ips = GetBlockedIPs(BlockedIpPath);
                context.Cache.Insert(BLOCKED_IP_KEY, ips);
            }
            
            return ips;
        }

        private string GetBlockedIpPath()
        {
            var basePath = HttpRuntime.AppDomainAppPath;
            return Path.Combine(basePath, "bin", "blockedips.txt");
        }

        private HashSet<string> GetBlockedIPs(string configPath)
        {
            if (! File.Exists(configPath))
            {
                return new HashSet<string>();
            }

            var inputs = File.ReadAllLines(configPath);
            HashSet<string> ips = new HashSet<string>();

            foreach (var item in inputs)
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }

                ips.Add(item.Trim());
            }
            
            return ips;
        }

        private string GetClientIP(HttpRequest request)
        {
            string ip = string.Empty;
            try
            {
                if (request.IsSecureConnection)
                {
                    ip = request.ServerVariables["REMOTE_ADDR"];
                }

                if (string.IsNullOrEmpty(ip))
                {
                    ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (!string.IsNullOrEmpty(ip))
                    {
                        if (ip.Any(m => m == ','))
                        {
                            ip = ip.Split(',').Last();
                        }
                    }
                    else
                    {
                        ip = request.UserHostAddress;
                    }
                }
            }
            catch
            {
                ip = string.Empty;
            }
            
            return ip;
        }
    }
}