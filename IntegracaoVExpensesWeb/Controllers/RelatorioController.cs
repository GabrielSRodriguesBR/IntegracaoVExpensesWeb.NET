using IntegracaoVExpensesWeb.Business;
using IntegracaoVExpensesWeb.Business.DBContext;
using IntegracaoVExpensesWeb.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IntegracaoVExpensesWeb.Controllers
{
	public class RelatorioController : Controller
	{
		private DBContext db = new DBContext();

		/// <summary>
		/// Exibe a página principal dos relatórios
		/// </summary>
		/// <param name="filter">Filtro de pesquisa</param>
		/// <returns>View com os relatórios</returns>
		public ActionResult Index(RelatorioFilterViewModel filter)
		{
			var query = db.Relatorios.Include(s => s.Despesas).AsQueryable();

			if (filter.RelatorioId.HasValue)
			{
				query = query.Where(r => r.RelatorioId == filter.RelatorioId.Value);
			}

			if (filter.StatusPagamento != StatusPagamento.Todos)
			{
				if (filter.StatusPagamento == StatusPagamento.Pago)
				{
					query = query.Where(r => r.DataPagamento.HasValue);
				}
				else if (filter.StatusPagamento == StatusPagamento.NaoPago)
				{
					query = query.Where(r => !r.DataPagamento.HasValue);
				}
			}

			if (!string.IsNullOrEmpty(filter.DataInicio) && !string.IsNullOrEmpty(filter.DataFim))
			{
				DateTime dataInicio = DateTime.ParseExact(filter.DataInicio, "yyyy-MM", null);
				DateTime dataFim = DateTime.ParseExact(filter.DataFim, "yyyy-MM", null).AddMonths(1).AddDays(-1);
				query = query.Where(r => r.DataIntegracao >= dataInicio && r.DataIntegracao <= dataFim);
			}


			if (!string.IsNullOrEmpty(filter.Usuario))
			{
				query = query.Where(r => r.Usuario.Contains(filter.Usuario));
			}

			if (filter.StatusIntegracao != StatusIntegracao.Todos)
			{
				if (filter.StatusIntegracao == StatusIntegracao.Integrado)
				{
					query = query.Where(r => r.DocEntry.HasValue);
				}
				else if (filter.StatusIntegracao == StatusIntegracao.NaoIntegrado)
				{
					query = query.Where(r => !r.DocEntry.HasValue);
				}
			}
			filter.Usuarios = db.Relatorios.Select(r => r.Usuario).Distinct().OrderBy(s => s).ToList();
			filter.Relatorios = query.ToList();

			return View(filter);
		}

		/// <summary>
		/// Exibe os relatório de um determinado mês/ano
		/// </summary>
		/// <param name="mes">Mês de competência do relatório</param>
		/// <param name="ano">Ano de competência do relatório</param>
		/// <returns>View com resultados</returns>
		public ActionResult _DadosRelatorio(int mes, int ano)
		{
			var query = db.Relatorios.Include(s => s.Despesas).Where(s => s.DataIntegracao.Month == mes && s.DataIntegracao.Year == ano).ToList();
			return PartialView(query);
		}


		/// <summary>
		/// Faz a integração de pegamento com a api do vExpenses
		/// </summary>
		/// <param name="listaRelatorios">Relatórios que estão sendo pagos</param>
		/// <param name="dtPagamento">Data de pagamento</param>
		/// <returns>Json com o resultado da operação</returns>
		public async Task<ActionResult> IntegrarPagamentos(List<int> listaRelatorios, DateTime dtPagamento)
		{

			try
			{
				List<RelatorioModel> relatoriosPagar = db.Relatorios
					.Include(s => s.Despesas)
					.Where(s => s.DocEntry != null && s.DataPagamento == null && listaRelatorios.Any(a => a == s.RelatorioId))
					.ToList();

				if (relatoriosPagar.Count == 0)
					return Json(new { status = false, text = "Atenção", exception = "Nenhuma despesa pendente de pagamento, recarregue a página e tente novamente" });

				foreach (var relatorio in relatoriosPagar)
				{

					dynamic result = await new VExpensesAPI().SendPaymentAsync<dynamic>(relatorio.RelatorioId, dtPagamento);
					if (!((bool)(result.success ?? false)))
						return Json(new { status = false, text = $"Relatório:{relatorio.RelatorioId}", exception = String.Format("{0} </br> {1}", result.message?.ToString(), JsonConvert.SerializeObject(result?.data ?? null)) });

					relatorio.DataPagamento = dtPagamento;
				}
				db.SaveChanges();

				return Json(new { status = true, text = "Pagamentos informados com sucesso!" });
			}
			catch (Exception e)
			{
				return Json(new { status = false, text = "Erro interno ao realizar o pagamento", exception = e.ToString() });
			}

		}

		/// <summary>
		/// Faz a integração das despesas com o SAP
		/// </summary>
		/// <param name="listaRelatorios">Relatórios que estão sendo integrados</param>
		/// <returns>Json com o resultado da operação</returns>
		public async Task<ActionResult> IntegrarDespesasSAP(List<int> listaRelatorios)
		{
			try
			{

				List<RelatorioModel> relatoriosIntegrar = db.Relatorios
					.Include(s => s.Despesas)
					.Where(s => s.DocEntry == null && listaRelatorios.Any(a => a == s.RelatorioId))
					.ToList();

				if (relatoriosIntegrar.Count == 0)
					return Json(new { status = false, text = "Nenhuma despesa pendente de integração com o SAP, recarregue a página e tente novamente" });

				SapAPI sap = new SapAPI();
				var integracaoResult = await sap.IntegrarDespesas(listaRelatorios);

				return Json(new { integracaoResult.status, integracaoResult.text, integracaoResult.exception }, JsonRequestBehavior.AllowGet);

			}
			catch (Exception e)
			{
				return Json(new { status = false, text = "Erro interno ao realizar a integração", exception = e.ToString() }, JsonRequestBehavior.AllowGet);
			}


		}

		/// <summary>
		/// Exporta relatórios em excel
		/// </summary>
		/// <param name="listaRelatorios">Relatórios que estão sendo exportados</param>
		/// <returns>Arquivo .xlsx</returns>
		public ActionResult ExportarDespesas(List<int> listaRelatorios)
		{
			var despesas = db.Despesas
					.Include(s => s.Relatorio)
					.Where(s => s.Relatorio.DocEntry == null && listaRelatorios.Any(a => a == s.RelatorioId))
					.Select(s => new
					{
						s.RelatorioId,
						s.Relatorio.DataIntegracao,
						s.Relatorio.Descricao,
						Observarcao_Relatorio = s.Relatorio.Observacao,
						s.Relatorio.Usuario,
						s.DespesaId,
						s.Data,
						s.Titulo,
						s.Valor,
						Observacao_Despesa = s.Observacao,
						s.Tipo,
						s.TipoIdSAP,
						s.CentroCusto,
						s.CentroCustoIdSAP,
						s.URL
					})
					.ToList();

			string mesAno = String.Join("_", despesas.Select(s => $"{s.DataIntegracao.Month}_{s.DataIntegracao.Year}")
				.GroupBy(s => s)
				.Select(s => s.FirstOrDefault())
				.ToList());

			ExcelWriter excel = new ExcelWriter(fileName: $"RelatórioDespesas_{mesAno}");
			excel.Write(despesas, Response);

			return RedirectToAction("Index");

		}
	}
}