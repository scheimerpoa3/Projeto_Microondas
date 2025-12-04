using System;
using System.Security.Cryptography;
using System.Text;

namespace Microondas.Infrastructure.Seguranca
{
    public static class HashHelper
    {
        public static string CriarHashSha256(string texto, string salt)
        {
            using (var sha = SHA256.Create())
            {
                var combinado = salt + texto;
                var bytes = Encoding.UTF8.GetBytes(combinado);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public static string GerarSaltAleatorio(int tamanho = 16)
        {
            var bytes = new byte[tamanho];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes);
        }
    }
}
