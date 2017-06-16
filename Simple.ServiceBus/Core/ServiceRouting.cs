using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.ServiceBus.Core
{
    public class ServiceRouting
    {
        public static ServiceRouting GlobalRouting = new ServiceRouting();

        public ServiceHandler GetHandler(string requestKey)
        {
            return ServiceRegister.GlobalRegister.GetHandler(requestKey);
        }
    }
}
