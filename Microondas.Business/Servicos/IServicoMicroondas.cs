using Microondas.Domain.Entidades;
using Microondas.Domain.DTOs;

namespace Microondas.Business.Servicos
{
    public interface IServicoMicroondas
    {
        SessaoAquecimento ObterSessaoAtual();
        void Iniciar(int? tempoSegundos = null, int? potencia = null, char? caractere = null, System.Guid? programaId = null);
        void InicioRapido();
        void PausarOuCancelar();
        void Tick();
        StatusSessaoDto ObterStatusDto();
    }
}
