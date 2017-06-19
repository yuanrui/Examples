using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;

namespace Simple.ServiceBus.Common.Impl
{
    public class SubscriptionClient : ISubscription, IPublishing, IPublishing<Test1>, IPublishing<Test2>
    {
        ISubscription _proxy;
        Timer _timer;
        string initTime;

        public SubscriptionClient()
        {
            MakeProxy(ServiceSetting.SubAddress, this);
            _timer = new Timer(DoPing, null, Timeout.Infinite, 5000);
        }

        public void MakeProxy(string endpoindAddress, object callbackinstance)
        {
            NetTcpBinding tcpBinding = new NetTcpBinding(SecurityMode.None);
            tcpBinding.MaxReceivedMessageSize = Int32.MaxValue;
            tcpBinding.MaxBufferSize = Int32.MaxValue;
            tcpBinding.MaxBufferPoolSize = 67108864;

            tcpBinding.SendTimeout = TimeSpan.FromMinutes(1);
            tcpBinding.ReceiveTimeout = TimeSpan.FromMinutes(1);
            
            EndpointAddress endpointAddress = new EndpointAddress(endpoindAddress);
            InstanceContext context = new InstanceContext(callbackinstance);

            DuplexChannelFactory<ISubscription> channelFactory = new DuplexChannelFactory<ISubscription>(new InstanceContext(this), tcpBinding, endpointAddress);
            _proxy = channelFactory.CreateChannel();
            initTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        
        public void Subscribe(string requestKey)
        {
            _proxy.Subscribe(requestKey);
            _timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(30));
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

            Trace.WriteLine(string.Format("Key:{0} Id:{1} Msg:{2}", message.Header.MessageKey, message.Header.RequestKey, message.Body.ToString()));
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
            Trace.WriteLine("Message<Test1>:" + message.Body.ToString());
        }

        public void Publish(Message<Test2> message)
        {
            Trace.WriteLine("Message<Test2>:" + message.Body.ToString());
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

        private void DoPing(object obj)
        {
            Ping();
        }

        public string Ping()
        {
            var now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            try
            {
                var result = _proxy.Ping();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(result);
                Console.ForegroundColor = ConsoleColor.Gray;

                return result;
            }
            catch (CommunicationObjectFaultedException ex)
            {
                Trace.WriteLine("initTime:" + initTime + " exTime:" + now);
                Trace.WriteLine(ex.Message);

                //MakeProxy(ServiceSetting.SubAddress, this);
            }

            return string.Empty;
        }
    }
}
