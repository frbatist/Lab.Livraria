using Lab.Livraria.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Lab.Livraria.Aplicacao
{
    public interface ILivroAplicacao : IDisposable
    {
        Task<IEnumerable<Livro>> ObterTodos();
        Task Inserir(params Livro[] livros);
        Task Alterar(Livro livro);
        Task Excluir(params long[] id);
        Task<Livro> ObterPorId(long? id);
    }
}