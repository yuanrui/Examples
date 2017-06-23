using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.ServiceBus.Common
{
    public class RequestMessage : Message
    { 
        
    }

    public class RequestMessage<T> : RequestMessage where T : ICommand
    {
        public RequestMessage()
        {
            TypeName = this.GetType().FullName;
        }

        public RequestMessage(T data)
        {
            Body = data;

            TypeName = this.GetType().FullName;
        }
    }
}
