
using Microsoft.Owin;
using Owin;
using System.Web.Http;
[assembly: OwinStartup(typeof(SelfHost_WebApi.Startup))]
namespace SelfHost_WebApi
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			//configura a web api para auto-hospedagem
			var config = new HttpConfiguration();

			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional }
		   );
			app.UseWebApi(config);
		}
	}
}