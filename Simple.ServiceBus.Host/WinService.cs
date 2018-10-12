using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Simple.ServiceBus.Host
{
    partial class WinService : ServiceBase
    {
        ServerHost _host;

        public WinService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _host = new ServerHost();
            _host.Open();
        }

        protected override void OnStop()
        {
            if (_host == null)
            {
                return;
            }

            _host.Close();
        }
    }
}
