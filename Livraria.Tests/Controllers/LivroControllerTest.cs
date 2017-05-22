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
            //nesse caso, mocamos o aplicacao, porque o método do update do EF6 não permite mock
            var aplicacao = new Mock<ILivroAplicacao>();            
            Livro[] livros = ObterLivrosCenarioTeste();
            var livro = livros.Where(d => d.Id == 1).First();
            var nomeAnterior = livro.Nome;
            livro.Nome = "Novo nome";
            aplicacao.Setup(a => a.Alterar(It.IsAny<Livro>())).Returns(Task.FromResult(false)).Verifiable();
            LivroController controller = new LivroController(aplicacao.Object);
            ViewResult result = await controller.Alterar(livro) as ViewResult;

            var livroAlterado = livros.Where(d=>d.Id == 1).FirstOrDefault();
            Assert.AreNotEqual(nomeAnterior, livroAlterado.Nome);
            aplicacao.Verify(m => m.Alterar(It.IsAny<Livro>()), Times.AtLeastOnce(), "É necessário invocar o método alterar da interface ILivroAplicacao!");
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

            mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .Returns<object[]>(async (d) =>
                {
                    return await Task.FromResult(ObterPorId<Livro>(data, d));
                });


            mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .Returns<object[]>(async (d) =>
                {
                    return await Task.Run(() => { return data.Where(a => d.Contains(a.Id)).FirstOrDefault(); });
                });
            contextoMock.Setup(m => m.Set<Livro>()).Returns(mockSet.Object);
        }

        private static Livro ObterPorId<T>(IQueryable<Livro> data, object[] d)
        {
            return data.Where(e => d.Contains(e.Id)).FirstOrDefault();
        }
    }
}
