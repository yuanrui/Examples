using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Simple.ServiceBus.Common.Impl
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class PublishService : IPublishService
    {
        protected OperationContext Context
        {
            get
            {
                return OperationContext.Current;
            }
        }

        public void Publish(Message message)
        {
            var subscribers = ServiceRouting.GlobalRouting.GetHandlers(message.Header.RequestKey);
            if (subscribers == null || subscribers.Count == 0)
            {
                Trace.WriteLine(Context.GetClientAddress() + " RequestKey:" + message.Header.RequestKey + " no Sub.");
                return;
            }

            for (int i = 0; i < subscribers.Count; i++)
            {
                var subscriber = subscribers[i];
                try
                {
                    subscriber.Publish(message);
                    Trace.WriteLine(Context.GetClientAddress() + " Published " + message.GetHashCode());
                }
                catch (Exception ex)
                {
                    ServiceRouting.GlobalRouting.UnRegister(message.Header.RequestKey, subscriber);

                    Trace.WriteLine(Context.GetClientAddress() + " RequestKey:" + message.Header.RequestKey + " removed. Exception:" + ex.Message);
                }
            }
        }

        public Message PublishSync(Message message)
        {
            var subscribers = ServiceRouting.GlobalRouting.GetHandlers(message.Header.RequestKey);
            if (subscribers == null || subscribers.Count == 0)
            {
                var msg = Context.GetClientAddress() + " RequestKey:" + message.Header.RequestKey + " no Sub.";
                Trace.WriteLine(msg);
                return null;
            }

            Message result = null;

            for (int i = 0; i < subscribers.Count; i++)
            {
                var subscriber = subscribers[i];
                try
                {
                    var tmpResult = subscriber.PublishSync(message);
                    if (result == null)
	                {
                        result = tmpResult;
	                }
                    else
                    {
                        result.SetNext(tmpResult);
                    }
                    
                    Trace.WriteLine(Context.GetClientAddress() + " Published " + message.GetHashCode());
                }
                catch (CommunicationObjectAbortedException ex)
                {
                    ServiceRouting.GlobalRouting.UnRegister(message.Header.RequestKey, subscriber);

                    Trace.WriteLine(Context.GetClientAddress() + " RequestKey:" + message.Header.RequestKey + " removed. Exception:" + ex.Message);
                }
            }

            return result;
        }
    }
}
