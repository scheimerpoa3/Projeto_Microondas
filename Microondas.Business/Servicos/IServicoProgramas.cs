using Microondas.Domain.Entidades;
using System;
using System.Collections.Generic;

namespace Microondas.Business.Servicos
{
    public interface IServicoProgramas
    {
        IEnumerable<ProgramaAquecimento> ObterTodos();
        ProgramaAquecimento ObterPorId(Guid id);
        void CadastrarProgramaCustomizado(ProgramaAquecimento programa);
        IEnumerable<ProgramaAquecimento> ObterPreDefinidos();
        void AtualizarProgramaCustomizado(ProgramaAquecimento programa);
        void RemoverProgramaCustomizado(Guid id);
    }
}
