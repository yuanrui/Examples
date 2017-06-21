using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Simple.ServiceBus.Common
{
    [DataContract]
    public class MessageHeader
    {
        [DataMember]
        public string MessageKey { get; set; }

        [DataMember]
        public string RequestKey { get; set; }

        [DataMember]
        public string ResponseKey { get; set; }

        [DataMember]
        public int RunCount { get; set; }

        public MessageHeader()
        {
            MessageKey = Guid.NewGuid().ToString();
        }
    }
}
