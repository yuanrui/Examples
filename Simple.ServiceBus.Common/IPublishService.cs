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
        [OperationContract(IsOneWay = true)]
        void Publish(Message message);

        [OperationContract(IsOneWay = false)]
        Message PublishSync(Message message);
    }
}
