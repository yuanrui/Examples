using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;

namespace Study.BigFiles
{
    partial class HttpWinService : ServiceBase
    {
        private HttpHostManager _hostManager;

        public HttpWinService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _hostManager = new HttpHostManager();
            _hostManager.Start();
        }

        protected override void OnStop()
        {
            if (_hostManager == null)
            {
                return;
            }

            _hostManager.Stop();
        }
    }
}
