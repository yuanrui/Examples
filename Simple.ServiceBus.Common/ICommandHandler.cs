using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.ServiceBus.Common
{
    public interface ICommandHandler<TIn, TOut>
        where TIn : class, ICommand
        where TOut : class, ICommand
    {
        Message<TOut> Handle(Message<TIn> message);
    }
}
