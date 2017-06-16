using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Simple.ServiceBus.Common.Impl
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class SubscriptionService : ISubscription
    {
        public void Subscribe(string requestKey)
        {
            IPublishing subscriber = OperationContext.Current.GetCallbackChannel<IPublishing>();
            
            Trace.WriteLine(" Subscribed");
            
            ServiceRegister.GlobalRegister.Register(requestKey, subscriber);
        }

        public void UnSubscribe(string requestKey)
        {
            IPublishing subscriber = OperationContext.Current.GetCallbackChannel<IPublishing>();
            
            Trace.WriteLine( " UnSubscribed");

            ServiceRegister.GlobalRegister.UnRegister(requestKey, subscriber);
        }
    }
}
