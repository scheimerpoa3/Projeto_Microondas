using Microondas.Domain.Entidades;
using Microondas.Domain.DTOs;
using Microondas.Domain.Excecoes;
using System;
using System.Text;

namespace Microondas.Business.Servicos
{
    public class ServicoMicroondas : IServicoMicroondas
    {
        private readonly IServicoProgramas _servicoProgramas;
        private SessaoAquecimento _sessao = new SessaoAquecimento();

        public ServicoMicroondas(IServicoProgramas servicoProgramas)
        {
            _servicoProgramas = servicoProgramas;
        }

        public SessaoAquecimento ObterSessaoAtual() => _sessao;

        private void ValidarTempo(int segundos)
        {
            if (segundos < 1 || segundos > 120)
                throw new RegraNegocioException("Tempo deve ser entre 1 e 120 segundos.");
        }

        private void ValidarPotencia(int potencia)
        {
            if (potencia < 1 || potencia > 10)
                throw new RegraNegocioException("Potência deve ser entre 1 e 10.");
        }


        public void Iniciar(int? tempoSegundos = null, int? potencia = null, char? caractere = null, Guid? programaId = null)
        {
            // 1) Já está rodando
            if (_sessao.Estado == EstadoSessao.Rodando)
            {
                // Se estiver rodando com programa pré-definido, NÃO permite acréscimo de tempo
                if (_sessao.ProgramaId.HasValue)
                {
                    var progAtual = _servicoProgramas.ObterPorId(_sessao.ProgramaId.Value);
                    if (progAtual != null && progAtual.EhPreDefinido)
                    {
                        // Apenas ignora o clique em Iniciar
                        return;
                    }
                }

                // Sessão manual ou programa customizado -> pode acrescentar 30s
                _sessao.TempoRestanteSegundos += 30;
                return;
            }

            // 2) Estava pausado -> retoma do ponto onde parou
            if (_sessao.Estado == EstadoSessao.Pausado)
            {
                _sessao.Estado = EstadoSessao.Rodando;
                return;
            }

            // 3) Nova sessão (Parado ou Concluído)

            // 3.1) Programa selecionado (pré-definido OU customizado)
            if (programaId.HasValue)
            {
                var prog = _servicoProgramas.ObterPorId(programaId.Value);
                if (prog == null)
                    throw new RegraNegocioException("Programa de aquecimento não encontrado.");
               
                _sessao = new SessaoAquecimento
                {
                    ProgramaId = prog.Id,
                    TempoRestanteSegundos = prog.TempoSegundos,
                    Potencia = prog.Potencia,
                    CaractereAquecimento = prog.CaractereAquecimento,
                    Estado = EstadoSessao.Rodando,
                    StringProcesso = string.Empty
                };
                return;
            }

            // 3.2) Nenhum valor informado -> início rápido
            if (!tempoSegundos.HasValue && !potencia.HasValue && !caractere.HasValue)
            {
                InicioRapido();
                return;
            }

            // 3.3) Aquecimento manual (tempo/potência digitados)
            int t = tempoSegundos ?? 30;
            int p = potencia ?? 10;
            char c = caractere ?? '.';

            ValidarTempo(t);
            ValidarPotencia(p);

            _sessao = new SessaoAquecimento
            {
                ProgramaId = null,
                TempoRestanteSegundos = t,
                Potencia = p,
                CaractereAquecimento = c,
                Estado = EstadoSessao.Rodando,
                StringProcesso = string.Empty
            };
        }



        public void InicioRapido()
        {
            // Se já existe aquecimento em andamento OU pausado, não faz nada.
            // Apenas o botão INICIAR pode retomar ou acrescentar tempo.
            if (_sessao.Estado == EstadoSessao.Rodando ||
                _sessao.Estado == EstadoSessao.Pausado)
            {
                return;
            }

            // Só cria nova sessão de início rápido se estiver Parado ou Concluído
            _sessao = new SessaoAquecimento
            {
                ProgramaId = null,
                TempoRestanteSegundos = 30,
                Potencia = 10,
                CaractereAquecimento = '.',
                Estado = EstadoSessao.Rodando,
                StringProcesso = string.Empty
            };
        }


        public void PausarOuCancelar()
        {
            if (_sessao.Estado == EstadoSessao.Rodando)
            {
                _sessao.Estado = EstadoSessao.Pausado;
                return;
            }
            if (_sessao.Estado == EstadoSessao.Pausado)
            {
                _sessao = new SessaoAquecimento { Estado = EstadoSessao.Parado };
                return;
            }
            _sessao = new SessaoAquecimento { Estado = EstadoSessao.Parado };
        }

        public void Tick()
        {
            if (_sessao.Estado != EstadoSessao.Rodando) return;

            if (_sessao.TempoRestanteSegundos <= 0)
            {
                _sessao.Estado = EstadoSessao.Concluido;
                _sessao.StringProcesso = (_sessao.StringProcesso + "  - Aquecimento concluído - ").Trim();
                return;
            }

            _sessao.TempoRestanteSegundos -= 1;

            var sb = new StringBuilder(_sessao.StringProcesso ?? string.Empty);
            for (int i = 0; i < _sessao.Potencia; i++)
                sb.Append(_sessao.CaractereAquecimento);
            sb.Append(' ');
            _sessao.StringProcesso = sb.ToString();

            if (_sessao.TempoRestanteSegundos <= 0)
            {
                _sessao.Estado = EstadoSessao.Concluido;
                _sessao.StringProcesso = _sessao.StringProcesso.TrimEnd() + "  -> Aquecimento concluído <- ";
            }
        }

        public StatusSessaoDto ObterStatusDto()
        {
            return new StatusSessaoDto
            {
                TempoRestanteSegundos = _sessao.TempoRestanteSegundos,
                TempoFormatado = FormatarTempo(_sessao.TempoRestanteSegundos),
                Potencia = _sessao.Potencia,
                Caractere = _sessao.CaractereAquecimento,
                StringProcesso = _sessao.StringProcesso,
                Estado = _sessao.Estado.ToString()
            };
        }

        private string FormatarTempo(int segundos)
        {
            if (segundos < 0) segundos = 0;
            int minutos = segundos / 60;
            int seg = segundos % 60;
            if (minutos > 0)
                return $"{minutos}:{seg:D2}";
            return $"{seg}s";
        }
    }
}
