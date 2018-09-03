using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Thrift.Configuration
{
    /// <summary>
    /// <![CDATA[
    ///<thrift.hosts defaultHost="default">
    ///    <host name="default" port="20188" minThreadPoolSize="10" maxThreadPoolSize="100" clientTimeout="0" useBufferedSockets="false">
    ///        <service contract="Thrift.Demo.Shared.AddressRpc, Thrift.Demo.Shared" handler="Thrift.Demo.Hosting.AddressRpcImpl, Thrift.Demo.Hosting" />
    ///        <service contract="Banana.RPC.SmsSendShortMessageRpc, Thrift.Demo.Shared" handler="Thrift.Demo.Hosting.SmsSendShortMessageRpcImpl, Thrift.Demo.Hosting" />
    ///    </host>
    ///    <host name="test" port="22221" minThreadPoolSize="10" maxThreadPoolSize="100" clientTimeout="0" useBufferedSockets="false">
    ///        <service contract="Thrift.Demo.Shared.AddressRpc, Thrift.Demo.Shared" handler="Thrift.Demo.Hosting.AddressRpcImpl, Thrift.Demo.Hosting" />
    ///        <service contract="Banana.RPC.SmsSendShortMessageRpc, Thrift.Demo.Shared" handler="Thrift.Demo.Hosting.SmsSendShortMessageRpcImpl, Thrift.Demo.Hosting" />
    ///    </host>
    ///</thrift.hosts>
    /// ]]>
    /// </summary>
    public class HostSetion : ConfigurationSection
    {
        [ConfigurationProperty("defaultHost")]
        public String DefaultHost
        {
            get { return (String)this["defaultHost"]; }
            set { this["defaultHost"] = value; }
        }

        [ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public HostElements Hosts
        {
            get { return (HostElements)base[""]; }
        }
    }

    public class HostElements : ConfigurationElementCollection, IEnumerable<HostElement>
    {
        protected override String ElementName
        {
            get
            {
                return "host";
            }
        }

        public new HostElement this[String name]
        {
            get
            {
                return BaseGet(name) as HostElement;
            }
        }

        public HostElement this[Int32 index]
        {
            get
            {
                return BaseGet(index) as HostElement;
            }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        public new IEnumerator<HostElement> GetEnumerator()
        {
            for (Int32 i = 0; i < base.Count; i++)
            {
                yield return (HostElement)base.BaseGet(i);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new HostElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as HostElement).Name;
        }
    }

    public class HostElement : ConfigurationElement
    {
        [ConfigurationProperty("name")]
        public String Name
        {
            get { return (String)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("port")]
        public Int32 Port
        {
            get { return (Int32)this["port"]; }
            set { this["port"] = value; }
        }

        [ConfigurationProperty("minThreadPoolSize")]
        public Int32 MinThreadPoolSize
        {
            get { return (Int32)this["minThreadPoolSize"]; }
            set { this["minThreadPoolSize"] = value; }
        }

        [ConfigurationProperty("maxThreadPoolSize")]
        public Int32 MaxThreadPoolSize
        {
            get { return (Int32)this["maxThreadPoolSize"]; }
            set { this["maxThreadPoolSize"] = value; }
        }

        [ConfigurationProperty("clientTimeout")]
        public Int32 ClientTimeout
        {
            get { return (Int32)this["clientTimeout"]; }
            set { this["clientTimeout"] = value; }
        }

        [ConfigurationProperty("useBufferedSockets")]
        public Boolean UseBufferedSockets
        {
            get { return (Boolean)this["useBufferedSockets"]; }
            set { this["useBufferedSockets"] = value; }
        }

        [ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public ServiceElements Services
        {
            get { return (ServiceElements)this[""]; }
            set { this[""] = value; }
        }
    }

    public class ServiceElements : ConfigurationElementCollection, IEnumerable<ServiceElement>
    {
        protected override String ElementName
        {
            get
            {
                return "service";
            }
        }

        public new ServiceElement this[String name]
        {
            get
            {
                return BaseGet(name) as ServiceElement;
            }
            set
            {
                if (BaseGet(name) != null)
                {
                    BaseRemove(name);
                }

                BaseAdd(value);
            }
        }

        public ServiceElement this[Int32 index]
        {
            get
            {
                return BaseGet(index) as ServiceElement;
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

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ServiceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as ServiceElement).Contract;
        }

        public new IEnumerator<ServiceElement> GetEnumerator()
        {
            for (Int32 i = 0; i < base.Count; i++)
            {
                yield return (ServiceElement)base.BaseGet(i);
            }
        }
    }

    public class ServiceElement : ConfigurationElement
    {
        [ConfigurationProperty("contract", IsRequired = true)]
        public String Contract
        {
            get { return (String)this["contract"]; }
            set { this["contract"] = value; }
        }

        [ConfigurationProperty("handler", IsRequired = true)]
        public String Handler
        {
            get { return (String)this["handler"]; }
            set { this["handler"] = value; }
        }

        public String ContractTypeName
        {
            get
            {
                return (GetSplitArray(this.Contract)[0] ?? String.Empty).Trim();
            }
        }

        public String ContractAssemblyName
        {
            get
            {
                return (GetSplitArray(this.Contract)[1] ?? String.Empty).Trim();
            }
        }

        public String HandlerTypeName
        {
            get
            {
                return (GetSplitArray(this.Handler)[0] ?? String.Empty).Trim();
            }
        }

        public String HandlerAssemblyName
        {
            get
            {
                return (GetSplitArray(this.Handler)[1] ?? String.Empty).Trim();
            }
        }

        protected String[] GetSplitArray(String value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return new String[2];
            }

            return value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
