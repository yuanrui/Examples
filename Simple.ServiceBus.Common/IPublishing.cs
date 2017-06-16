using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Simple.ServiceBus.Common
{
    [ServiceContract]
    public interface IPublishing
    {
        [OperationContract]
        string GetAppId();

        [OperationContract(IsOneWay = true)]
        void Publish(Message message);
    }
    
    public static class IPublishingExtension
    {
        public static void Publish2<T>(this IPublishing pub, Message<T> message) where T : IBusData
        {
            pub.Publish(message.ToMessage());
        }
    }

    [ServiceContract]
    public interface IPublishing<T> : IPublishing where T : IBusData
    {
        [OperationContract(IsOneWay = true)]
        void Publish(Message<T> message);
    }

}
