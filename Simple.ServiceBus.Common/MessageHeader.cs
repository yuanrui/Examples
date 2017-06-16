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

        public MessageHeader()
        {
            MessageKey = Guid.NewGuid().ToString();
        }
    }
}
