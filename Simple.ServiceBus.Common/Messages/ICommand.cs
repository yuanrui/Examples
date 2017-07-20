using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.ServiceBus.Messages
{
    public interface ICommand
    {
    }

    public interface IFailCommand : ICommand
    {
        string RequestKey { get; set; }
    }
}
