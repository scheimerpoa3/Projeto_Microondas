using System;
using System.Linq;
using NUnit.Framework;
using Microondas.Business.Servicos;
using Microondas.Domain.Entidades;
using Microondas.Infrastructure.Repositorios;

namespace Microondas.Tests
{
    [TestFixture]
    public class ServicoProgramasTests
    {
        private ServicoProgramas _servico;
        private IRepositorioProgramas _repoCustom;

        [SetUp]
        public void Setup()
        {
            var caminho = System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "programas_custom_test.json");

            _repoCustom = new RepositorioJsonProgramas(caminho);
            System.IO.File.WriteAllText(caminho, "[]");

            _servico = new ServicoProgramas(_repoCustom);
        }

        [Test]
        public void PreDefinidos_DevemSerCincoETodosComEhPreDefinidoTrue()
        {
            var pre = _servico.ObterPreDefinidos().ToList();

            Assert.That(pre.Count, Is.EqualTo(5));
            Assert.That(pre.All(p => p.EhPreDefinido), Is.True);
        }

        [Test]
        public void PreDefinidos_CaracteresDevemSerDistintosENaoPonto()
        {
            var pre = _servico.ObterPreDefinidos().ToList();

            var chars = pre.Select(p => p.CaractereAquecimento).ToList();
            Assert.That(chars.All(c => c != '.'), Is.True);
            Assert.That(chars.Distinct().Count(), Is.EqualTo(chars.Count));
        }

        [Test]
        public void CadastrarCustomizado_DevePersistirComIdENaoPreDefinido()
        {
            var programa = new ProgramaAquecimento
            {
                Nome = "Meu Custom",
                Alimento = "Teste",
                TempoSegundos = 90,
                Potencia = 5,
                CaractereAquecimento = 'X',
                Instrucoes = "Teste custom"
            };

            _servico.CadastrarProgramaCustomizado(programa);

            var todos = _servico.ObterTodos().ToList();
            var encontrado = todos.FirstOrDefault(p => p.Nome == "Meu Custom");

            Assert.That(encontrado, Is.Not.Null);
            Assert.That(encontrado.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(encontrado.EhPreDefinido, Is.False);
        }

        [Test]
        public void CadastrarCustomizado_ComCaractereRepetido_DeveLancarException()
        {
            var pre = _servico.ObterPreDefinidos().First();
            var programa = new ProgramaAquecimento
            {
                Nome = "Repetido",
                Alimento = "Teste",
                TempoSegundos = 60,
                Potencia = 5,
                CaractereAquecimento = pre.CaractereAquecimento,
                Instrucoes = "Teste"
            };

            var ex = Assert.Throws<ArgumentException>(() =>
            {
                _servico.CadastrarProgramaCustomizado(programa);
            });

            StringAssert.Contains("Caractere de aquecimento já está em uso", ex.Message);
        }

        [Test]
        public void CadastrarCustomizado_ComCaracterePonto_DeveLancarException()
        {
            var programa = new ProgramaAquecimento
            {
                Nome = "Ponto",
                Alimento = "Teste",
                TempoSegundos = 60,
                Potencia = 5,
                CaractereAquecimento = '.',
                Instrucoes = "Teste"
            };

            var ex = Assert.Throws<ArgumentException>(() =>
            {
                _servico.CadastrarProgramaCustomizado(programa);
            });

            StringAssert.Contains("Caractere '.' não é permitido", ex.Message);
        }

        [Test]
        public void ObterPorId_PreDefinido_DeveRetornarProgramaCorreto()
        {
            var pre = _servico.ObterPreDefinidos().First();
            var encontrado = _servico.ObterPorId(pre.Id);

            Assert.That(encontrado, Is.Not.Null);
            Assert.That(encontrado.Nome, Is.EqualTo(pre.Nome));
            Assert.That(encontrado.EhPreDefinido, Is.True);
        }
    }
}
