using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SapWindowsService
{
    public partial class Service1 : ServiceBase
    {
        private IDisposable _webApp;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            string baseAddress = "http://localhost:9000/"; // URL do serviço
            _webApp = WebApp.Start<Startup>(baseAddress);
        }

        protected override void OnStop()
        {
            _webApp?.Dispose();
        }
    }
}
