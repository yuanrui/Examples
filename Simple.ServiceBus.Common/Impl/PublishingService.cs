using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Diagnostics;

namespace Simple.ServiceBus.Common.Impl
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class PublishingService : IPublishing
    {
        public void Publish(Message message)
        {
            var subscribers = ServiceRouting.GlobalRouting.GetHandlers(message.Header.RequestKey);
            if (subscribers == null || subscribers.Count == 0)
            {
                Trace.WriteLine("RequestKey:" + message.Header.RequestKey + " no Sub.");
                return;
            }
            
            for (int i = 0; i < subscribers.Count; i++)
            {
                var subscriber = subscribers[i];
                try
                {
                    subscriber.Publish(message);
                }
                catch (CommunicationObjectAbortedException ex)
                {
                    ServiceRouting.GlobalRouting.UnRegister(message.Header.RequestKey, subscriber);

                    Trace.WriteLine("RequestKey:" + message.Header.RequestKey + " removed. Exception:" + ex.Message);
                }
            }
        }

        public void Publish<T>(Message<T> message) where T : IBusData
        {
            throw new NotImplementedException();
        }

        public void Publish(IMessage message)
        {
            throw new NotImplementedException();
        }

        public string GetAppId()
        {
            return "Pub_" + DateTime.Now;
        }
    }
}
