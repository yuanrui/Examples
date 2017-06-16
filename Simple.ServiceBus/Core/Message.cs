using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.ServiceBus.Core
{
    public class Message
    {
        public MessageHeader Header { get; set; }

        public object Body { get; set; }

    }
}
