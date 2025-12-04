using Microondas.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace Microondas.Infrastructure.Repositorios
{
    public class RepositorioJsonProgramas : IRepositorioProgramas
    {
        private string _arquivo;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public RepositorioJsonProgramas(string caminhoArquivo = null)
        {
            _arquivo = caminhoArquivo ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "programas_custom.json");
            if (!File.Exists(_arquivo))
                File.WriteAllText(_arquivo, "[]");
        }

        private List<ProgramaAquecimento> LerTodos()
        {
            _lock.EnterReadLock();
            try
            {
                var txt = File.ReadAllText(_arquivo);
                var lista = JsonConvert.DeserializeObject<List<ProgramaAquecimento>>(txt) ?? new List<ProgramaAquecimento>();
                return lista;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        private void GravarTodos(IEnumerable<ProgramaAquecimento> programas)
        {
            _lock.EnterWriteLock();
            try
            {
                var txt = JsonConvert.SerializeObject(programas, Formatting.Indented);
                File.WriteAllText(_arquivo, txt);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public IEnumerable<ProgramaAquecimento> ObterTodos() => LerTodos();

        public ProgramaAquecimento ObterPorId(Guid id) => LerTodos().FirstOrDefault(p => p.Id == id);

        public void Inserir(ProgramaAquecimento programa)
        {
            if (programa.Id == Guid.Empty)
                programa.Id = Guid.NewGuid();  

            var todos = LerTodos();
            todos.Add(programa);
            GravarTodos(todos);
        }

        public void Atualizar(ProgramaAquecimento programa)
        {
            var todos = LerTodos();
            var ex = todos.FirstOrDefault(p => p.Id == programa.Id);
            if (ex != null)
            {
                todos.Remove(ex);
                todos.Add(programa);
                GravarTodos(todos);
            }
        }

        public void Remover(Guid id)
        {
            var todos = LerTodos();
            var ex = todos.FirstOrDefault(p => p.Id == id);
            if (ex != null)
            {
                todos.Remove(ex);
                GravarTodos(todos);
            }
        }
    }
}
