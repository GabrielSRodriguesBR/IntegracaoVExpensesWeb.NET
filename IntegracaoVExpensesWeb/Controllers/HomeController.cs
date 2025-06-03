using System.Web.Mvc;
using IntegracaoVExpensesWeb.Business.Utils;

namespace IntegracaoVExpensesWeb.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
            return View();
		}

	}
}