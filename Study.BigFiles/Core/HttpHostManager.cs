using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace Study.BigFiles
{
    public class HttpHostManager
    {
        public HostConfig Config { get; private set; }

        public HttpHostManager()
        {
            this.Config = ConfigurationManager.GetSection(BigFileHttpHost.HOST_CONFIG_SECTION) as HostConfig;
        }

        public void Start()
        {
            try
            {
                if (this.Config == null)
                {
                    Trace.WriteLine("配置节点[" + BigFileHttpHost.HOST_CONFIG_SECTION + "]不存在，无法运行程序。");
                    Trace.WriteLine("Exiting...");
                    Thread.Sleep(3000);
                    Environment.Exit(0);
                    return;
                }

                foreach (HostElement setting in this.Config.Hosts)
                {
                    BigFileHttpHost host = new BigFileHttpHost(setting.Port, setting.GetFilePath(), setting.FileSize, setting.User, setting.Passwd);
                    host.Start();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Start Exception:" + ex);
            }

        }

        public void Stop()
        {
            if (this.Config == null)
            {
                return;
            }

            try
            {                
                foreach (HostElement setting in this.Config.Hosts)
                {
                    BigFileHttpHost host = new BigFileHttpHost(setting.Port, setting.GetFilePath(), setting.FileSize, setting.User, setting.Passwd);
                    host.Stop();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Stop Exception:" + ex);
            }
        }
    }
}
