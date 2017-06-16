using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Simple.ServiceBus.Common
{
    public class ServiceSetting
    {
        public static string PubAddress 
        {
            get 
            {
                return ConfigurationManager.AppSettings["PubEndpointAddress"];
            }
        }

        public static string SubAddress
        {
            get 
            {
                return ConfigurationManager.AppSettings["SubEndpointAddress"];
            }
        }
    }
}
