using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Simple.ServiceBus.Messages;

namespace Simple.ServiceBus
{
    [ServiceContract]
    public interface IPublishService
    {
        [OperationContract(IsOneWay = false)]
        Message Publish(Message message);
    }
}
