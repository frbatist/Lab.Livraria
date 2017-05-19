using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Data.Entity.ModelConfiguration.Conventions;
using Lab.Livraria.ORM.Mapeamentos;

namespace Lab.Livraria.Models.ORM
{
    public class LivrariaContexto : DbContext
    {
        public LivrariaContexto() : base("conexaoLivraria")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Configurations.Add(new LivroMapeamento());
        }

        public System.Data.Entity.DbSet<Lab.Livraria.Entidades.Livro> Livroes { get; set; }
    }
}