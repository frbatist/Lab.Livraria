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
            LivroController controller = new LivroController(ObterMockAplicacao());
            ViewResult result = await controller.Index() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetInserir()
        {
            LivroController controller = new LivroController(ObterMockAplicacao());
            ViewResult result = controller.Inserir() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task PostInserir()
        {
            var contexto = new Mock<DbContext>();
            var mockSet = new Mock<DbSet<Livro>>();
            Livro[] livros = ObterLivrosCenarioTeste();
            ConfigurarMockSet(livros, contexto, mockSet);
            var aplicacao = new LivroAplicacao(contexto.Object);
            var livro = new Livro
            {
                Id = 5,
                Nome = "Livro teste",
                Autor = "Autor teste",
                Ano = 2011,
                Edicao = 1
            };
            LivroController controller = new LivroController(aplicacao);
            ViewResult result = await controller.Inserir(livro) as ViewResult;
            mockSet.Verify(m => m.AddRange(It.IsAny<IEnumerable<Livro>>()), Times.Once(), "Um livro deve ser inserido pelo método");
            contexto.Verify(m => m.SaveChangesAsync(), Times.AtLeastOnce(), "É necessário finalizar o contexto!");
        }

        [TestMethod]
        public async Task GetAlterar()
        {
            LivroController controller = new LivroController(ObterMockAplicacao());
            ViewResult result = await controller.Alterar(1) as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task PostAlterar()
        {
            var contexto = new Mock<DbContext>();
            var mockSet = new Mock<DbSet<Livro>>();
            Livro[] livros = ObterLivrosCenarioTeste();
            ConfigurarMockSet(livros, contexto, mockSet);
            var aplicacao = new LivroAplicacao(contexto.Object);
            var livro = livros.Where(d => d.Id == 1).First();
            var nomeAnterior = livro.Nome;
            livro.Nome = "Novo nome";
            LivroController controller = new LivroController(aplicacao);
            ViewResult result = await controller.Alterar(livro) as ViewResult;

            Assert.AreNotEqual(nomeAnterior, contexto.Object.Set<Livro>().Find(1).Nome);
            contexto.Verify(m => m.SaveChangesAsync(), Times.AtLeastOnce(), "É necessário finalizar o contexto!");
        }

        [TestMethod]
        public async Task GetExcluir()
        {
            LivroController controller = new LivroController(ObterMockAplicacao());
            ViewResult result = await controller.Excluir(1) as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task PostExcluir()
        {
            var contexto = new Mock<DbContext>();
            var mockSet = new Mock<DbSet<Livro>>();
            Livro[] livros = ObterLivrosCenarioTeste();
            ConfigurarMockSet(livros, contexto, mockSet);
            var aplicacao = new LivroAplicacao(contexto.Object);
            LivroController controller = new LivroController(aplicacao);
            ViewResult result = await controller.ExcluirConfirmado(1) as ViewResult;

            Assert.IsNull(contexto.Object.Set<Livro>().Find(1));
            contexto.Verify(m => m.SaveChangesAsync(), Times.AtLeastOnce(), "É necessário finalizar o contexto!");
        }

        private ILivroAplicacao ObterMockAplicacao()
        {
            var contexto = new Mock<DbContext>();
            var mockSet = new Mock<DbSet<Livro>>();
            Livro[] livros = ObterLivrosCenarioTeste();
            ConfigurarMockSet(livros, contexto, mockSet);
            return new LivroAplicacao(contexto.Object);
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
