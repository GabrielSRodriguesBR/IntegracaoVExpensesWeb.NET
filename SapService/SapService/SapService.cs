﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using SelfHost_WebApi;

namespace SapService
{
    partial class SapService : ServiceBase
    {
        private IDisposable webApp;
        public SapService()
        {
            InitializeComponent();
        }

        public void StartService(string[] args)
        {
            if (Environment.UserInteractive)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Clear();
                Console.WriteLine("Iniciando vExpenses - Integração SAP...");
            }
            OnStart(args);

            if (Environment.UserInteractive)
            {
                Console.WriteLine("Press Q to quit");
                while (Console.ReadKey().KeyChar.ToString().ToUpper() != "Q") ;
                OnStop();
            }
        }

        public void StopService()
        {
            OnStop();
        }

        protected override void OnStart(string[] args)
        {
            string baseAddress = ConfigurationManager.AppSettings["ServiceAddress"];
            webApp = WebApp.Start<Startup>(url: baseAddress);
            if (Environment.UserInteractive)
            {
                Console.WriteLine("Serviço iniciado em: " + baseAddress);
            }
        }

        protected override void OnStop()
        {
            if (webApp != null)
            {
                webApp.Dispose();
            }

            if (Environment.UserInteractive)
            {
                Console.WriteLine("Serviço parado");
            }
        }
    }
}
