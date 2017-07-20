using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Simple.ServiceBus.Messages;

namespace Simple.ServiceBus.Client
{
    public class Test1Handler : ICommandHandler<EmptyCommand, EmptyCommand>
        , ICommandHandler<Test1Command, Test1Command>, ICommandHandler<Test2Command, Test2ResultCommand>
    {

        public Message<EmptyCommand> Handle(Message<EmptyCommand> message)
        {
            return message;
        }

        public Message<Test1Command> Handle(Message<Test1Command> message)
        {
            Thread.Sleep(2000);
            Trace.WriteLine("Message<Test1>:" + message.Body.ToString());
            message.Body.Id = message.Body.Time.ToString("HH:mm:ss");
            message.Body.Time = DateTime.Now;
            return message;
        }

        public Message<Test2ResultCommand> Handle(Message<Test2Command> message)
        {
            Thread.Sleep(2000);
            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ">> Message<Test2Command>:" + message.Body.ToString());
            return new Message<Test2ResultCommand>(new Test2ResultCommand(), message.Header);
        }

    }
}
