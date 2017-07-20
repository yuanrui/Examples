using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Simple.ServiceBus.Messages;

namespace Simple.ServiceBus.Client
{
    public class Test3Handler : ICommandHandler<Test3InCommand, Test3OutCommand>
    {
        public Message<Test3OutCommand> Handle(Message<Test3InCommand> message)
        {
            Thread.Sleep(2000);
            Trace.WriteLine(message.Body.Index.ToString().PadLeft(3, '0') + ":" + message.Body.ToString());

            var result = new Test3OutCommand();
            for (int i = 0; i < 10; i++)
            {
                result.List.Add(new Test3OutCommand.Test3OutWapper() { Id = message.Body.Id, Message = message.Body.Remark, Success = true });
            }

            return new Message<Test3OutCommand>(result, message.Header);
        }
    }
}
