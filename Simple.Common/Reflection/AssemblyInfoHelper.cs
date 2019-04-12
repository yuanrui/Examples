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
            AssemblyFileVersionAttribute fileVersionAttr = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)[0] as AssemblyFileVersionAttribute;
            if (fileVersionAttr != null)
            {
                this.FileVersion = fileVersionAttr.Version;
            }

            AssemblyCompanyAttribute companyAttr = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false)[0] as AssemblyCompanyAttribute;
            if (companyAttr != null)
            {
                this.Company = companyAttr.Company;
            }

            AssemblyProductAttribute productAttr = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false)[0] as AssemblyProductAttribute;
            if (productAttr != null)
            {
                this.Product = productAttr.Product;
            }

            AssemblyTitleAttribute titleAttr = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false)[0] as AssemblyTitleAttribute;
            if (titleAttr != null)
            {
                this.Title = titleAttr.Title;
            }

            AssemblyCopyrightAttribute copyrightAttr = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0] as AssemblyCopyrightAttribute;
            if (companyAttr != null)
            {
                this.Company = companyAttr.Company;
            }

            AssemblyDescriptionAttribute descriptionAttr = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)[0] as AssemblyDescriptionAttribute;
            if (descriptionAttr != null)
            {
                this.Description = descriptionAttr.Description;
            }
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
