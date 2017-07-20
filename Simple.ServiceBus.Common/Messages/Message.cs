using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Simple.ServiceBus.Messages
{
    [DataContract, Serializable]
    [KnownType("GetKnownTypes")]
    public class Message 
    {
        private Object _body;

        [DataMember]
        public virtual MessageHeader Header { get; set; }

        [DataMember]
        public virtual Object Body
        {
            get { return _body; }
            set
            {
                if (value != null)
                {
                    BodyType = value.GetType().FullName;
                }

                _body = value;
            }
        }

        [DataMember]
        public virtual string BodyType { get; set; }

        [DataMember]
        public virtual string TypeName { get; set; }

        [IgnoreDataMember]
        public virtual bool IsSuccess
        {
            get
            {
                return !(Body is IFailCommand);
            }
        }

        [DataMember]
        public Message Next { get; protected set; }

        public void SetNext(Message next)
        {
            if (Next == null)
            {
                Next = next;

                return;
            }

            Next.SetNext(next);
        }

        protected static Type[] GetKnownTypes()
        {
            return MessageTypeHelper.GetCommandTypes();
        } 
    }

    public class Message<T> where T : class, ICommand
    {
        public virtual MessageHeader Header { get; set; }

        public virtual T Body { get; set; }
        
        public virtual string BodyType { get; set; }

        public virtual string TypeName { get; set; }

        public Message() : this(null, null)
        {
            
        }

        public Message(T data) : this(data, null)
        {

        }

        public Message(T data, MessageHeader header)
        {
            Body = data;
            Header = header;

            BodyType = typeof(T).FullName;
            TypeName = this.GetType().FullName;
        }
        
        public Message ToMessage()
        {
            var msg = new Message();
            msg.Header = this.Header;
            msg.Body = this.Body;
            msg.TypeName = this.TypeName;

            return msg;
        }
    }

    internal class MessageTypeHelper
    {
        static Type[] _types;
        
        public static Type[] GetCommandTypes()
        {
            if (_types == null)
            {
                List<Assembly> allAssemblies = new List<Assembly>();
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                foreach (string dll in Directory.GetFiles(path, "*.dll"))
                    allAssemblies.Add(Assembly.LoadFile(dll));

                var result = new List<Type>();

                Type thisType = typeof(ICommand);

                allAssemblies.ForEach(m =>
                {
                    result.AddRange(m.GetTypes().Where(t => t.IsSubclassOf(thisType) || t.GetInterface(thisType.Name) == thisType));
                });

                _types = result.ToArray();
            }

            return _types;
        }
    }
}
