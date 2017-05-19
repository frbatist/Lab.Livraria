using Lab.Livraria.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace Lab.Livraria.ORM.Mapeamentos
{
    public class LivroMapeamento : EntityTypeConfiguration<Livro>
    {
        public LivroMapeamento()
        {
            Property(d => d.Nome).IsFixedLength().IsUnicode().HasMaxLength(80);
            Property(d => d.Autor).IsFixedLength().IsUnicode().HasMaxLength(80);
        }
    }
}