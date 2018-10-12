using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace Simple.ServiceBus.Host
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            var displayName = this.Context.Parameters["DN"].ToString();
            var serviceName = this.Context.Parameters["SN"].ToString();
            if (!String.IsNullOrEmpty(displayName))
            {
                this.serviceInstaller.DisplayName = displayName;
            }
            if (!String.IsNullOrEmpty(serviceName))
            {
                this.serviceInstaller.ServiceName = serviceName;
            }

            base.Install(stateSaver);
        }

        public override void Uninstall(IDictionary savedState)
        {
            var displayName = this.Context.Parameters["DN"].ToString();
            var serviceName = this.Context.Parameters["SN"].ToString();
            if (!String.IsNullOrEmpty(displayName))
            {
                this.serviceInstaller.DisplayName = displayName;
            }
            if (!String.IsNullOrEmpty(serviceName))
            {
                this.serviceInstaller.ServiceName = serviceName;
            }

            base.Uninstall(savedState);
        }
    }
}
