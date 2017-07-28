using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;

namespace Simple.ServiceBus.Host
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)]
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

            ServiceRegister.GlobalRegister.Register(requestKey, subscriber, Context.GetClientAddress());
        }

        public void UnSubscribe(string requestKey)
        {
            IPublishService subscriber = Context.GetCallbackChannel<IPublishService>();
            
            Trace.WriteLine(Context.GetClientAddress() + " UnSubscribed");

            ServiceRegister.GlobalRegister.UnRegister(requestKey, subscriber);
        }

        public string Ping(string[] keys)
        {
            IPublishService subscriber = Context.GetCallbackChannel<IPublishService>();
            var result = string.Empty;
            foreach (var key in keys)
	        {
                var list = ServiceRegister.GlobalRegister.GetHandler(key);

                foreach (var item in list)
                {
                    if (subscriber != item.Key)
                    {
                        continue;
                    }

                    item.Value.Active();

                    result = Context.GetClientAddress() + " key:" + key + " ping current host(" + Dns.GetHostName() + ") at:" + DateTime.Now.ToString();
                    Trace.WriteLine(result);
                }
	        }

            if (string.IsNullOrEmpty(result))
            {
                foreach (var key in keys)
                {
                    ServiceRegister.GlobalRegister.Register(key, subscriber, Context.GetClientAddress());
                }
            }

            return result;
        }
    }
}
