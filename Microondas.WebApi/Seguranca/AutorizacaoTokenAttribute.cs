using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microondas.WebApi.Seguranca
{
    public class AutorizacaoTokenAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            // Lê o cabeçalho Authorization: Bearer <token>
            var authHeader = actionContext.Request.Headers.Authorization;

            if (authHeader == null || authHeader.Scheme != "Bearer")
                return false;

            var token = authHeader.Parameter;

            if (!TokenStore.ValidarToken(token))
                return false;

            // Se quiser, pode colocar o usuário no contexto
            var usuario = TokenStore.ObterUsuarioPorToken(token);
            if (usuario != null)
            {
                // Exemplo: adicionar em Properties para uso nos controllers
                actionContext.Request.Properties["UsuarioAutenticado"] = usuario;
            }

            return true;
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(
                HttpStatusCode.Unauthorized,
                new
                {
                    Erro = "Não autorizado",
                    Mensagem = "Token inválido ou não informado."
                });
        }
    }
}
