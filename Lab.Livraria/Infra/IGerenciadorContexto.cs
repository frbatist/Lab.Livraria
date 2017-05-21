using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Lab.Livraria.Infra
{
    public interface IGerenciadorContexto
    {
        DbContext ObterContexto();
    }
}