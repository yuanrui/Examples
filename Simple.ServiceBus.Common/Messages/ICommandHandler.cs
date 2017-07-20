using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.ServiceBus.Messages;

namespace Simple.ServiceBus.Messages
{
    public interface ICommandHandler { }

    public interface ICommandHandler<TIn, TOut> : ICommandHandler
        where TIn : class, ICommand
        where TOut : class, ICommand
    {
        Message<TOut> Handle(Message<TIn> message);
    }
}
