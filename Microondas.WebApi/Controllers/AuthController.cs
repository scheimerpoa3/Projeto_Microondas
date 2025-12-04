using Microondas.Infrastructure.Seguranca;
using Microondas.WebApi.Seguranca;
using System;
using System.Configuration;
using System.Web.Http;

namespace Microondas.WebApi.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private const string UsuarioPadrao = "admin";
        private const string SaltPadrao = "SALT_PADRAO_ADMIN";

        // Hash da senha "senha123" com salt "SALT_PADRAO_ADMIN"
        // (salt + senha -> hash base64)
        private const string HashSenhaPersistida = "+DKGne4MzGR+G/64tgpnYL65ZV075pTfVJUX1SKWfvA=";

        [HttpPost, Route("login")]
        public IHttpActionResult Login([FromBody] LoginRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Usuario) || string.IsNullOrWhiteSpace(req.Senha))
                return BadRequest("Usuário e senha são obrigatórios.");

            // Lê configurações do Web.config
            var usuarioConfig = ConfigurationManager.AppSettings["Auth.AdminUser"];
            var salt = ConfigurationManager.AppSettings["Auth.AdminSalt"];
            var hashConfig = ConfigurationManager.AppSettings["Auth.AdminPasswordHash"];

            if (string.IsNullOrWhiteSpace(usuarioConfig) ||
                string.IsNullOrWhiteSpace(salt) ||
                string.IsNullOrWhiteSpace(hashConfig))
            {
                // Se a configuração estiver errada, isso é erro interno do servidor
                return InternalServerError(new Exception("Configuração de autenticação inválida."));
            }

            // Confere usuário
            if (!string.Equals(req.Usuario, usuarioConfig, StringComparison.OrdinalIgnoreCase))
                return Unauthorized();

            // Gera o hash da senha informada com o mesmo salt
            var hashEntrada = HashHelper.CriarHashSha256(req.Senha, salt);

            // Compara o hash calculado com o hash armazenado (comparação exata)
            if (!string.Equals(hashEntrada, hashConfig, StringComparison.Ordinal))
                return Unauthorized();

            // 5) Usuário/senha válidos -> gera token "bearer" 
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
