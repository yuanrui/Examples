using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace Simple.ServiceBus.Configuration
{
    public class NetSetting
    {
        public static string PubAddress 
        {
            get 
            {
                return HostAddress + "/Pub";
            }
        }

        public static string SubAddress
        {
            get 
            {
                return HostAddress + "/Sub";
            }
        }

        public static string HostAddress 
        {
            get
            {
                return (ConfigurationManager.AppSettings["hostAddress"] ?? string.Empty).TrimEnd('/');
            }
        }

        public static Binding GetBinding()
        {
            const Int64 MaxReceivedMessageSize = Int32.MaxValue;
            const Int32 MaxBufferSize = Int32.MaxValue;
            const Int64 MaxBufferPoolSize = 67108864;

            if (HostAddress.StartsWith("net.pipe", StringComparison.OrdinalIgnoreCase))
            {
                var namedPipeBinding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
                namedPipeBinding.MaxReceivedMessageSize = MaxReceivedMessageSize;
                namedPipeBinding.MaxBufferSize = MaxBufferSize;
                namedPipeBinding.MaxBufferPoolSize = MaxBufferPoolSize;

                SetTimeout(namedPipeBinding);

                return namedPipeBinding;
            }

            var tcpBinding = new NetTcpBinding(SecurityMode.None);
            tcpBinding.MaxReceivedMessageSize = MaxReceivedMessageSize;
            tcpBinding.MaxBufferSize = MaxBufferSize;
            tcpBinding.MaxBufferPoolSize = MaxBufferPoolSize;

            SetTimeout(tcpBinding);

            return tcpBinding;
        }

        private static void SetTimeout(Binding tcpBinding)
        {
            tcpBinding.SendTimeout = TimeSpan.FromMinutes(1);
            tcpBinding.ReceiveTimeout = TimeSpan.FromMinutes(1);
        }
    }
}
