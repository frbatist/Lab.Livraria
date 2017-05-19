using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using Lab.Livraria.Entidades;
using Lab.Livraria.Aplicacao;

namespace Lab.Livraria.Controllers
{
    public class LivroController : Controller
    {
        private LivroAplicacao _aplicacao = new LivroAplicacao();

        public async Task<ActionResult> Index()
        {
            return View(await _aplicacao.ObterTodos());
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
                await _aplicacao.Inserir(livro);                
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
            Livro livro = await _aplicacao.ObterPorId(id);
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
                await _aplicacao.Alterar(livro);                
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
            Livro livro = await _aplicacao.ObterPorId(id);
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
            await _aplicacao.Excluir(id);            
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _aplicacao.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
