using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lab.Livraria.Entidades
{
    public class Livro
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public int Edicao { get; set; }
        public string Autor { get; set; }
        public int Ano { get; set; }
    }
}