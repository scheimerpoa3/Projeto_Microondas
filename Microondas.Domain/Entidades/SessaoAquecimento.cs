using System;

namespace Microondas.Domain.Entidades
{
    public enum EstadoSessao { Parado, Rodando, Pausado, Concluido }

    public class SessaoAquecimento
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? ProgramaId { get; set; }
        public int TempoRestanteSegundos { get; set; }
        public int Potencia { get; set; }
        public char CaractereAquecimento { get; set; }
        public EstadoSessao Estado { get; set; } = EstadoSessao.Parado;
        public string StringProcesso { get; set; } = string.Empty;
    }
}
