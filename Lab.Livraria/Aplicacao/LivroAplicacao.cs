using Lab.Livraria.Entidades;
using Lab.Livraria.Models.ORM;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Lab.Livraria.Aplicacao
{
    public class LivroAplicacao : IDisposable
    {
        private DbContext _contexto;
        private DbSet<Livro> _livroSet;

        public LivroAplicacao()
        {
            //Em um caso na vida real, seria criado através de injeção de dependencias
            _contexto = new LivrariaContexto();
            _livroSet = _contexto.Set<Livro>();
        }

        public async Task<IEnumerable<Livro>> ObterTodos()
        {
            return await _livroSet.ObterTodos().ToListAsync();
        }        

        public async Task Inserir(params Livro[] livros)
        {
            _livroSet.AddRange(livros);
            await _contexto.SaveChangesAsync();
        }

        public async Task Alterar(Livro livro)
        {
            _contexto.Entry<Livro>(livro).State = EntityState.Modified;
            await _contexto.SaveChangesAsync();
        }

        public async Task Excluir(params long[] id)
        {
            IEnumerable<Livro> livros = await _livroSet.Where(d=> id.Contains(d.Id)).ToListAsync();
            _livroSet.RemoveRange(livros);
            await _contexto.SaveChangesAsync();
        }

        public async Task<Livro> ObterPorId(long? id)
        {
            return await _livroSet.FindAsync(id);
        }

        public void Dispose()
        {
            _livroSet = null;
            _contexto.Dispose();
        }
    }
}