using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Simple.ServiceBus.Messages;

namespace Simple.ServiceBus.Host
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
        
        public Message Publish(Message message)
        {
            Message result = null;

            var subscribers = ServiceRouting.GlobalRouting.GetHandlers(message.Header.RequestKey, message.Header.RouteType).ToList();
            if (subscribers == null || subscribers.Count == 0)
            {
                var msg = Context.GetClientAddress() + " RequestKey:" + message.Header.RequestKey + " no Sub.";
                Trace.WriteLine(msg);
                
                result = new Message();
                result.Header = message.Header;
                var cmd = new NotSubscriberCommand();
                cmd.RequestKey = message.Header.RequestKey;
                
                result.Body = cmd;

                return result;
            }
            var watch = Stopwatch.StartNew();
            for (int i = 0; i < subscribers.Count; i++)
            {
                var subscriber = subscribers[i];
                
                try
                {
                    var tmpResult = subscriber.Publish(message);
                    if (result == null)
	                {
                        result = tmpResult;
	                }
                    else
                    {
                        result.SetNext(tmpResult);
                    }
                    
                    
                }
                catch (CommunicationException ex)
                {
                    ServiceRouting.GlobalRouting.UnRegister(message.Header.RequestKey, subscriber);
                    
                    if (result == null)
                    {
                        result = new Message();
                        result.Header = message.Header;
                    }

                    var cmd = new CommExceptionCommand();
                    cmd.OriginalObject = message.Body;
                    cmd.RequestKey = message.Header.RequestKey;
                    cmd.ExceptionMessage = ex.Message;
                    
                    result.Body = cmd;

                    Trace.WriteLine(Context.GetClientAddress() + " RequestKey:" + message.Header.RequestKey + " removed. Exception:" + ex.Message);
                }
            }

            watch.Stop();
            
            Trace.WriteLine(Context.GetClientAddress() + " Published " + message.Header.MessageKey + " total time:" + Math.Round(watch.Elapsed.TotalSeconds, 2) + "s");
            
            return result;
        }
    }
}
