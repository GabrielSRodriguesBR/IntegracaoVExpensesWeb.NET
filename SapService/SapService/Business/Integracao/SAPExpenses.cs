using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SapService.Business.Integracao
{
	public class SAPExpenses : IDisposable
	{
		private SapSDKService _sap;
		private bool _disposed = false;

		public SAPExpenses()
		{
			_sap = new SapSDKService();
			_sap.Connect(); //chama a conexão com o SAP ao instanciar o objeto
		}
		/// <summary>
		/// Integra uma despesa no SAP
		/// </summary>
		/// <param name="expense"></param>
		/// <returns></returns>
		public (bool status, string text, string exception, int docEntry) Integrar(ExpenseModel expense)
		{

			try
			{
				DateTime ultimaDiaMes = GetUltimoDiaDoMesAtual();

				#region capa

				JournalEntries oJournalEntry = (JournalEntries)_sap.GetCompany().GetBusinessObject(BoObjectTypes.oJournalEntries);
				oJournalEntry.ReferenceDate = ultimaDiaMes;
				oJournalEntry.Memo = expense.memo;
				oJournalEntry.TransactionCode = "RDV";
				oJournalEntry.TaxDate = ultimaDiaMes;
				oJournalEntry.DueDate = ultimaDiaMes;
				oJournalEntry.Series = 17;
				oJournalEntry.Reference = expense.ref1;
				oJournalEntry.Reference2 = expense.ref2;

				#endregion

				#region linhas (débito)
				foreach (var line in expense.Itens)
				{
					oJournalEntry.Lines.AccountCode = line.accountCode;
					oJournalEntry.Lines.Debit = line.amount;
					oJournalEntry.Lines.DueDate = line.dueDate;
					oJournalEntry.Lines.LineMemo = line.lineMemo;
					oJournalEntry.Lines.TaxDate = line.taxDate;
					oJournalEntry.Lines.BPLID = 1;
					oJournalEntry.Lines.CostingCode = line.profitCode;
					oJournalEntry.Lines.CostingCode2 = line.ocrCode2;
					oJournalEntry.Lines.CostingCode3 = line.ocrCode3;
					oJournalEntry.Lines.Add();
				}
				#endregion

				#region última linha (crédito)
				oJournalEntry.Lines.AccountCode = "1.01.03.03.35";
				oJournalEntry.Lines.Credit = expense.ammountTotal;
				oJournalEntry.Lines.DueDate = ultimaDiaMes;
				oJournalEntry.Lines.LineMemo = expense.creditMemo;
				oJournalEntry.Lines.TaxDate = ultimaDiaMes;
				oJournalEntry.Lines.BPLID = 1;
				oJournalEntry.Lines.Add();

				#endregion

				int lRetCode = oJournalEntry.Add();
				if (lRetCode != 0)
				{
					string error = _sap.HandleError();
					return (false, $"Não foi possível integrar o Relatório ID: {expense.RelatorioID} ao SAP", error, 0);
				}
				else
				{
					return (true, "Sucesso", "", _sap.GetDocEntry());
				}


			}
			catch (Exception e)
			{
				return (false, $"Ocorreu um erro ao integrar Relatório ID: {expense.RelatorioID}", e.ToString(), 0);
			}
		}

		/// <summary>
		/// Obtém o último dia do mês atual
		/// </summary>
		/// <returns></returns>
		private DateTime GetUltimoDiaDoMesAtual()
		{
			DateTime dataAtual = DateTime.Now;
			DateTime primeiroDiaDoMesSeguinte = new DateTime(dataAtual.Year, dataAtual.Month, 1).AddMonths(1);
			DateTime ultimoDiaDoMes = primeiroDiaDoMesSeguinte.AddDays(-1);
			return ultimoDiaDoMes;
		}


		#region Dispose

		//O código abaixo garante que o objeto SAPExpenses faça a deconexão com o SAP e seja eliminado no fim de sua vida útil
		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					if (_sap != null)
					{
						_sap.Disconnect();
						_sap = null;
					}
				}

				_disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~SAPExpenses()
		{
			Dispose(false);
		}

		#endregion

	}

	public class ExpenseModel
	{
		public int RelatorioID { get; set; }
		public string ref1 { get; set; }
		public string ref2 { get; set; }
		public string memo { get; set; }
		public string creditMemo { get; set; }

		public List<ExpenseItemModel> Itens { get; set; }

		public double ammountTotal
		{
			get
			{
				return Itens.Sum(s => s.amount);
			}
		}
	}

	public class ExpenseItemModel
	{
		public double amount { get; set; }
		public DateTime dueDate { get; set; }
		public DateTime taxDate { get; set; }
		public string accountCode { get; set; }
		public string profitCode { get; set; }
		public string ocrCode2 { get; set; }
		public string ocrCode3 { get; set; }
		public string lineMemo { get; set; }
	}
}
