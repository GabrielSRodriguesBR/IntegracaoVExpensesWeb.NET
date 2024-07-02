
using SeasideResearch.LibCurlNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SeasideResearch.LibCurlNet.Easy;



namespace HttpCurl
{
    public class HttpClientCurl
    {
        // Função de callback para processar os dados recebidos
        private static int OnWriteData(byte[] buf, int size, int nmemb, object extraData)
        {
            // Exemplo simples de concatenação dos dados recebidos
            StringBuilder data = (StringBuilder)extraData;
            data.Append(Encoding.UTF8.GetString(buf));
            return size * nmemb;
        }

        public (bool status, string text, string exception, string data) Get(string url, Dictionary<string, string> headers)
        {
            Curl.GlobalInit((int)CURLinitFlag.CURL_GLOBAL_ALL); 
            using (Easy easy = new Easy())
            {
                // Configura a URL da requisição
                easy.SetOpt(CURLoption.CURLOPT_URL, url);
                // Configura os cabeçalhos HTTP da requisição
                Slist httpHheaders = new Slist();
                httpHheaders.Append("Accept: application/json");
                httpHheaders.Append("User-Agent: Mozilla/5.0");
                httpHheaders.Append("Authorization: saSgIZa3MY07Yu5ePKIFf8Z3p41AO6Vv75VW7RXoP9Bq8eAHX638qbnnRFj4");
                easy.SetOpt(CURLoption.CURLOPT_HTTPHEADER, httpHheaders);
                easy.SetOpt(CURLoption.CURLOPT_SSL_VERIFYPEER, false);

                // Configura a função de callback para processar os dados recebidos
                easy.SetOpt(CURLoption.CURLOPT_WRITEFUNCTION, new WriteFunction(OnWriteData));

                StringBuilder data = new StringBuilder();
                easy.SetOpt(CURLoption.CURLOPT_WRITEDATA, data);

                // Executa a requisição
                CURLcode res = easy.Perform();

                // Verifica se a requisição foi bem sucedida
                if (res != CURLcode.CURLE_OK)
                {
                    return (false, "Erro na requisição", res.ToString(), "");
                }
                else
                {
                    return (true, "", res.ToString(), data.ToString());

                }
            }

        }

		public (bool status, string text, string exception, string data) Put(string url, string payload, Dictionary<string, string> headers)
		{
			Curl.GlobalInit((int)CURLinitFlag.CURL_GLOBAL_ALL);
			using (Easy easy = new Easy())
			{
				// Configura a URL da requisição
				easy.SetOpt(CURLoption.CURLOPT_URL, url);
				// Configura o método PUT
				easy.SetOpt(CURLoption.CURLOPT_CUSTOMREQUEST, "PUT");
				// Configura os cabeçalhos HTTP da requisição
				Slist httpHeaders = new Slist();
				foreach (var header in headers)
				{
					httpHeaders.Append($"{header.Key}: {header.Value}");
				}
				httpHeaders.Append("Content-Type: application/json");
				easy.SetOpt(CURLoption.CURLOPT_HTTPHEADER, httpHeaders);
				easy.SetOpt(CURLoption.CURLOPT_SSL_VERIFYPEER, false);
				// Configura o payload da requisição
				easy.SetOpt(CURLoption.CURLOPT_POSTFIELDS, payload);

				// Configura a função de callback para processar os dados recebidos
				easy.SetOpt(CURLoption.CURLOPT_WRITEFUNCTION, new WriteFunction(OnWriteData));

				StringBuilder data = new StringBuilder();
				easy.SetOpt(CURLoption.CURLOPT_WRITEDATA, data);

				// Executa a requisição
				CURLcode res = easy.Perform();

				if (res != CURLcode.CURLE_OK)
				{
					return (false, "Erro na requisição", res.ToString(), "");
				}
				else
				{
					return (true, "", res.ToString(), data.ToString());

				}
			}
		}

	}
}
