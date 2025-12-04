using System;
using System.Collections.Concurrent;

namespace Microondas.WebApi.Seguranca
{
    public static class TokenStore
    {
        // token -> usuário
        private static readonly ConcurrentDictionary<string, string> _tokens =
            new ConcurrentDictionary<string, string>();

        public static string GerarToken(string usuario)
        {
            var token = Guid.NewGuid().ToString("N"); // token simples
            _tokens[token] = usuario;
            return token;
        }

        public static bool ValidarToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            return _tokens.ContainsKey(token);
        }

        public static string ObterUsuarioPorToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            _tokens.TryGetValue(token, out var usuario);
            return usuario;
        }

        public static void RevogarToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return;

            _tokens.TryRemove(token, out _);
        }
    }
}
