using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using Lab.Livraria.Entidades;
using Lab.Livraria.Aplicacao;

namespace Lab.Livraria.Controllers
{
    public class LivroController : Controller
    {
        private ILivroAplicacao _livroAplicacao;

        public LivroController(ILivroAplicacao livroAplicacao)
        {
            _livroAplicacao = livroAplicacao;
        }

        public async Task<ActionResult> Index()
        {
            return View(await _livroAplicacao.ObterTodos());
        }

        public ActionResult Inserir()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Inserir(Livro livro)
        {
            if (ModelState.IsValid)
            {
                await _livroAplicacao.Inserir(livro);                
                return RedirectToAction("Index");
            }

            return View(livro);
        }
        
        public async Task<ActionResult> Alterar(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Livro livro = await _livroAplicacao.ObterPorId(id);
            if (livro == null)
            {
                return HttpNotFound();
            }
            return View(livro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Alterar(Livro livro)
        {
            if (ModelState.IsValid)
            {
                await _livroAplicacao.Alterar(livro);                
                return RedirectToAction("Index");
            }
            return View(livro);
        }

        // GET: Livro/Delete/5
        public async Task<ActionResult> Excluir(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Livro livro = await _livroAplicacao.ObterPorId(id);
            if (livro == null)
            {
                return HttpNotFound();
            }
            return View(livro);
        }

        
        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExcluirConfirmado(long id)
        {
            await _livroAplicacao.Excluir(id);            
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _livroAplicacao.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
