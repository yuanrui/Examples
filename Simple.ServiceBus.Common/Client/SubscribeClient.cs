using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading;
using Simple.ServiceBus.Configuration;
using Simple.ServiceBus.Inspect;
using Simple.ServiceBus.Messages;

namespace Simple.ServiceBus.Client
{
    public class SubscribeClient : ISubscribeService, IPublishService
    {
        Dictionary<Type, ICommandHandler> _handlers;
        ISubscribeService _proxy;
        Timer _timer;
        TimeSpan _defaultWaitTime;
        ConcurrentDictionary<string, DateTime> _keyMaps;
        
        public SubscribeClient()
        {
            _handlers = new Dictionary<Type, ICommandHandler>();
            _keyMaps = new ConcurrentDictionary<string, DateTime>();
            MakeProxy(NetSetting.SubAddress, this);
            _defaultWaitTime = TimeSpan.FromSeconds(15);
            _timer = new Timer(DoPing, null, Timeout.Infinite, 5000);
        }

        public void MakeProxy(string endpoindAddress, object callbackinstance)
        {
            var binding = NetSetting.GetBinding();
            
            EndpointAddress endpointAddress = new EndpointAddress(endpoindAddress);
            InstanceContext context = new InstanceContext(callbackinstance);

            DuplexChannelFactory<ISubscribeService> channelFactory = new DuplexChannelFactory<ISubscribeService>(new InstanceContext(this), binding, endpointAddress);
            channelFactory.Endpoint.Behaviors.Add(new BusClientBehavior()); 
            channelFactory.Open();
            _proxy = channelFactory.CreateChannel();
        }
        
        public void Subscribe(string requestKey)
        {
            try
            {
                _proxy.Subscribe(requestKey);
                _timer.Change(TimeSpan.Zero, _defaultWaitTime);
            }
            catch (EndpointNotFoundException enfEx)
            {
                Trace.WriteLine(enfEx.Message);
                Trace.Write("wait:" + _defaultWaitTime.TotalSeconds + "s");
                _timer.Change(_defaultWaitTime, _defaultWaitTime);
            }            

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
            _timer.Change(_defaultWaitTime, _defaultWaitTime);
            ICommandHandler handler = null;
            var result = new Message();
            var header = new MessageHeader();
            header.RequestKey = message.Header.ResponseKey;
            header.ResponseKey = message.Header.ResponseKey;
            header.RouteType = message.Header.RouteType;
            header.RunCount = message.Header.RunCount;
            result.Header = header;

            try
            {
                if (message.TypeName != null)
                {
                    if (this._handlers.TryGetValue(message.Body.GetType(), out handler))
                    {
                        var handleResult = ((dynamic)handler).Handle((dynamic)message.Body);
                        
                        result.Body = handleResult;
                    }
                                        
                    if (handler == null)
                    {
                        var cmd = new NotImplExceptionCommand();
                        cmd.TypeName = message.BodyType;
                        cmd.OriginalObject = message.Body;
                        cmd.ExceptionMessage = "no provider";
                        result.Body = cmd;
                    }
                }
            }
            catch (NotImplementedException notImplEx)
            {
                var cmd = new NotImplExceptionCommand();
                cmd.TypeName = message.BodyType;
                cmd.OriginalObject = message.Body;
                cmd.ExceptionMessage = notImplEx.Message;
                result.Body = cmd;
            }
            catch (Exception ex)
            {
                var cmd = new FailCommand();
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
                Trace.Write(result);
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

        public void Register(ICommandHandler commandHandler)
        {
            var genericHandler = typeof(ICommandHandler<,>);
            var supportedCommandTypes = commandHandler.GetType()
                .GetInterfaces()
                .Where(iface => iface.IsGenericType && iface.GetGenericTypeDefinition() == genericHandler)
                .Select(iface => iface.GetGenericArguments()[0])
                .ToList();

            if (_handlers.Keys.Any(registeredType => supportedCommandTypes.Contains(registeredType)))
                throw new ArgumentException("The command handled by the received handler already has a registered handler.");

            foreach (var commandType in supportedCommandTypes)
            {
                this._handlers.Add(commandType, commandHandler);
            }
        }
    }
}
