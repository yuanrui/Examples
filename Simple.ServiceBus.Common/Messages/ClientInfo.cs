using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Simple.ServiceBus.Messages
{
    [DataContract]
    internal class ClientInfo
    {
        [IgnoreDataMember]
        public const string KEY = "Simple.ServiceBus.Common.ClientInfo";
        [IgnoreDataMember]
        public const string HEADER_NAMESPACE = "Simple.ServiceBus.Common";

        [IgnoreDataMember]
        private static ClientInfo _current;

        [DataMember]
        public string MachineName { get; set; }

        [DataMember]
        public string ProcessName { get; set; }

        [DataMember]
        public int ProcessId { get; set; }

        [DataMember]
        public string IP { get; set; }

        [DataMember]
        public int Port { get; set; }
        
        public static ClientInfo GetCurrent()
        {
            return _current;
        }

        static ClientInfo()
        {
            _current = new ClientInfo();
            
            var process = Process.GetCurrentProcess();
            _current.MachineName = process.MachineName;
            _current.ProcessId = process.Id;
            _current.ProcessName = process.ProcessName;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(IP) || Port == 0)
            {
                return ProcessName + ":" + ProcessId;
            }

            return IP + ":" + Port;
        }
    }
}
