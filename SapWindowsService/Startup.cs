
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
namespace SapWindowsService
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            // Configuração das rotas da Web API
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Configurar o servidor HTTP com a configuração da Web API
            HttpServer httpServer = new HttpServer(config);

            // Ativar o servidor HTTP no aplicativo OWIN
            app.UseWebApi(httpServer);
        }
    }
    }
}
