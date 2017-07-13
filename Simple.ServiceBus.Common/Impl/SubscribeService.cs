using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;

namespace Simple.ServiceBus.Common.Impl
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class SubscribeService : ISubscribeService
    {
        protected OperationContext Context
        {
            get
            {
                return OperationContext.Current;
            }
        }

        public void Subscribe(string requestKey)
        {
            IPublishService subscriber = Context.GetCallbackChannel<IPublishService>();
            
            Trace.WriteLine(Context.GetClientAddress() +  " Subscribed");
            
            ServiceRegister.GlobalRegister.Register(requestKey, subscriber);
        }

        public void UnSubscribe(string requestKey)
        {
            IPublishService subscriber = Context.GetCallbackChannel<IPublishService>();
            
            Trace.WriteLine(Context.GetClientAddress() + " UnSubscribed");

            ServiceRegister.GlobalRegister.UnRegister(requestKey, subscriber);
        }

        public string Ping()
        {
            var result = Context.GetClientAddress() + " ping current host(" + Dns.GetHostName() + ") at:" + DateTime.Now.ToString();
            Trace.WriteLine(result);
            
            return result;
        }
    }
}
