using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.ServiceBus.Common.Impl
{
    class DefaultCommand
    {
    }

    

    public class EmptyCommand : ICommand
    {

    }

    public class ExceptionCommand : IFailCommand, ICommand
    {
        public virtual object OriginalObject { get; set; }

        public virtual string ExceptionMessage { get; set; }

        public virtual string RequestKey { get; set; }
    }

    public class NotImplExceptionCommand : ExceptionCommand, IFailCommand, ICommand
    {
        public string TypeName { get; set; }
    }

    public class CommExceptionCommand : ExceptionCommand, IFailCommand, ICommand
    {
    }

    public class NotSubscriberCommand : IFailCommand, ICommand
    {
        public virtual string RequestKey { get; set; }
    }
}
