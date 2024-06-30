using IntegracaoVExpensesWeb.Business.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace IntegracaoVExpensesWeb.Controllers
{
    public class HomeController : Controller
    {
        private DBContext db = new DBContext();
        public ActionResult Index()
        {

            return View();
        }

        //[HttpGet]
        //public JsonResult TestarConexaoSAP()
        //{
        //    try
        //    {
        //        new SapService().Connect();
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new { status = false, exception = e.ToString() }, JsonRequestBehavior.AllowGet);
        //    }


        //    return Json(new { status = true }, JsonRequestBehavior.AllowGet);

        //}


    }
}