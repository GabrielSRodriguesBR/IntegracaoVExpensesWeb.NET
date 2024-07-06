using IntegracaoVExpensesWeb.Business;
using IntegracaoVExpensesWeb.Business.DBContext;
using IntegracaoVExpensesWeb.Business.Utils;
using IntegracaoVExpensesWeb.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IntegracaoVExpensesWeb.Controllers
{
	public class ConsultaController : Controller
    {
        private DBContext db = new DBContext();

		/// <summary>
		/// Renderiza a modal que faz e exibe os resultados da api do vExpenses
		/// </summary>
		/// <returns>View com os resultados</returns>
		public ActionResult ModalBuscarDadosApi()
        {
            return PartialView("_ModalBuscarDadosApi");
        }

        /// <summary>
        /// Faz a chamada da api do vExpenses e exibe em tela
        /// </summary>
        /// <returns>View com os resultados</returns>
        public async Task<ActionResult> ChamarAPI()
        {
            VExpensesModel resultadoAPI = await new VExpensesAPI().GetApiDataAsync<VExpensesModel>();
            ViewBag.resultadosAPI = JsonConvert.SerializeObject(resultadoAPI);
            return PartialView("_DadosAPI", resultadoAPI);
        }

       /// <summary>
       /// Salva os resultados da api do vExpenses na base de dados
       /// </summary>
       /// <param name="resultadosAPI">Objeto vExpenses</param>
       /// <returns>JSON de resultado da operação</returns>
        public ActionResult SalvarResultadosApi(VExpensesModel resultadosAPI)
        {
            if (resultadosAPI == null)
            {
                return Json(new { status = false, text = "Dados não recebidos" });
            }

            if (resultadosAPI.data.Length == 0)
                return Json(new { status = false, text = "O resultado não possuí dados para serem salvos" });

            DateTime hoje = DateTime.Now;

            HashSet<int> relatoriosExistententes = db.Relatorios.Select(s => s.RelatorioId).ToHashSet();
            HashSet<int> despesasExistententes = db.Despesas.Select(s => s.DespesaId).ToHashSet();

            List<RelatorioModel> relatoriosNovos = resultadosAPI.data
                .Where(s => !relatoriosExistententes.Any(RelatorioId => RelatorioId == s.id)) //filtra todos os relatórios que não existem no banco
                .Select(s => new RelatorioModel()
                {
                    RelatorioId = s.id,
                    DataIntegracao = hoje,
                    Descricao = s.description,
                    Observacao = s.observation,
                    TipoUsuario = s.user.data.user_type,
                    Usuario = s.user.data.name,
                    UsuarioIdSAP = s.user.data.id.ToString(),
                })
                .ToList();


            List<DespesaModel> despesasNovas = resultadosAPI.data
                 .Where(s => s.expenses != null)
                 .SelectMany(s => s.expenses.data)
                 .Where(s => !despesasExistententes.Any(DespesaId => DespesaId == s.id)) //filtra todas os despesas que não existem no banco
                 .Select(s => new DespesaModel()
                 {
                     DespesaId = s.id,
                     RelatorioId = s.expense_id,
                     Data = s.date.ToDateTime("yyyy-MM-dd HH:mm:ss"),
                     Titulo = s.title,
                     Valor = Convert.ToDecimal(s.value),
                     Observacao = s.observation,
                     Tipo = s.expense_type.data.description,
                     TipoIdSAP = s.expense_type.data.integration_id,
                     CentroCusto = s.costs_center.data.name,
                     CentroCustoIdSAP = s.costs_center.data.integration_id,
                     URL = s.reicept_url

                 }).ToList();

            int totalRelatoriosInseridos = relatoriosNovos.Count,
                totalDespesasInseridos = despesasNovas.Count;

            db.Relatorios.AddRange(relatoriosNovos);
            db.Despesas.AddRange(despesasNovas);

            db.SaveChanges();

            string text = $"Foram adicionados <br>{totalRelatoriosInseridos} Relatórios <br> {totalDespesasInseridos} Despesas <br> com sucesso!";
            if (totalDespesasInseridos == 0 && totalRelatoriosInseridos == 0)
                text = "Esses resultados já foram salvos na base de dados.";


            return Json(new { status = true, text = text }, JsonRequestBehavior.AllowGet);
        }

    }
}