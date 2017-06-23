using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Simple.ServiceBus.Common
{
    [DataContract, Serializable]
    [KnownType("GetKnownTypes")]
    public class Message 
    {
        private Object _body;

        [DataMember]
        public virtual MessageHeader Header { get; set; }

        [DataMember]
        public virtual Object Body
        {
            get { return _body; }
            set
            {
                if (value != null)
                {
                    BodyType = value.GetType().FullName;
                }

                _body = value;
            }
        }

        [DataMember]
        public virtual string BodyType { get; set; }

        [DataMember]
        public virtual string TypeName { get; set; }

        protected static Type[] GetKnownTypes()
        {
            Type thisType = typeof(ICommand);
            var result = thisType
                .Assembly
                .GetTypes()
                .Where(t => t.IsSubclassOf(thisType) || t.GetInterface(thisType.Name) == thisType)
                .ToArray();

            return result;
        } 
    }

    [KnownType("GetKnownTypes")]
    public class Message<T> : Message where T : ICommand
    {
        public Message()
        {
            TypeName = this.GetType().FullName;
        }

        public Message(T data)
        {
            Body = data;

            TypeName = this.GetType().FullName;
        }

        private static Type[] GetKnownTypes()
        {
            Type thisType = typeof(ICommand);
            var result = thisType
                .Assembly
                .GetTypes()
                .Where(t => t.IsSubclassOf(thisType) || t.GetInterface(thisType.Name) == thisType)
                .ToArray();

            return result;
        } 

        public Message ToMessage()
        {
            var msg = new Message();
            msg.Header = this.Header;
            msg.Body = this.Body;
            msg.TypeName = this.TypeName;

            return msg;
        }
    }
}
