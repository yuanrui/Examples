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

        public static string GetClientAddress(this OperationContext context)
        {
            const string UNKNOWN = "unknown";

            if (context == null)
            {
                return UNKNOWN;
            }

            if (string.Equals(context.Channel.LocalAddress.Uri.Scheme, "net.pipe", StringComparison.OrdinalIgnoreCase))
            {
                var seesionId = context.SessionId;
                if (seesionId != null && seesionId.Contains(";"))
                {
                    return "ClientId:" + seesionId.Split(';')[1].Replace("id=", string.Empty);
                }

                return seesionId;
            }

            RemoteEndpointMessageProperty endpoint = GetRemoteEndpointMessageProperty(context);
            
            return endpoint == null ? UNKNOWN : endpoint.Address + ":" + endpoint.Port.ToString();
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
    }
}
