using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModernRecrut.MVC.Areas.Identity.Data;
using ModernRecrut.MVC.Interface;
using ModernRecrut.MVC.Models;
using System.Web.Http;

namespace ModernRecrut.MVC.Controllers
{
    [Authorize(Roles = "Candidat, Admin")]
    public class DocumentsController : Controller
    {
        private readonly IDocumentsService _documentsService;
        private readonly ILogger<DocumentsController> _logger;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<Utilisateur> _userManager;

        public DocumentsController(IWebHostEnvironment hostEnvironment, IDocumentsService documentsService,
            ILogger<DocumentsController> logger, UserManager<Utilisateur> userManager)
        {
            _documentsService = documentsService;
            _logger = logger;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
        }

        // GET: DocumentsController
        [Authorize(Roles = "Candidat, Admin")]
        public async Task<ActionResult> Index()
        {           
            var user = await _userManager.GetUserAsync(User);
            
            if(user == null)
            {
                _logger.LogInformation("Utilisateur non authentifié, redirection vers la page de connexion.");
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            
            var documents = await _documentsService.ObtenirTout(user.Id);

            return View(documents);
        }

        // GET: DocumentsController/Create
        [Authorize(Roles = "Candidat, Admin")]
        public async Task<ActionResult> Create()
        {
            return View();
        }

        // POST: DocumentsController/Create
        [Authorize(Roles = "Candidat, Admin")]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(DocumentModel document, IFormFile file)
        {
            var user = await _userManager.GetUserAsync(User);

            var allowedExtensions = new[] { ".doc", ".docx", ".pdf" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError(String.Empty, "Les extension autorisés sont .doc , .pdf et .docx");
                _logger.LogError($"Erreur de création de document extension non-autorisés par l'utilisateur : {user.Id}");
                return View(document);
            }

            if (document.Type != null)
            {
                await _documentsService.Ajouter(user.Id, document.Type, file);
                _logger.LogInformation($"Document ajouté pour l'utilisateur : {user.Id}");
                TempData["success"] = "Document ajouté avec succès";
                return RedirectToAction(nameof(Index));
            }

            return View(document);
        }

        // GET: DocumentsController/Delete/5
        [Authorize(Roles = "Candidat, Admin")]
        public async Task<ActionResult> Delete(string chemin)
        {
            var user = await _userManager.GetUserAsync(User);

            if (chemin == null)
            {
                return NotFound();
            }

            var document = await _documentsService.ObtenirSelonChemin(user.Id, chemin);

            if (document == null)
            {
                _logger.LogWarning($"Document non trouvé pour le chemin : {chemin} et l'utilisateur : {user.Id}");
                return NotFound();
            }

            return View(document);
        }

        // POST: DocumentsController/Delete/5
        [Authorize(Roles = "Candidat, Admin")]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public async Task<ActionResult> Delete(DocumentModel document)
        {
            var user = await _userManager.GetUserAsync(User);
            await _documentsService.Supprimer(user.Id, document.Chemin);
            _logger.LogInformation($"Document supprimé : {document.Chemin} pour l'utilisateur : {user.Id}");
            TempData["success"] = "Document supprimé avec succès";
            return RedirectToAction(nameof(Index));
        }
    }
}
