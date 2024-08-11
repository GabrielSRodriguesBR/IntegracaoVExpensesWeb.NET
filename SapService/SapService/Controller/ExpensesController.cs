using IntegracaoVExpensesWeb.Business.DBContext;
using IntegracaoVExpensesWeb.Models;
using Newtonsoft.Json;
using SapService.Business.Integracao;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;

namespace SapService.Controller
{
	public class ExpensesController : ApiController
	{
		[HttpGet]
		public string TestarIntegracaoSAP()
		{
			using (SAPExpenses Integracao = new SAPExpenses())
			{
				ExpenseModel expense = new ExpenseModel();
				expense.memo = $"Teste de integração vExpenses: {DateTime.Now.ToString("dd/MM/yyyy")}";
				expense.RelatorioID = 99;
				expense.ref1 = "99";
				expense.ref2 = "Teste Integração";
				expense.creditMemo = $"V.EXPENSES (99, Teste Integração)";
				expense.Itens = new List<ExpenseItemModel>()
					{
						new ExpenseItemModel()
						{
							accountCode = "4.01.01.06.09" ,
							amount = 0.01,
							lineMemo = "Teste de Integração",
							dueDate = DateTime.Now,
							taxDate = DateTime.Now,
							profitCode = "102",
							ocrCode2 = "207",
							ocrCode3 = "F999999"
						},
						new ExpenseItemModel()
						{
							accountCode = "4.01.01.06.09" ,
							amount = 0.01,
							lineMemo = "Teste de Integração",
							dueDate = DateTime.Now,
							taxDate = DateTime.Now,
							profitCode = "102",
							ocrCode2 = "207",
							ocrCode3 = "F999999"
						}
					};



				var result = Integracao.Integrar(expense);
				return JsonConvert.SerializeObject(result);

			}
		}

		/// <summary>
		/// Integra as despesas no SAP
		/// </summary>
		/// <param name="listaRelatorios">Lista de relatórios que vão ser integrados</param>
		/// <returns></returns>
		[HttpPost]
		public (bool status, string text, string exception) Add(List<int> listaRelatorios)
		{
			try
			{
				using (DBContext _db = new DBContext())
				{
					List<RelatorioModel> relatoriosIntegrar = _db.Relatorios
					.Include(s => s.Despesas)
					.Where(s => s.DocEntry == null && listaRelatorios.Any(a => a == s.RelatorioId))
					.ToList()
					.Where(s => s.Despesas.Any()) //SAP não deixa integrar relatórios sem itens
					.ToList(); 

					if (relatoriosIntegrar.Count == 0)
						return (false, "Nenhuma despesa pendente de integração com o SAP, recarregue a página e tente novamente", "");

					using (SAPExpenses Integracao = new SAPExpenses())
					{
						foreach (var relatorio in relatoriosIntegrar)
						{
							ExpenseModel expense = new ExpenseModel();
							expense.memo = relatorio.Descricao;
							expense.RelatorioID = relatorio.RelatorioId;
							expense.ref1 = relatorio.RelatorioId.ToString();
							expense.ref2 = relatorio.Usuario;
							string nomeUsuario = (relatorio.Usuario.Length <= 20) ? relatorio.Usuario : relatorio.Usuario.Substring(0, 20);
							expense.creditMemo = $"V.EXPENSES ({relatorio.RelatorioId.ToString()}, {nomeUsuario})";
							expense.Itens = relatorio.Despesas
								.Select(s => new ExpenseItemModel()
								{
									accountCode = s.TipoIdSAP,
									amount = Convert.ToDouble(s.Valor),
									lineMemo = s.Titulo,
									dueDate = s.Data,
									taxDate = s.Data,
									profitCode = "102",
									ocrCode2 = s.CentroCustoIdSAP,
									ocrCode3 = "F999999"
								}).ToList();

							var result = Integracao.Integrar(expense);
							if (result.status)
								relatorio.DocEntry = result.docEntry;
							else
							{
								_db.SaveChanges();
								return (result.status, result.text, result.exception);
							}

						}
						_db.SaveChanges();
					}

					return (true, "Despesas integradas com sucesso!", "");
				}
			}
			catch (Exception e)
			{
				return (false, "Erro interno ao realizar a integração", e.ToString());
			}
		}

	}
}
