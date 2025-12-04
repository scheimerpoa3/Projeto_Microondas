using Microondas.Domain.Entidades;
using Microondas.Domain.Excecoes;
using Microondas.Infrastructure.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microondas.Business.Servicos
{
    public class ServicoProgramas : IServicoProgramas
    {
        private readonly IRepositorioProgramas _repoCustom;
        private readonly List<ProgramaAquecimento> _preDefinidos;

        public ServicoProgramas(IRepositorioProgramas repoCustom)
        {
            _repoCustom = repoCustom;
            _preDefinidos = CriarPreDefinidos();
        }

        private List<ProgramaAquecimento> CriarPreDefinidos()
        {
            return new List<ProgramaAquecimento>
            {

        new ProgramaAquecimento {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Nome="Pipoca",
            Alimento="Milho para pipoca",
            TempoSegundos = 3*60,
            Potencia = 7,
            CaractereAquecimento = 'P',
            Instrucoes = "Observar estouros; se houver intervalo > 10s entre estouros, interrompa.",
            EhPreDefinido = true
        },
        new ProgramaAquecimento {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Nome="Leite",
            Alimento="Leite",
            TempoSegundos = 5*60,
            Potencia = 5,
            CaractereAquecimento = 'L',
            Instrucoes = "Cuidado com fervura súbita; risco de queimadura.",
            EhPreDefinido = true
        },
        new ProgramaAquecimento {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Nome="Carnes de boi",
            Alimento="Carne em pedaços ou fatias",
            TempoSegundos = 14*60,
            Potencia = 4,
            CaractereAquecimento = 'C',
            Instrucoes = "Interrompa na metade e vire para descongelar uniformemente.",
            EhPreDefinido = true
        },
        new ProgramaAquecimento {
            Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            Nome="Frango",
            Alimento="Frango (qualquer corte)",
            TempoSegundos = 8*60,
            Potencia = 7,
            CaractereAquecimento = 'F',
            Instrucoes = "Interrompa na metade e vire para descongelar uniformemente.",
            EhPreDefinido = true
        },
        new ProgramaAquecimento {
            Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
            Nome="Feijão",
            Alimento="Feijão congelado",
            TempoSegundos = 8*60,
            Potencia = 9,
            CaractereAquecimento = 'E',
            Instrucoes = "Deixe o recipiente destampado; atenção a recipientes plásticos.",
            EhPreDefinido = true
        }
            };
        }

        public IEnumerable<ProgramaAquecimento> ObterTodos()
        {
            var customs = _repoCustom.ObterTodos() ?? new List<ProgramaAquecimento>();
            return _preDefinidos.Concat(customs);
        }

        public IEnumerable<ProgramaAquecimento> ObterPreDefinidos() => _preDefinidos;

        public ProgramaAquecimento ObterPorId(Guid id)
        {
            var p = _preDefinidos.FirstOrDefault(x => x.Id == id);
            if (p != null) return p;
            return _repoCustom.ObterPorId(id);
        }

        private void ValidarProgramaCustomizado(ProgramaAquecimento programa, bool atualizacao = false)
        {
            if (programa == null)
                throw new RegraNegocioException("Programa inválido.");

            if (string.IsNullOrWhiteSpace(programa.Nome))
                throw new RegraNegocioException("Nome obrigatório.");

            if (string.IsNullOrWhiteSpace(programa.Alimento))
                throw new RegraNegocioException("Alimento é obrigatório.");

            if (programa.TempoSegundos < 1 || programa.TempoSegundos > 60 * 60)
                throw new RegraNegocioException("Tempo inválido. Deve estar entre 1 segundo e 60 minutos.");

            if (programa.Potencia < 1 || programa.Potencia > 10)
                throw new RegraNegocioException("Potência inválida. Deve estar entre 1 e 10.");

            if (programa.CaractereAquecimento == '.')
                throw new RegraNegocioException("Caractere '.' não é permitido para programas customizados.");

            // valida caractere duplicado (ignora o próprio em caso de atualização)
            var todos = ObterTodos();
            if (todos.Any(t => t.CaractereAquecimento == programa.CaractereAquecimento &&
                               (!atualizacao || t.Id != programa.Id)))
            {
                throw new RegraNegocioException("Caractere de aquecimento já está em uso por outro programa.");
            }
        }
        public void CadastrarProgramaCustomizado(ProgramaAquecimento programa)
        {
            ValidarProgramaCustomizado(programa);

            if (programa.Id == Guid.Empty)
                programa.Id = Guid.NewGuid();

            programa.EhPreDefinido = false;

            _repoCustom.Inserir(programa);
        }

        public void AtualizarProgramaCustomizado(ProgramaAquecimento programa)
        {
            ValidarProgramaCustomizado(programa, atualizacao: true);

            var existente = _repoCustom.ObterPorId(programa.Id);
            if (existente == null)
                throw new RegraNegocioException("Programa customizado não encontrado.");

            if (existente.EhPreDefinido)
                throw new RegraNegocioException("Programas pré-definidos não podem ser alterados.");

            programa.EhPreDefinido = false;
            _repoCustom.Atualizar(programa);
        }
        public void RemoverProgramaCustomizado(Guid id)
        {
            var existente = ObterPorId(id);
            if (existente == null)
                return;

            if (existente.EhPreDefinido)
                throw new RegraNegocioException("Programas pré-definidos não podem ser excluídos.");

            _repoCustom.Remover(id);
        }
    }
}
