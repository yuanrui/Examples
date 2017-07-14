using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace Simple.ServiceBus.Common
{
    public static class OperationContextExtension
    {
        private static RemoteEndpointMessageProperty GetRemoteEndpointMessageProperty(OperationContext context)
        {
            MessageProperties properties = context.IncomingMessageProperties;

            if (! properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                return null;
            }

            RemoteEndpointMessageProperty endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;

            return endpoint;
        }

        private static bool IsNamedPipe(OperationContext context)
        {
            return string.Equals(context.Channel.LocalAddress.Uri.Scheme, "net.pipe", StringComparison.OrdinalIgnoreCase);
        }

        public static string GetClientAddress(this OperationContext context)
        {
            const string UNKNOWN = "unknown";

            if (context == null)
            {
                return UNKNOWN;
            }

            var clientInfo = GetClientInfo(context);
            
            return clientInfo != null ? clientInfo.ToString() : UNKNOWN;
        }

        public static string GetClientIP(this OperationContext context)
        {
            const string UNKNOWN = "unknown";

            if (context == null)
            {
                return UNKNOWN;
            }
            
            RemoteEndpointMessageProperty endpoint = GetRemoteEndpointMessageProperty(context);

            return endpoint == null ? UNKNOWN : endpoint.Address;
        }

        internal static ClientInfo GetClientInfo(this OperationContext context)
        {
            var clientInfo = context.IncomingMessageHeaders.GetHeader<ClientInfo>(ClientInfo.KEY, ClientInfo.HEADER_NAMESPACE);

            if (clientInfo != null && ! IsNamedPipe(context))
            {
                var endpoint = GetRemoteEndpointMessageProperty(context);
                clientInfo.IP = endpoint.Address;
                clientInfo.Port = endpoint.Port;
            }

            return clientInfo;
        }
    }
}
