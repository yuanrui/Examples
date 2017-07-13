using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;

namespace Simple.ServiceBus.Common.Impl
{
    public class SubscribeClient : ISubscribeService, IPublishService
        , ICommandHandler<EmptyCommand, EmptyCommand>
        , ICommandHandler<Test1Command, Test1Command>, ICommandHandler<Test2Command, Test2ResultCommand>
    {
        ISubscribeService _proxy;
        Timer _timer;
        ConcurrentDictionary<string, DateTime> _keyMaps;
        
        public SubscribeClient()
        {
            _keyMaps = new ConcurrentDictionary<string, DateTime>();
            MakeProxy(NetSetting.SubAddress, this);
            _timer = new Timer(DoPing, null, Timeout.Infinite, 5000);
        }

        public void MakeProxy(string endpoindAddress, object callbackinstance)
        {
            var binding = NetSetting.GetBinding();
            
            EndpointAddress endpointAddress = new EndpointAddress(endpoindAddress);
            InstanceContext context = new InstanceContext(callbackinstance);

            DuplexChannelFactory<ISubscribeService> channelFactory = new DuplexChannelFactory<ISubscribeService>(new InstanceContext(this), binding, endpointAddress);
            channelFactory.Open();
            _proxy = channelFactory.CreateChannel();
        }
        
        public void Subscribe(string requestKey)
        {
            var waitTime = TimeSpan.FromSeconds(30);
            try
            {
                _proxy.Subscribe(requestKey);
            }
            catch (EndpointNotFoundException enfEx)
            {
                Trace.WriteLine(enfEx.Message);
                Trace.Write("wait:" + waitTime.TotalSeconds + "s" + Environment.NewLine);
                Thread.Sleep(waitTime);
            }

            _timer.Change(TimeSpan.Zero, waitTime);

            _keyMaps.AddOrUpdate(requestKey, DateTime.Now, (m, n) => DateTime.Now);
        }

        public void UnSubscribe(string requestKey)
        {
            _proxy.UnSubscribe(requestKey);

            DateTime time = DateTime.MinValue;
            _keyMaps.TryRemove(requestKey, out time);
        }

        public Message Publish(Message message)
        {
            var result = new Message();

            try
            {
                if (message.TypeName != null)
                {
                    var type = Type.GetType(message.TypeName);
                    var instance = Activator.CreateInstance(type, message.Body, message.Header);

                    var handleResult = Handle((dynamic)instance);

                    if (handleResult != null)
                    {
                        result.Header = handleResult.Header;
                        result.Body = handleResult.Body;
                    }
                }
            }
            catch (NotImplementedException notImplEx)
            {
                var cmd = new Impl.NotImplExceptionCommand();
                cmd.TypeName = message.TypeName;
                cmd.OriginalObject = message.Body;
                cmd.ExceptionMessage = notImplEx.Message;
                result.Body = cmd;
            }
            catch (Exception ex)
            {
                var cmd = new Impl.ExceptionCommand();
                cmd.OriginalObject = message.Body;
                cmd.ExceptionMessage = ex.Message;
                result.Body = cmd;
            }
            
            return result;
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
                MakeProxy(NetSetting.SubAddress, this);

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

        public Message<EmptyCommand> Handle(Message<EmptyCommand> message)
        {
            return message;
        }

        public Message<Test1Command> Handle(Message<Test1Command> message)
        {
            Thread.Sleep(2000);
            Trace.WriteLine("Message<Test1>:" + message.Body.ToString());
            message.Body.Id = message.Body.Time.ToString("HH:mm:ss");
            message.Body.Time = DateTime.Now;            
            return message;
        }

        public Message<Test2ResultCommand> Handle(Message<Test2Command> message)
        {
            Thread.Sleep(2000);
            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ">> Message<Test2Command>:" + message.Body.ToString());
            return new Message<Test2ResultCommand>(new Test2ResultCommand(), message.Header);
        }
    }
}
