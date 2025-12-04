using Microondas.Business.Servicos;
using Microondas.Domain.DTOs;
using Microondas.Domain.Entidades;
using Microondas.WebApi.Seguranca;
using System;
using Microondas.Domain.Excecoes;
using System.Web.Http;

namespace Microondas.WebApi.Controllers
{
    [RoutePrefix("api/microondas")]
    [AutorizacaoToken]
    public class MicroondasController : ApiController
    {
        private static IServicoMicroondas _servicoMicroondas;

        static MicroondasController()
        {
            var repo = new Microondas.Infrastructure.Repositorios.RepositorioJsonProgramas();
            var servicoProgramas = new ServicoProgramas(repo);
            _servicoMicroondas = new ServicoMicroondas(servicoProgramas);
        }

        [HttpPost, Route("iniciar")]
        public IHttpActionResult Iniciar([FromBody] InicioRequestDto dto)
        {
            try
            {
                _servicoMicroondas.Iniciar(dto?.TempoSegundos, dto?.Potencia, dto?.CaractereAquecimento, dto?.ProgramaId);
                return Ok();
            }
            catch (RegraNegocioException ex) { return BadRequest(ex.Message); }
            catch (Exception ex) { return InternalServerError(ex); }
        }

        [HttpPost, Route("iniciorapido")]
        public IHttpActionResult InicioRapido()
        {
            _servicoMicroondas.InicioRapido();
            return Ok();
        }

        [HttpPost, Route("pausarcancelar")]
        public IHttpActionResult PausarCancelar()
        {
            _servicoMicroondas.PausarOuCancelar();
            return Ok();
        }

        [HttpGet, Route("status")]
        public IHttpActionResult Status()
        {
            var dto = _servicoMicroondas.ObterStatusDto();
            return Ok(dto);
        }

        [HttpPost, Route("tick")]
        public IHttpActionResult Tick()
        {
            _servicoMicroondas.Tick();
            return Ok(_servicoMicroondas.ObterStatusDto());
        }
    }
}
