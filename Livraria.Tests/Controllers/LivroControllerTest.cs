using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using System.Data.Entity;
using Lab.Livraria.Entidades;
using Lab.Livraria.Controllers;
using Lab.Livraria.Aplicacao;
using System.Web.Mvc;
using System.Data.Entity.Infrastructure;

namespace Livraria.Tests.Controllers
{
    [TestClass]
    public class LivroControllerTest
    {
        [TestMethod]
        public async Task Index()
        {
            var contexto = new Mock<DbContext>();
            var mockSet = new Mock<DbSet<Livro>>();
            Livro[] livros = ObterLivrosCenarioTeste();
            ConfigurarMockSet(livros, contexto, mockSet);

            ILivroAplicacao livroAplicacao = new LivroAplicacao(contexto.Object);
            LivroController controller = new LivroController(livroAplicacao);

            ViewResult result = await controller.Index() as ViewResult;

            Assert.IsNotNull(result);
        }

        private Livro[] ObterLivrosCenarioTeste()
        {
            return new Livro[]
            {
                new Livro{ Id = 1, Nome = "Livro 01", Ano = 2017, Autor = "Teste", Edicao = 1 },
                new Livro{ Id = 2, Nome = "Livro 02", Ano = 2016, Autor = "Teste 2", Edicao = 3 },
                new Livro{ Id = 3, Nome = "Livro 03", Ano = 2014, Autor = "Teste 3", Edicao = 5 },
                new Livro{ Id = 4, Nome = "Livro 04", Ano = 2015, Autor = "Teste 4", Edicao = 2 },
            };
        }

        public static void ConfigurarMockSet(Livro[] objetos, Mock<DbContext> contextoMock, Mock<DbSet<Livro>> mockSet)
        {
            var data = objetos.AsQueryable();
            mockSet.As<IDbAsyncEnumerable<Livro>>().Setup(m => m.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<Livro>(data.GetEnumerator()));
            mockSet.As<IQueryable<Livro>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Livro>(data.Provider)); 
            mockSet.As<IQueryable<Livro>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Livro>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Livro>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            
            mockSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => data.FirstOrDefault(p => (long)p.GetType().GetProperty("Id").GetValue(p, null) == (long)ids[0]));
            contextoMock.Setup(m => m.Set<Livro>()).Returns(mockSet.Object);
        }
    }
}
