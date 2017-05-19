using Lab.Livraria.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Lab.Livraria.Models.ORM
{
    public static class LivroRepositorio
    {
        public static IQueryable<Livro> ObterTodos(this DbSet<Livro> livrosSet)
        {
            var consulta = from livro in livrosSet
                           orderby livro.Nome ascending
                           select livro;
            return consulta;
        }
    }
}