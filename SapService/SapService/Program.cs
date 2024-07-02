using Microsoft.Owin.Hosting;
using SelfHost_WebApi;
using System;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;

namespace SapService
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                HandleArguments(args);
                return;
            }

            RunAsService();
        }

        private static void HandleArguments(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                StartServiceInteractively(args);
                return;
            }

            foreach (var arg in args)
            {
                try
                {
                    var param = arg.Contains("=") ? arg.Split('=')[0] : arg;
                    switch (param)
                    {
                        case "--install":
                            InstallService();
                            break;

                        case "--uninstall":
                            UninstallService();
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Erro ao processar o argumento '{arg}': {e}");
                }
            }
        }

        private static void InstallService()
        {
            try
            {
                ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro ao desinstalar o serviço: {e}");
            }

            try
            {
                ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro ao instalar o serviço: {e}");
            }
        }

        private static void UninstallService()
        {
            try
            {
                ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro ao desinstalar o serviço: {e}");
            }
        }

        private static void StartServiceInteractively(string[] args)
        {
            SapService service = new SapService();
            service.StartService(args);

            Console.WriteLine("Pressione Q para sair.");
            while (Console.ReadKey().KeyChar.ToString().ToUpper() != "Q") ;
            service.StopService();
        }

        private static void RunAsService()
        {
            ServiceBase[] ServicesToRun = new ServiceBase[] { new SapService() };
            ServiceBase.Run(ServicesToRun);
        }
    }

}
