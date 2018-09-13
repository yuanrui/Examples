using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Study.BigFiles
{
    /// <summary>
    /// <![CDATA[
    /// <bigfile.hosts>
    ///    <host path="BigFile.data" port="33119" size="10GB" />
    ///    <host path="33120.data" port="33120" size="5GB" />
    /// </bigfile.hosts>
    /// ]]>
    /// </summary>
    public class HostConfig : ConfigurationSection
    {
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
            get { return "host"; }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        public HostElement this[int index]
        {
            get
            {
                return (HostElement)BaseGet(index);
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

        public new HostElement this[String name]
        {
            get
            {
                return (HostElement)base.BaseGet(name);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new HostElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((HostElement)element).FilePath;
        }

        public new IEnumerator<HostElement> GetEnumerator()
        {
            for (Int32 i = 0; i < base.Count; i++)
            {
                yield return (HostElement)base.BaseGet(i);
            }
        }
    }

    public class HostElement : ConfigurationElement
    {
        [ConfigurationProperty("path", IsRequired = true)]
        public String FilePath
        {
            get { return this["path"].ToString(); }
            set { this["path"] = value; }
        }

        [ConfigurationProperty("size", IsRequired = true)]
        public String Size
        {
            get { return this["size"].ToString(); }
            set { this["size"] = value; }
        }

        public Int64 FileSize
        {
            get
            {
                return GetFileSize(this.Size);
            }
        }

        [ConfigurationProperty("port", IsRequired = true)]
        public Int32 Port
        {
            get { return (Int32)this["port"]; }
            set { this["port"] = value; }
        }

        public override String ToString()
        {
            return this.FilePath;
        }

        private Int64 GetFileSize(String size)
        {
            const String TB_SUFFIX = "TB";
            const String GB_SUFFIX = "GB";
            const String MB_SUFFIX = "MB";
            const String KB_SUFFIX = "KB";
            const Int64 TB_SIZE = 1099511627776L;
            const Int64 GB_SIZE = 1073741824L;
            const Int64 MB_SIZE = 1048576L;
            const Int64 KB_SIZE = 1024L;

            if (String.IsNullOrEmpty(size))
            {
                return 0L;
            }

            String originalText = size.ToUpper().Replace(TB_SUFFIX, String.Empty)
                .Replace(GB_SUFFIX, String.Empty)
                .Replace(MB_SUFFIX, String.Empty)
                .Replace(KB_SUFFIX, String.Empty);

            Decimal original = 0;
            Decimal.TryParse(originalText, out original);

            if (size.EndsWith(TB_SUFFIX, StringComparison.OrdinalIgnoreCase))
            {
                return (Int64)(original * TB_SIZE);
            }

            if (size.EndsWith(GB_SUFFIX, StringComparison.OrdinalIgnoreCase))
            {
                return (Int64)(original * GB_SIZE);
            }

            if (size.EndsWith(MB_SUFFIX, StringComparison.OrdinalIgnoreCase))
            {
                return (Int64)(original * MB_SIZE);
            }

            if (size.EndsWith(KB_SUFFIX, StringComparison.OrdinalIgnoreCase))
            {
                return (Int64)(original * KB_SIZE);
            }

            return (Int64)original;
        }
    }
}
