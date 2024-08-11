using System.Configuration;

namespace IntegracaoVExpensesWeb.Business.Utils
{
	public class VExpensesConfig
    {
        public string Endereco { get; set; }
        public string TokenAcesso { get; set; }

        public VExpensesEndPointConfg EndPoints { get; set; }
		public string Pagamento { get; set; }

	}
    public class VExpensesEndPointConfg
    {
        public string Consulta { get; set; }
        public string Pagamento { get; set; }
    }


    public class ConfigReader
    {
		/// <summary>
		/// Converte as configurações do web.config em um objeto para ser usado no sistema
		/// </summary>
		/// <returns></returns>
		public VExpensesConfig ReadVExpensesConfig()
        {
            var config = new VExpensesConfig();

            config.Endereco = ConfigurationManager.AppSettings["VExpensesConfig:Endereco"];
            config.TokenAcesso = ConfigurationManager.AppSettings["VExpensesConfig:TokenAcesso"];
            config.EndPoints = new VExpensesEndPointConfg
            {
                Consulta = ConfigurationManager.AppSettings["VExpensesConfig:EndPoints:Consulta"],
                Pagamento = ConfigurationManager.AppSettings["VExpensesConfig:EndPoints:Pagamento"]
            };

            return config;
        }

    }


}