using Lab.Livraria.Models.ORM;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Lab.Livraria.Infra
{
    public class GerenciadorContexto : IGerenciadorContexto
    {
        public DbContext ObterContexto()
        {
            return new LivrariaContexto();
        }
    }
}