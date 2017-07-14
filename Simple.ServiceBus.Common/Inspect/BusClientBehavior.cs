using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using ChannelMessage = System.ServiceModel.Channels.Message;
using ChannelMessageHeader = System.ServiceModel.Channels.MessageHeader;

namespace Simple.ServiceBus.Common.Inspect
{
    public class BusClientBehavior : IClientMessageInspector, IDispatchMessageInspector, IEndpointBehavior
    {
        #region IClientMessageInspector

        public void AfterReceiveReply(ref ChannelMessage reply, object correlationState)
        {
            
        }

        public object BeforeSendRequest(ref ChannelMessage request, IClientChannel channel)
        {
            var pro = Process.GetCurrentProcess();

            var clientInfoHeader = ChannelMessageHeader.CreateHeader(ClientInfo.KEY, ClientInfo.HEADER_NAMESPACE, ClientInfo.GetCurrent());
            request.Headers.Add(clientInfoHeader);

            return null;
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {

        }

        #endregion

        #region IDispatchMessageInspector

        public object AfterReceiveRequest(ref ChannelMessage request, IClientChannel channel, InstanceContext instanceContext)
        {
            return null;
        }

        public void BeforeSendReply(ref ChannelMessage reply, object correlationState)
        {

        }

        #endregion

        #region IEndpointBehavior

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new BusClientBehavior());
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new BusClientBehavior());
        }

        public void Validate(ServiceEndpoint endpoint)
        {

        }

        #endregion
    }
}
