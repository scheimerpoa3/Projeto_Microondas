using Microondas.Domain.Entidades;
using System;
using System.Collections.Generic;

namespace Microondas.Infrastructure.Repositorios
{
    public interface IRepositorioProgramas
    {
        IEnumerable<ProgramaAquecimento> ObterTodos();
        ProgramaAquecimento ObterPorId(Guid id);
        void Inserir(ProgramaAquecimento programa);
        void Atualizar(ProgramaAquecimento programa);
        void Remover(Guid id);
    }
}
