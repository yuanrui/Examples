using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Simple.ServiceBus.Messages
{
    class DefaultCommand
    {
    }

    public class EmptyCommand : ICommand
    {

    }

    public class FailCommand : IFailCommand, ICommand
    {
        public virtual object OriginalObject { get; set; }

        public virtual string ExceptionMessage { get; set; }

        public virtual string RequestKey { get; set; }

        public virtual Hashtable Extend { get; protected set; }

        public FailCommand()
        {
            Extend = new Hashtable();
        }

        public void SetValue(string key, object value)
        {
            Extend[key] = value;
        }

        public object GetValue(string key)
        {
            return Extend[key];
        }

        public virtual void SetInheritType()
        {
            SetValue("InheritType", this.GetType());
        }
    }

    public class NotImplExceptionCommand : FailCommand, IFailCommand, ICommand
    {
        public virtual string TypeName
        {
            get
            {
                return GetValue("RequestKey") as string;
            }
            set
            {
                SetValue("RequestKey", value);
            }
        }

        public NotImplExceptionCommand()
        {
            SetInheritType();
        }
    }

    public class CommExceptionCommand : FailCommand, IFailCommand, ICommand
    {
        public CommExceptionCommand()
        {
            SetInheritType();
        }
    }

    public class NotSubscriberCommand : FailCommand, IFailCommand, ICommand
    {
        public NotSubscriberCommand()
        {
            SetInheritType();
        }
    }
}
