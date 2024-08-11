using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IntegracaoVExpensesWeb.Business
{
	public class SapAPI
	{
		private readonly HttpClient _httpClient;
		public SapAPI()
		{
			_httpClient = new HttpClient();
			_httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["ServiceAddress"]);	
		}

		/// <summary>
		/// Integra as despesas no SAP usando o SapService
		/// </summary>
		/// <param name="listaRelatorios">Lista de relatórios que vão ser integrados</param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public async Task<(bool status, string text, string exception)> IntegrarDespesas(List<int> listaRelatorios)
		{

			var json = JsonConvert.SerializeObject(listaRelatorios);
			var content = new StringContent(json, Encoding.UTF8, "application/json");

			try
			{
				var response = await _httpClient.PostAsync("/api/Expenses/Add", content);

				// Verifica se a resposta é bem-sucedida (código de status 2xx)
				response.EnsureSuccessStatusCode();

				string data = await response.Content.ReadAsStringAsync();

				if (string.IsNullOrEmpty(data))
				{
					throw new Exception("Resposta vazia recebida da API SAP.");
				}

				var result = JsonConvert.DeserializeObject<(bool status, string text, string exception)>(data);

				return result;
			}
			catch (HttpRequestException ex)
			{
				throw new Exception("Erro HTTP ao chamar a API SAP", ex);
			}
			catch (JsonSerializationException ex)
			{
				throw new Exception("Erro ao desserializar a resposta da API SAP", ex);
			}
			catch (Exception ex)
			{
				throw new Exception("Erro ao chamar a API SAP", ex);
			}
		}
	}
}