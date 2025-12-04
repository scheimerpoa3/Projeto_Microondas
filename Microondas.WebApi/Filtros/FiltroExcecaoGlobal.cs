using Microondas.Infrastructure.Logger;
using Microondas.Domain.Excecoes;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace Microondas.WebApi.Filtros
{
    public class FiltroExcecaoGlobal : ExceptionFilterAttribute
    {
        private readonly ArquivoLogger _logger = new ArquivoLogger();

        public override void OnException(HttpActionExecutedContext context)
        {
            _logger.LogErro(context.Exception);

            // Regras de negócio → HTTP 400
            if (context.Exception is RegraNegocioException)
            {
                var respostaNegocio = context.Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    Erro = "Regra de negócio violada.",
                    Mensagem = context.Exception.Message
                });

                context.Response = respostaNegocio;
                return;
            }

            // Demais → 500 padrão
            var resposta = context.Request.CreateResponse(HttpStatusCode.InternalServerError, new
            {
                Erro = "Ocorreu um erro interno.",
                Mensagem = context.Exception.Message
            });

            context.Response = resposta;
        }
    }
}
