using IntegracaoVExpensesWeb.Business.DBContext;
using IntegracaoVExpensesWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace IntegracaoVExpensesWeb.Controllers
{
    public class RelatorioController : Controller
    {
        private DBContext db = new DBContext();
        // GET: Relatorio
   

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
    }
}