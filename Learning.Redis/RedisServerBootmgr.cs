using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Learning.Redis
{
    public static class RedisServerBootmgr
    {
        const String REDIS_SERVER = "redis-server.exe";
        const String REDIS_CONF = "redis.windows.conf";

        public static void Start()
        {
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = REDIS_SERVER;
            startInfo.Arguments = REDIS_CONF;
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            Process.Start(startInfo);
        }
    }
}
