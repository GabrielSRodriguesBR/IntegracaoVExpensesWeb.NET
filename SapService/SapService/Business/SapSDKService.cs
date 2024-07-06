using SAPbobsCOM;
using SapService.Business.Utils;
using System;

namespace SapService.Business
{

	public class SapSDKService
	{
		private Company _company;
		private readonly SapConfig _sapConfig;

		public SapSDKService()
		{
			_company = new Company();
			_sapConfig = new ConfigReader().ReadSapConfig();
		}

		public void Connect()
		{
			_company.Server = _sapConfig.Server;
			_company.CompanyDB = _sapConfig.CompanyDB;
			_company.UserName = _sapConfig.UserName;
			_company.Password = _sapConfig.Password;
			_company.DbServerType = BoDataServerTypes.dst_MSSQL2014;
			_company.DbUserName = _sapConfig.DbUserName;
			_company.DbPassword = CriptoPass.CriptoPass.DescriptografarSenha(_sapConfig.DbPassword);
			_company.UserName = _sapConfig.UserName;
			_company.Password = CriptoPass.CriptoPass.DescriptografarSenha(_sapConfig.Password);
			_company.language = BoSuppLangs.ln_Portuguese_Br;
			_company.UseTrusted = _sapConfig.UseTrusted;
			_company.LicenseServer = _sapConfig.LicenseServer;
			_company.SLDServer = _sapConfig.SLDServer;

			if (_company.Connect() != 0)
			{
				throw new Exception("Não foi possível conectar ao SAP: " + _company.GetLastErrorDescription());
			}
		}


		public void Disconnect()
		{
			if (_company != null && _company.Connected)
			{
				_company.Disconnect();
			}
		}

		public string HandleError()
		{
			int lErrCode;
			string sErrMsg;
			_company.GetLastError(out lErrCode, out sErrMsg);
			return $"{lErrCode} - {sErrMsg}";
		}

		public int GetDocEntry()
		{
			string docEntry = "";
			_company.GetNewObjectCode(out docEntry);
			int parsedDocEntry = 0;
			Int32.TryParse(docEntry, out parsedDocEntry);
			return parsedDocEntry;
		}

		public Company GetCompany()
		{
			return _company;
		}


	}




}
