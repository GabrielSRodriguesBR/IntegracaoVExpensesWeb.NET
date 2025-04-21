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
    public class ConfiguracoesController : Controller
    {
        public ActionResult ModalConfiguracoes()
        {
            using (DBContext _db = new DBContext())
            {
                var configuracao = _db.Configuracoes.FirstOrDefault();
                if (configuracao == null)
                {
                    configuracao = new ConfiguracaoModel();
                }
                return PartialView("_ModalConfiguracoes", configuracao);
            }

        }

        [HttpPost]
        public JsonResult SalvarConfiguracao(ConfiguracaoModel model)
        {
            try
            {
                using (DBContext _db = new DBContext())
                {
                    var configuracao = _db.Configuracoes.FirstOrDefault();
                    if (configuracao == null)
                    {
                        _db.Configuracoes.Add(model);
                    }
                    else
                    {
                        configuracao.TransactionCode = model.TransactionCode;
                        configuracao.BPLID = model.BPLID;
                        configuracao.AccountCode = model.AccountCode;
                        configuracao.ProfitCode = model.ProfitCode;
                        configuracao.OcrCode3 = model.OcrCode3;
                    }

                    _db.SaveChanges();
                    return Json(new { status = true, text = "Configuração salva com sucesso!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, text = "Erro ao salvar a configuração: " + ex.Message });
            }
        }
    }
}
