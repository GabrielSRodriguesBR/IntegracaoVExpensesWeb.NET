using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace IntegracaoVExpensesWeb.Business.Utils
{
    public class VExpensesConfig
    {
        public string Endereco { get; set; }
        public string TokenAcesso { get; set; }

        public VExpensesEndPointConfg EndPoints { get; set; }

    }
    public class VExpensesEndPointConfg
    {
        public string Consulta { get; set; }
    }


    public class ConfigReader
    {
        public VExpensesConfig ReadVExpensesConfig()
        {
            var config = new VExpensesConfig();

            config.Endereco = ConfigurationManager.AppSettings["VExpensesConfig:Endereco"];
            config.TokenAcesso = ConfigurationManager.AppSettings["VExpensesConfig:TokenAcesso"];
            config.EndPoints = new VExpensesEndPointConfg
            {
                Consulta = ConfigurationManager.AppSettings["VExpensesConfig:EndPoints:Consulta"]
            };

            return config;
        }

        public SapConfig ReadSapConfig()
        {
            var config = new SapConfig();

            config.Server = ConfigurationManager.AppSettings["SapConfig:Server"];
            config.SLDServer = ConfigurationManager.AppSettings["SapConfig:SLDServer"];
            config.LicenseServer = ConfigurationManager.AppSettings["SapConfig:LicenseServer"];
            config.UseTrusted = bool.Parse(ConfigurationManager.AppSettings["SapConfig:UseTrusted"]);
            config.CompanyDB = ConfigurationManager.AppSettings["SapConfig:CompanyDB"];
            config.DbUserName = ConfigurationManager.AppSettings["SapConfig:DbUserName"];
            config.DbPassword = ConfigurationManager.AppSettings["SapConfig:DbPassword"];
            config.UserName = ConfigurationManager.AppSettings["SapConfig:UserName"];
            config.Password = ConfigurationManager.AppSettings["SapConfig:Password"];

            return config;
        }
    }


    public class SapConfig
    {
        public string Server { get; set; }
        public string SLDServer { get; set; }
        public string LicenseServer { get; set; }
        public bool UseTrusted { get; set; }
        public string CompanyDB { get; set; }
        public string DbUserName { get; set; }
        public string DbPassword { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}