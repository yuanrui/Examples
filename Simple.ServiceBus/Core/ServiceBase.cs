using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.ServiceBus.Core
{
    public abstract class ServiceBase
    {
        public void Register()
        {
            this.RegisterServiceHandler(ServiceRegister.GlobalRegister);
        }

        protected abstract void RegisterServiceHandler(ServiceRegister registerManager);
    }
}
