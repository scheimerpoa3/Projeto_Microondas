using System.Web.Http;
using Microsoft.Owin;
using Microondas.WebApi;
using Owin;

[assembly: OwinStartup(typeof(Microondas.WebApi.Startup))]

namespace Microondas.WebApi
{
    public class Startup
    {
        // Este método é chamado pelo OWIN ao iniciar a aplicação.
        public void Configuration(IAppBuilder app)
        {
            // Cria uma instância de configuração do Web API
            var config = new HttpConfiguration();

            // Registra rotas e filtros (chama o seu WebApiConfig.Register)
            WebApiConfig.Register(config);

            // Opcional: configurações adicionais
            // config.Formatters.JsonFormatter.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;

            // Habilita o Web API via OWIN
            app.UseWebApi(config);
        }
    }
}
