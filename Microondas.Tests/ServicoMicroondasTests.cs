using System;
using System.Linq;
using NUnit.Framework;
using Microondas.Business.Servicos;
using Microondas.Domain.Entidades;
using Microondas.Infrastructure.Repositorios;

namespace Microondas.Tests
{
    [TestFixture]
    public class ServicoMicroondasTests
    {
        private IServicoProgramas _servicoProgramas;
        private IServicoMicroondas _servicoMicroondas;
        private IRepositorioProgramas _repoFake;

        [SetUp]
        public void Setup()
        {
            // Usa repositório JSON com arquivo em pasta de testes (mas só para customizados)
            var caminho = System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "programas_custom_test.json");

            _repoFake = new RepositorioJsonProgramas(caminho);

            // Garante arquivo vazio a cada teste
            System.IO.File.WriteAllText(caminho, "[]");

            _servicoProgramas = new ServicoProgramas(_repoFake);
            _servicoMicroondas = new ServicoMicroondas(_servicoProgramas);
        }

        [Test]
        public void InicioRapido_DeveIniciarCom30SegundosEPotencia10()
        {
            _servicoMicroondas.InicioRapido();
            var sessao = _servicoMicroondas.ObterSessaoAtual();

            Assert.That(sessao.TempoRestanteSegundos, Is.EqualTo(30));
            Assert.That(sessao.Potencia, Is.EqualTo(10));
            Assert.That(sessao.CaractereAquecimento, Is.EqualTo('.'));
            Assert.That(sessao.Estado, Is.EqualTo(EstadoSessao.Rodando));
        }

        [Test]
        public void Iniciar_Manual_ComTempoEPotencia_ValidaValores()
        {
            _servicoMicroondas.Iniciar(20, 5, '*', null);
            var sessao = _servicoMicroondas.ObterSessaoAtual();

            Assert.That(sessao.TempoRestanteSegundos, Is.EqualTo(20));
            Assert.That(sessao.Potencia, Is.EqualTo(5));
            Assert.That(sessao.CaractereAquecimento, Is.EqualTo('*'));
            Assert.That(sessao.Estado, Is.EqualTo(EstadoSessao.Rodando));
        }

