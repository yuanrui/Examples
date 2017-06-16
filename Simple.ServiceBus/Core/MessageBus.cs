using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.ServiceBus.Core
{
    public class MessageBus
    {
        public static TResult Send<TResult>(Message parameter)
        {
            var handler = ServiceRouting.GlobalRouting.GetHandler(parameter.Header.RequestKey);

            if (handler == null)
            {
                return default(TResult);
            }

            object result = null;

            handler(parameter.Body, ref result);

            if (result == null)
            {
                return default(TResult);
            }

            return (TResult)result;
        }
    }
}
