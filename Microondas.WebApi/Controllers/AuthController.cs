using System;
using System.Web.Http;
using Microondas.WebApi.Seguranca;
using Microondas.Infrastructure.Seguranca;

namespace Microondas.WebApi.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private const string UsuarioPadrao = "admin";
        private const string SaltPadrao = "SALT_PADRAO_ADMIN";

        // Hash da senha "senha123" com salt "SALT_PADRAO_ADMIN" usando SHA256
        // (salt + senha -> hash base64)
        private const string HashSenhaPersistida = "+DKGne4MzGR+G/64tgpnYL65ZV075pTfVJUX1SKWfvA=";

        [HttpPost, Route("login")]

        public IHttpActionResult Login([FromBody] LoginRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Usuario) || string.IsNullOrWhiteSpace(req.Senha))
                return BadRequest("Usuário e senha são obrigatórios.");

            var hashInformado = HashHelper.CriarHashSha256(req.Senha, SaltPadrao);

            if (req.Usuario != UsuarioPadrao || hashInformado != HashSenhaPersistida)
                return Unauthorized();

            // Gera um token "bearer" simples e guarda na memória
            var token = TokenStore.GerarToken(req.Usuario);

            return Ok(new
            {
                token,
                tipo = "Bearer",
                usuario = req.Usuario
            });
        }

        [HttpPost, Route("logout")]
        public IHttpActionResult Logout()
        {
            var authHeader = Request.Headers.Authorization;
            if (authHeader != null && authHeader.Scheme == "Bearer")
            {
                var token = authHeader.Parameter;
                TokenStore.RevogarToken(token);
            }

            return Ok(new { Mensagem = "Logout realizado (se token existia)." });
        }
    }

    public class LoginRequest
    {
        public string Usuario { get; set; }
        public string Senha { get; set; }
    }
}