        [Test]
        public void Iniciar_ComTempoInvalido_DeveLancarException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _servicoMicroondas.Iniciar(0, 5, '.', null);
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _servicoMicroondas.Iniciar(121, 5, '.', null);
            });
        }

        [Test]
        public void Iniciar_ComPotenciaInvalida_DeveLancarException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _servicoMicroondas.Iniciar(10, 0, '.', null);
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _servicoMicroondas.Iniciar(10, 11, '.', null);
            });
        }

        [Test]
        public void Iniciar_ProgramaPreDefinido_DeveUsarTempoEPotenciaDoPrograma()
        {
            var pipoca = _servicoProgramas.ObterPreDefinidos()
                                          .First(p => p.Nome == "Pipoca");

            _servicoMicroondas.Iniciar(null, null, null, pipoca.Id);
            var sessao = _servicoMicroondas.ObterSessaoAtual();

            Assert.That(sessao.TempoRestanteSegundos, Is.EqualTo(pipoca.TempoSegundos));
            Assert.That(sessao.Potencia, Is.EqualTo(pipoca.Potencia));
            Assert.That(sessao.CaractereAquecimento, Is.EqualTo(pipoca.CaractereAquecimento));
            Assert.That(sessao.ProgramaId, Is.EqualTo(pipoca.Id));
            Assert.That(sessao.Estado, Is.EqualTo(EstadoSessao.Rodando));
        }

        [Test]
        public void Iniciar_EnquantoRodando_Manual_DeveAcrescentar30Segundos()
        {
            _servicoMicroondas.Iniciar(20, 5, '*', null);
            var antes = _servicoMicroondas.ObterSessaoAtual().TempoRestanteSegundos;

            _servicoMicroondas.Iniciar(); // clique em iniciar enquanto roda

            var depois = _servicoMicroondas.ObterSessaoAtual().TempoRestanteSegundos;
            Assert.That(depois, Is.EqualTo(antes + 30));
        }

        [Test]
        public void Iniciar_EnquantoRodando_PreDefinido_NaoDeveAcrescentarTempo()
        {
            var pipoca = _servicoProgramas.ObterPreDefinidos().First();
            _servicoMicroondas.Iniciar(null, null, null, pipoca.Id);
            var antes = _servicoMicroondas.ObterSessaoAtual().TempoRestanteSegundos;

            _servicoMicroondas.Iniciar(); // tentar acrescentar

            var depois = _servicoMicroondas.ObterSessaoAtual().TempoRestanteSegundos;
            Assert.That(depois, Is.EqualTo(antes)); // não mudou
        }

        [Test]
        public void Pausar_DeveMudarEstadoParaPausado()
        {
            _servicoMicroondas.Iniciar(30, 5, '*', null);
            _servicoMicroondas.PausarOuCancelar();

            var sessao = _servicoMicroondas.ObterSessaoAtual();
            Assert.That(sessao.Estado, Is.EqualTo(EstadoSessao.Pausado));
        }

        [Test]
        public void PausarDepoisCancelar_DeveZerarSessao()
        {
            _servicoMicroondas.Iniciar(30, 5, '*', null);
            _servicoMicroondas.PausarOuCancelar(); // pausa
            _servicoMicroondas.PausarOuCancelar(); // cancela

            var sessao = _servicoMicroondas.ObterSessaoAtual();
            Assert.That(sessao.Estado, Is.EqualTo(EstadoSessao.Parado));
            Assert.That(sessao.TempoRestanteSegundos, Is.EqualTo(0));
            Assert.That(sessao.Potencia, Is.EqualTo(0));
        }

        [Test]
        public void Iniciar_AposPausa_DeveRetomarDoMesmoTempo()
        {
            _servicoMicroondas.Iniciar(30, 5, '*', null);

            // simula 3 segundos passados
            _servicoMicroondas.Tick();
            _servicoMicroondas.Tick();
            _servicoMicroondas.Tick();
            var tempoAntesPausa = _servicoMicroondas.ObterSessaoAtual().TempoRestanteSegundos;

            _servicoMicroondas.PausarOuCancelar(); // pausa
            Assert.That(_servicoMicroondas.ObterSessaoAtual().Estado, Is.EqualTo(EstadoSessao.Pausado));

            _servicoMicroondas.Iniciar(); // retomar

            var sessao = _servicoMicroondas.ObterSessaoAtual();
            Assert.That(sessao.Estado, Is.EqualTo(EstadoSessao.Rodando));
            Assert.That(sessao.TempoRestanteSegundos, Is.EqualTo(tempoAntesPausa));
        }

        [Test]
        public void InicioRapido_QuandoJaRodandoOuPausado_NaoDeveAlterarSessao()
        {
            // Rodando
            _servicoMicroondas.Iniciar(40, 5, '*', null);
            var antes = _servicoMicroondas.ObterSessaoAtual().TempoRestanteSegundos;

            _servicoMicroondas.InicioRapido();
            var depois = _servicoMicroondas.ObterSessaoAtual().TempoRestanteSegundos;

            Assert.That(depois, Is.EqualTo(antes)); // não mudou

            // Pausado
            _servicoMicroondas.PausarOuCancelar();
            var antesPausado = _servicoMicroondas.ObterSessaoAtual().TempoRestanteSegundos;

            _servicoMicroondas.InicioRapido();
            var depoisPausado = _servicoMicroondas.ObterSessaoAtual().TempoRestanteSegundos;

            Assert.That(depoisPausado, Is.EqualTo(antesPausado));
            Assert.That(_servicoMicroondas.ObterSessaoAtual().Estado, Is.EqualTo(EstadoSessao.Pausado));
        }

        [Test]
        public void Tick_DeveDiminuirTempoEAtualizarStringProcesso()
        {
            _servicoMicroondas.Iniciar(5, 3, '#', null);
            _servicoMicroondas.Tick();

            var sessao = _servicoMicroondas.ObterSessaoAtual();
            Assert.That(sessao.TempoRestanteSegundos, Is.EqualTo(4));
            Assert.That(sessao.StringProcesso.Trim(), Is.EqualTo("###")); // 3 caracteres (potência)
        }
    }
}
