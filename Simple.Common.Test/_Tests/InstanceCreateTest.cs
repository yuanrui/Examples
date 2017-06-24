using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.Common.Utility;

namespace Simple.Common.Test._Tests
{
    public static class InstanceCreateTest
    {
        private class MessageTest<T>
        {
            public Object Body { get; set; }

            public MessageHeaderTest Header { get; set; }

            public string TypeName { get; set; }

            public MessageTest()
            {
                TypeName = this.GetType().FullName;
            }

            public MessageTest(T data)
                : this(data, null)
            {

            }

            public MessageTest(T data, MessageHeaderTest header)
            {
                Body = data;
                Header = header;
                TypeName = this.GetType().FullName;
            } 
        }

        private class CmdTest 
        {
            
        }

        private class MessageHeaderTest 
        {
            public string Key { get; set; }

            public string RequestKey { get; set; }

            public string ResponseKey { get; set; }
        }

        public static void Run()
        {
            CodeTimer.Initialize();
            var type = typeof(MessageTest<CmdTest>);
            var ctor = type.GetConstructors()[2];
            ObjectActivator<object> createdActivator = ActivatorHelper.GetActivator<object>(ctor);
            var input = new CmdTest();
            var header = new MessageHeaderTest();
            object tmpObj = null;
            var count = 1000000;
            CodeTimer.Time("new instance", count, () =>
            {
                tmpObj = new MessageTest<CmdTest>(input, header);
            });

            CodeTimer.Time("exp tree", count, () =>
            {
                tmpObj = createdActivator(input, header);
            });

            CodeTimer.Time("Activator.CreateInstance", count, () =>
            {
                tmpObj = Activator.CreateInstance(type, input, header);
            });

            CodeTimer.Time("exp tree2", count, () =>
            {
                tmpObj = ActivatorHelper.CreateInstance(type, input, header);               
            });
        }
    }
}
