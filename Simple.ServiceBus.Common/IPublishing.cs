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

        [OperationContract(IsOneWay = false)]
        ResponseMessage PublishSync(RequestMessage message);
    }
    
    public static class IPublishingExtension
    {
        public static void Publish2<T>(this IPublishing pub, Message<T> message) where T : IBusEntity
        {
            pub.Publish(message.ToMessage());
        }
    }

    [ServiceContract]
    public interface IPublishing<T> : IPublishing where T : IBusEntity
    {
        [OperationContract(IsOneWay = true)]
        void Handle(Message<T> message);
    }

}
