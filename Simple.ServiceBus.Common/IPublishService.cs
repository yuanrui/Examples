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
        ResponseMessage PublishSync(RequestMessage message);
    }
    
    public interface IBusCommond<TIn> where TIn : ICommand
    {
        void Handle(Message<TIn> message);
    }

    public interface IBusCommond<TIn, TOut> : IBusCommond<TIn> where TIn : ICommand where TOut : ICommand
    {
        new Message<TOut> Handle(Message<TIn> message);
    }
}
