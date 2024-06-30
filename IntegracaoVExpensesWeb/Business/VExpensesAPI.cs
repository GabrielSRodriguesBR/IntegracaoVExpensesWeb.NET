using HttpCurl;
using IntegracaoVExpensesWeb.Business.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace IntegracaoVExpensesWeb.Business
{

    public class VExpensesAPI
    {
        private readonly HttpClientCurl _httpClient;
        private readonly VExpensesConfig _apiConfig;

        public VExpensesAPI()
        {
            _httpClient = new HttpClientCurl();
            _apiConfig = new ConfigReader().ReadVExpensesConfig();
        }

        public async Task<T> GetApiDataAsync<T>()
        {
            string fullUrl = _apiConfig.Endereco.TrimEnd('/') + "/" + _apiConfig.EndPoints.Consulta.TrimStart('/');

            var resultApi = _httpClient.Get(fullUrl, new Dictionary<string, string>() { { "Authorization", _apiConfig.TokenAcesso } });

            if (!resultApi.status)
                throw new Exception($"{resultApi.text}: {resultApi.exception}");
            else
            {
                T result = JsonConvert.DeserializeObject<T>(resultApi.data);
                return result;
            }
        }
    }



    public class VExpensesModel
    {
        public string request { get; set; }
        public string method { get; set; }
        public bool success { get; set; }
        public int code { get; set; }
        public string message { get; set; }
        public Datum[] data { get; set; }
    }

    public class Datum
    {
        public int id { get; set; }
        public object external_id { get; set; }
        public int user_id { get; set; }
        public object device_id { get; set; }
        public string description { get; set; }
        public string status { get; set; }
        public object approval_stage_id { get; set; }
        public object approval_user_id { get; set; }
        public object approval_date { get; set; }
        public object payment_date { get; set; }
        public int? payment_method_id { get; set; }
        public string observation { get; set; }
        public int paying_company_id { get; set; }
        public bool on { get; set; }
        public object justification { get; set; }
        public string pdf_link { get; set; }
        public string excel_link { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public Expenses expenses { get; set; }
        public User user { get; set; }
    }

    public class Expenses
    {
        public Datum1[] data { get; set; }
    }

    public class Datum1
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int expense_id { get; set; }
        public object device_id { get; set; }
        public object integration_id { get; set; }
        public object external_id { get; set; }
        public string mileage { get; set; }
        public string date { get; set; }
        public int expense_type_id { get; set; }
        public int payment_method_id { get; set; }
        public int paying_company_id { get; set; }
        public object course_id { get; set; }
        public string reicept_url { get; set; }
        public float value { get; set; }
        public string title { get; set; }
        public string validate { get; set; }
        public bool reimbursable { get; set; }
        public string observation { get; set; }
        public int rejected { get; set; }
        public bool on { get; set; }
        public string mileage_value { get; set; }
        public string original_currency_iso { get; set; }
        public int? exchange_rate { get; set; }
        public float? converted_value { get; set; }
        public string converted_currency_iso { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public Expense_Type expense_type { get; set; }
        public Costs_Center costs_center { get; set; }
    }

    public class Expense_Type
    {
        public Data data { get; set; }
    }

    public class Data
    {
        public int id { get; set; }
        public string integration_id { get; set; }
        public string description { get; set; }
        public bool on { get; set; }
    }

    public class Costs_Center
    {
        public Data1 data { get; set; }
    }

    public class Data1
    {
        public int id { get; set; }
        public string integration_id { get; set; }
        public string name { get; set; }
        public int company_group_id { get; set; }
        public bool on { get; set; }
        public object approval_flow_id { get; set; }
    }

    public class User
    {
        public Data2 data { get; set; }
    }

    public class Data2
    {
        public int id { get; set; }
        public object integration_id { get; set; }
        public object external_id { get; set; }
        public int company_id { get; set; }
        public object role_id { get; set; }
        public int approval_flow_id { get; set; }
        public int expense_limit_policy_id { get; set; }
        public string user_type { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string cpf { get; set; }
        public string phone1 { get; set; }
        public object phone2 { get; set; }
        public string birth_date { get; set; }
        public string bank { get; set; }
        public string agency { get; set; }
        public string account { get; set; }
        public bool confirmed { get; set; }
        public bool active { get; set; }
        public object parameters { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
    }

}