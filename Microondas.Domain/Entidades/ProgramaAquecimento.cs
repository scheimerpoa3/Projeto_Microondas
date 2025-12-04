using System;

namespace Microondas.Domain.Entidades
{
    public class ProgramaAquecimento
    {
        public Guid Id { get; set; } 
        public string Nome { get; set; }
        public string Alimento { get; set; }
        public int TempoSegundos { get; set; }
        public int Potencia { get; set; }
        public char CaractereAquecimento { get; set; }
        public string Instrucoes { get; set; }
        public bool EhPreDefinido { get; set; } = false;
    }
}
