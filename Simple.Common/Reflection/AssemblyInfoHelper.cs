using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Simple.Common.Reflection
{
    public class AssemblyInfoHelper
    {
        Type _type;

        public AssemblyInfoHelper(Type type)
        {
            this._type = type;
            Assembly assembly = Assembly.GetAssembly(type);
            Init(assembly);
        }

        protected void Init(Assembly assembly)
        {
            this.Version = GetAttributeProperty<AssemblyVersionAttribute, String>(assembly, m => m.Version);
            this.FileVersion = GetAttributeProperty<AssemblyFileVersionAttribute, String>(assembly, m => m.Version) ?? this.Version;
            this.Company = GetAttributeProperty<AssemblyCompanyAttribute, String>(assembly, m => m.Company);
            this.Product = GetAttributeProperty<AssemblyProductAttribute, String>(assembly, m => m.Product);
            this.Title = GetAttributeProperty<AssemblyTitleAttribute, String>(assembly, m => m.Title);
            this.Copyright = GetAttributeProperty<AssemblyCopyrightAttribute, String>(assembly, m => m.Copyright);
            this.Description = GetAttributeProperty<AssemblyDescriptionAttribute, String>(assembly, m => m.Description);
        }

        protected TAttr GetAttribute<TAttr>(Assembly assembly) where TAttr : Attribute
        {
            var attrs = assembly.GetCustomAttributes(typeof(TAttr), false);
            if (attrs == null || attrs.Length == 0)
            {
                return default(TAttr);
            }

            return attrs[0] as TAttr;
        }

        protected TProperty GetAttributeProperty<TAttr, TProperty>(Assembly assembly, Func<TAttr, TProperty> selector) where TAttr : Attribute
        {
            var attr = GetAttribute<TAttr>(assembly);
            if (attr == null || attr == default(TAttr))
            {
                return default(TProperty);
            }

            return selector(attr);
        }

        public String Version
        {
            get;
            private set;
        }

        public String FileVersion
        {
            get;
            private set;
        }

        public String Company
        {
            get;
            private set;
        }

        public String Product
        {
            get;
            private set;
        }

        public String Title
        {
            get;
            private set;
        }

        public String Copyright
        {
            get;
            private set;
        }

        public String Description
        {
            get;
            private set;
        }
    }
}
