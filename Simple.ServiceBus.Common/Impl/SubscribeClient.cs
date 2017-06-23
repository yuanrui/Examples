using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.ComponentModel;

namespace Simple.ServiceBus.Common.Impl
{
    public class SubscribeClient : ISubscribeService, IPublishService, IBusCommond<Test1>, IBusCommond<Test2>
    {
        ISubscribeService _proxy;
        Timer _timer;
        Dictionary<string, DateTime> _keyMaps;

        public SubscribeClient()
        {
            _keyMaps = new Dictionary<string, DateTime>();
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

            DuplexChannelFactory<ISubscribeService> channelFactory = new DuplexChannelFactory<ISubscribeService>(new InstanceContext(this), tcpBinding, endpointAddress);
            channelFactory.Open();
            _proxy = channelFactory.CreateChannel();
        }
        
        public void Subscribe(string requestKey)
        {
            _proxy.Subscribe(requestKey);
            _timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(30));

            if (! _keyMaps.ContainsKey(requestKey))
            {
                _keyMaps.Add(requestKey, DateTime.Now);
            }
            else
            {
                _keyMaps[requestKey] = DateTime.Now;
            }
        }

        public void UnSubscribe(string requestKey)
        {
            _proxy.UnSubscribe(requestKey);

            if (_keyMaps.ContainsKey(requestKey))
            {
                _keyMaps.Remove(requestKey);
            }
        }

        public virtual void Publish(Message message)
        {
            if (message == null || message.Body == null)
            {
                Console.WriteLine("no message");
                return;
            }

            var type = Type.GetType(message.BodyType);


            try
            {
                if (message.TypeName != null)
                {
                    var type2 = Type.GetType(message.TypeName);
                    //var obj2 = ChangeType(message, type2);
                    var obj3 = Activator.CreateInstance(type2, message.Body);
                    Handle((dynamic)obj3);
                }

            }
            catch (NotImplementedException notImplEx)
            { 
                
            }
            catch (Exception ex)
            {

            }
            

            Trace.WriteLine(string.Format("Key:{0} Id:{1} Msg:{2}", message.Header.MessageKey, message.Header.RequestKey, message.Body.ToString()));
        }

        static public object ChangeType(object value, Type type)
        {
            if (value == null && type.IsGenericType) return Activator.CreateInstance(type);
            if (value == null) return null;
            if (type == value.GetType()) return value;
            if (type.IsEnum)
            {
                if (value is string)
                    return Enum.Parse(type, value as string);
                else
                    return Enum.ToObject(type, value);
            }
            if (!type.IsInterface && type.IsGenericType)
            {
                Type innerType = type.GetGenericArguments()[0];
                
                return Activator.CreateInstance(type, new object[] { value });
            }
            if (value is string && type == typeof(Guid)) return new Guid(value as string);
            if (value is string && type == typeof(Version)) return new Version(value as string);
            if (!(value is IConvertible)) return value;
            return Convert.ChangeType(value, type);
        } 

        public void Handle(Message<Test1> message)
        {
            Trace.WriteLine("Message<Test1>:" + message.Body.ToString());
        }

        public void Handle(Message<Test2> message)
        {
            Trace.WriteLine("Message<Test2>:" + message.Body.ToString());
        }
        
        public void Publish<T>(Message<T> message) where T : ICommand
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
            catch (CommunicationException ex)
            {
                Trace.WriteLine("Ping Communication Exception:" + ex.Message);
                
                TryReconnect();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Ping Exception:" + ex.Message);

                TryReconnect();
            }

            return string.Empty;
        }

        private void TryReconnect()
        {
            try
            {
                MakeProxy(ServiceSetting.SubAddress, this);

                if (_keyMaps == null || _keyMaps.Count == 0)
                {
                    return;
                }

                foreach (var item in _keyMaps)
                {
                    this.Subscribe(item.Key);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Reconnect Exception:" + ex.Message);
            }
        }


        public string PublishSync(Message message)
        {
            var result = Guid.NewGuid().ToString("N") + "_" + DateTime.Now.ToString();
            Trace.WriteLine(result);
            Thread.Sleep(500);
            return result;
        }


        public ResponseMessage PublishSync(RequestMessage message)
        {
            var result = new ResponseMessage();

            result.Body = DateTime.Now.ToString();

            return result;
        }

    }
}
