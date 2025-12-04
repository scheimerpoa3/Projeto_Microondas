using Microondas.Business.Servicos;
using Microondas.Domain.Entidades;
using Microondas.WebApi.Seguranca;
using System;
using System.Web.Http;
using Microondas.Domain.Excecoes;

namespace Microondas.WebApi.Controllers
{
    [RoutePrefix("api/programas")]
    [AutorizacaoToken]
    public class ProgramasController : ApiController
    {
        private readonly IServicoProgramas _servico;

        public ProgramasController()
        {
            var repo = new Microondas.Infrastructure.Repositorios.RepositorioJsonProgramas();
            _servico = new ServicoProgramas(repo);
        }

        [HttpGet, Route("")]
        public IHttpActionResult ObterTodos()
        {
            return Ok(_servico.ObterTodos());
        }

        [HttpPost, Route("custom")]
        public IHttpActionResult CadastrarCustom([FromBody] ProgramaAquecimento programa)
        {
            try
            {
                _servico.CadastrarProgramaCustomizado(programa);
                return Ok();
            }
            catch (RegraNegocioException ex) { return BadRequest(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet, Route("{id:guid}")]
        public IHttpActionResult ObterPorId(Guid id)
        {
            var p = _servico.ObterPorId(id);
            if (p == null) return NotFound();
            return Ok(p);
        }
        // PUT api/programas/{id}
        [HttpPut, Route("{id:guid}")]
        public IHttpActionResult Atualizar(Guid id, [FromBody] ProgramaAquecimento programa)
        {
            try
            {
                if (programa == null || programa.Id == Guid.Empty || programa.Id != id)
                    return BadRequest("Programa inválido.");

                _servico.AtualizarProgramaCustomizado(programa);
                return Ok();
            }
            catch (RegraNegocioException ex) { return BadRequest(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // DELETE api/programas/{id}
        [HttpDelete, Route("{id:guid}")]
        public IHttpActionResult Remover(Guid id)
        {
            try
            {
                _servico.RemoverProgramaCustomizado(id);
                return Ok();
            }
            catch (RegraNegocioException ex) { return BadRequest(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

    }
}
