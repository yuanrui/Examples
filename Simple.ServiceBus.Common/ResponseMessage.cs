using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.ServiceBus.Common
{
    public class ResponseMessage : Message 
    {
        public ResponseMessage Next { get; protected set; }

        public void SetNext(ResponseMessage next)
        {
            if (Next == null)
            {
                Next = next;

                return;
            }

            Next.SetNext(next);
        }
    }

    public class ResponseMessage<T> : ResponseMessage where T : IBusEntity
    {
        public ResponseMessage()
        {
            TypeName = this.GetType().FullName;
        }

        public ResponseMessage(T data)
        {
            Body = data;

            TypeName = this.GetType().FullName;
        }
    }
}
