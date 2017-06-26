using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Simple.ServiceBus.Common
{
    [ServiceContract]
    public interface IPublishService
    {
        [OperationContract(IsOneWay = false)]
        Message Publish(Message message);
    }
}
