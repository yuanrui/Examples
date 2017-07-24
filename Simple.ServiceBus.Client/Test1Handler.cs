﻿using System;
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

        public EmptyCommand Handle(EmptyCommand message)
        {
            return message;
        }

        public Test1Command Handle(Test1Command message)
        {
            Thread.Sleep(2000);
            Trace.WriteLine("Message<Test1>:" + message.ToString());
            message.Id = message.Time.ToString("HH:mm:ss");
            message.Time = DateTime.Now;
            return message;
        }

        public Test2ResultCommand Handle(Test2Command message)
        {
            Thread.Sleep(2000);
            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ">> Message<Test2Command>:" + message.ToString());
            return new Test2ResultCommand();
        }

    }
}
