using System;

namespace Microondas.Domain.DTOs
{
    public class InicioRequestDto
    {
        public int? TempoSegundos { get; set; }
        public int? Potencia { get; set; }
        public char? CaractereAquecimento { get; set; }
        public Guid? ProgramaId { get; set; }
    }

    public class StatusSessaoDto
    {
        public int TempoRestanteSegundos { get; set; }
        public string TempoFormatado { get; set; }
        public int Potencia { get; set; }
        public char Caractere { get; set; }
        public string StringProcesso { get; set; }
        public string Estado { get; set; }
    }
}
