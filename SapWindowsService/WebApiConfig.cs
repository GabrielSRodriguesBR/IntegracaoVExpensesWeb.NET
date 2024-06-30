using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace SapWindowsService
{
    public partial class WebServiceHost : ServiceBase
    {
        private IDisposable _webApp;

        public WebServiceHost()
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
