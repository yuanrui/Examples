using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Simple.ServiceBus.Common
{
    public abstract class IBusData
    {
        
    }

    public interface IMessage 
    {
        MessageHeader Header { get; set; }

        Object Body { get; set; }
    }

    [DataContract, Serializable]
    [KnownType("GetKnownTypes")]
    public class Message //: IMessage
    {
        [DataMember]
        public MessageHeader Header { get; set; }

        [DataMember]
        public virtual Object Body { get; set; }

        private static Type[] GetKnownTypes()
        {
            Type thisType = typeof(IBusData);
            var result = thisType
                .Assembly
                .GetTypes()
                .Where(t => t.IsSubclassOf(thisType) || t.GetInterface(thisType.Name) == thisType)
                .ToArray();

            return result;
        } 
    }

    [KnownType("GetKnownTypes")]
    public class Message<T> : Message where T : IBusData
    {
        public Message(T data)
        {
            Body = data;
        }

        private static Type[] GetKnownTypes()
        {
            Type thisType = typeof(IBusData);
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

            return msg;
        }
    }
}
