using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Simple.ServiceBus.Messages
{
    public class TestCommand : ICommand
    {
        public string Input { get; set; }

        public override string ToString()
        {
            return Input;
        }
    }

    public class Test1Command : ICommand
    {
        public string Id { get; set; }

        public string Url { get; set; }

        public string Img { get; set; }

        public string Code { get; set; }

        public Int64 Index { get; set; }

        public DateTime Time { get; set; }

        public Test1Command()
        {
            Id = Guid.NewGuid().ToString();
            Url = Guid.NewGuid().ToString();
            Img = Guid.NewGuid().ToString();
            Code = string.Empty;
            var length = (Math.Abs(Guid.NewGuid().GetHashCode()) % 4000) + 4000;

            for (int i = 0; i < length; i++)
            {
                Code = string.Concat(Code, "好");
            }

            Time = DateTime.Now;
        }

        public override string ToString()
        {
            return Time.ToString("HH:mm:ss") + "=" + Id + " #" + Index + "@" + Code.Length;
        }
    }

    public class Test2Command : ICommand
    {
        public Int64 Id { get; set; }

        public DateTime Now { get; set; }

        public Test2Command()
        {
            Now = DateTime.Now;
            Id = Now.Ticks;
        }

        public override string ToString()
        {
            return Id + "@" + Now;
        }
    }

    public class Test2ResultCommand : ICommand
    {
        public string Key { get; set; }

        public Int64 Id { get; set; }

        public DateTime Now { get; set; }

        public Test2ResultCommand()
        {
            Key = Guid.NewGuid().ToString();
            Now = DateTime.Now;
            Id = Now.Ticks;
        }

        public override string ToString()
        {
            return Id + "@" + Now + "_" + Key;
        }
    }
}
