using System;

namespace Microondas.Domain.Excecoes
{
    public class RegraNegocioException : Exception
    {
        public RegraNegocioException() { }

        public RegraNegocioException(string message) : base(message) { }

        public RegraNegocioException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
