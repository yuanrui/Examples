using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Simple.ServiceBus.Common.Impl
{
    public class SubscriptionClient : ISubscription, IPublishing, IPublishing<Test1>, IPublishing<Test2>
    {
        ISubscription _proxy;

        public SubscriptionClient()
        {
            MakeProxy(ServiceSetting.SubAddress, this);
        }

        public void MakeProxy(string endpoindAddress, object callbackinstance)
        {
            NetTcpBinding tcpBinding = new NetTcpBinding(SecurityMode.None);
            tcpBinding.MaxReceivedMessageSize = Int32.MaxValue;
            tcpBinding.MaxBufferSize = Int32.MaxValue;
            tcpBinding.SendTimeout = TimeSpan.FromMinutes(1);
            tcpBinding.ReceiveTimeout = TimeSpan.FromMinutes(1);

            EndpointAddress endpointAddress = new EndpointAddress(endpoindAddress);
            InstanceContext context = new InstanceContext(callbackinstance);

            DuplexChannelFactory<ISubscription> channelFactory = new DuplexChannelFactory<ISubscription>(new InstanceContext(this), tcpBinding, endpointAddress);
            _proxy = channelFactory.CreateChannel();
        }
        
        public void Subscribe(string requestKey)
        {
            _proxy.Subscribe(requestKey);
        }

        public void UnSubscribe(string requestKey)
        {
            _proxy.UnSubscribe(requestKey);
        }

        public virtual void Publish(Message message)
        {
            if (message == null || message.Body == null)
            {
                Console.WriteLine("no message");
                return;
            }
            
            Console.WriteLine("Key:{0} Id:{1} Msg:{2}", message.Header.MessageKey, message.Header.RequestKey, message.Body.ToString());
        }


        //public void Publish(BodyBase obj)
        //{
        //    Message message = obj as Message;
        //    if (message == null)
        //    {
        //        Console.WriteLine("no message");
        //        return;
        //    }

        //    Console.WriteLine("Key:{0} Id:{1} Msg:{2}", message.Header.MessageKey, message.Header.RequestKey, message.Body.ToString());
        //}

        public void Publish(Message<Test1> message)
        {
            Console.WriteLine("Message<Test1>:" + message.Body.ToString());
        }

        public void Publish(Message<Test2> message)
        {
            Console.WriteLine("Message<Test2>:" + message.Body.ToString());
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
            return "Sub_" + DateTime.Now;
        }
    }
}
