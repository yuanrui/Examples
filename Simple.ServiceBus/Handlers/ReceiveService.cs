using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.ServiceBus.Core;

namespace Simple.ServiceBus.Handlers
{
    public class ReceiveService : ServiceBase
    {
        protected override void RegisterServiceHandler(ServiceRegister registerManager)
        {
            registerManager.Register("ReceiveService.DoReceive", (object parameter, ref object result) => 
            {
                result = DoReceive(parameter as string);
            });
        }

        public string DoReceive(string input)
        {
            Console.WriteLine(input);
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ">>" + input;
        }
    }
}
