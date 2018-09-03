using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Thrift.Configuration
{
    /// <summary>
    /// <![CDATA[
    ///<thrift.clients host="localhost" port="20188">
    ///    <client service="Thrift.Demo.Shared.AddressRpc" />
    ///    <client service="Banana.RPC.SmsSendShortMessageRpc" />
    ///</thrift.clients>
    /// ]]>
    /// </summary>
    public class ClientSection : ConfigurationSection
    {
        [ConfigurationProperty("host")]
        public String Host
        {
            get { return (String)this["host"]; }
            set { this["host"] = value; }
        }

        [ConfigurationProperty("port")]
        public Int32 Port
        {
            get { return (Int32)this["port"]; }
            set { this["port"] = value; }
        }
        
        [ConfigurationProperty("timeout")]
        public Int32 Timeout
        {
            get { return (Int32)this["timeout"]; }
            set { this["timeout"] = value; }
        }

        [ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public ClientElements Clients
        {
            get { return (ClientElements)base[""]; }
        }

        public String GetHost(String serviceName)
        {
            var client = Clients[serviceName];

            if (client == null || string.IsNullOrEmpty(client.Host))
            {
                return this.Host;
            }

            return client.Host;
        }

        public Int32 GetPort(String serviceName)
        {
            var client = Clients[serviceName];

            if (client == null || client.Port == 0)
            {
                return this.Port;
            }

            return client.Port;
        }

        public Int32 GetTimeout(String serviceName)
        {
            var client = Clients[serviceName];

            if (client == null || client.Timeout == 0)
            {
                return this.Timeout;
            }

            return client.Timeout;
        }
    }

    public class ClientElements : ConfigurationElementCollection, IEnumerable<ClientElement>
    {
        protected override String ElementName
        {
            get { return "client"; }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        public ClientElement this[int index]
        {
            get
            {
                return (ClientElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public new ClientElement this[String name]
        {
            get
            {
                return (ClientElement)base.BaseGet(name);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ClientElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ClientElement)element).ServiceName;
        }

        public new IEnumerator<ClientElement> GetEnumerator()
        {
            for (Int32 i = 0; i < base.Count; i++)
            {
                yield return (ClientElement)base.BaseGet(i);
            }
        }
    }

    public class ClientElement : ConfigurationElement
    {
        [ConfigurationProperty("service", IsRequired = true)]
        public String ServiceName
        {
            get { return this["service"].ToString(); }
            set { this["service"] = value; }
        }

        [ConfigurationProperty("host", IsRequired = false)]
        public String Host
        {
            get { return this["host"].ToString(); }
            set { this["host"] = value; }
        }

        [ConfigurationProperty("port", IsRequired = false)]
        public Int32 Port
        {
            get { return (Int32)this["port"]; }
            set { this["port"] = value; }
        }

        [ConfigurationProperty("timeout", IsRequired = false)]
        public Int32 Timeout
        {
            get { return (Int32)this["timeout"]; }
            set { this["timeout"] = value; }
        }

        public override String ToString()
        {
            return this.ServiceName;
        }
    }
}
