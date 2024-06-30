using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SapWindowsService
{
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        private ServiceProcessInstaller serviceProcessInstaller;
        private ServiceInstaller serviceInstaller;

        public ProjectInstaller()
        {
            serviceProcessInstaller = new ServiceProcessInstaller();
            serviceInstaller = new ServiceInstaller();

            // Defina as propriedades do instalador de processo do serviço
            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;

            // Defina as propriedades do instalador do serviço
            serviceInstaller.ServiceName = "vExpenses - Sap Integração 2";
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            // Adicione os instaladores ao conjunto de instaladores
            Installers.Add(serviceProcessInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
