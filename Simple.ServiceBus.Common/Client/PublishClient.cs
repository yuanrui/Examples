using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Simple.ServiceBus.Configuration;
using Simple.ServiceBus.Inspect;
using Simple.ServiceBus.Messages;

namespace Simple.ServiceBus.Client
{
    public class PublishClient
    {
        protected ChannelFactory<IPublishService> Factory { get; private set; }

        public PublishClient()
        {
            Factory = CreateChannelFactory();
        }

        protected ChannelFactory<IPublishService> CreateChannelFactory()
        {
            EndpointAddress endpointAddress = new EndpointAddress(NetSetting.PubAddress);
            
            var binding = NetSetting.GetBinding();
            
            var result = new ChannelFactory<IPublishService>(binding, endpointAddress);

            result.Endpoint.Behaviors.Add(new BusClientBehavior());
            
            return result;
        }

        public Message Send(Message message)
        {
            Message result = null;
            var proxy = Factory.CreateChannel();
            bool success = false;

            try
            {
                result = proxy.Publish(message);

                ((IClientChannel)proxy).Close();
                success = true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Publish Exception:" + ex.Message);
            }
            finally
            {
                if (! success)
                {
                    ((IClientChannel)proxy).Abort();
                }
            }

            return result;
        }

        public Message<TOut> Send<TIn, TOut>(Message<TIn> message)
            where TIn : class, ICommand
            where TOut : class, ICommand
        {
            Message dto = new Message() { Header = message.Header, Body = message.Body, TypeName = message.TypeName };

            var result = Send(dto);

            if (result.TypeName == null)
            {
                return new Message<TOut>();
            }

            var type = Type.GetType(result.TypeName);
            var instance = Activator.CreateInstance(type, result.Body, result.Header);

            return instance as Message<TOut>;
        }
    }
}
