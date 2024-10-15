using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModernRecrut.MVC.Areas.Identity.Data;
using ModernRecrut.MVC.Interface;
using ModernRecrut.MVC.Models;
using System.Security.Claims;


namespace ModernRecrut.MVC.Controllers
{
    public class FavorisController : Controller
    {
        private readonly IFavorisService _favorisService;
        private readonly IOffresEmploisService _offresEmploisService;
        private readonly UserManager<Utilisateur> _userManager;
        private readonly ILogger<OffresEmploisController> _logger;
        public FavorisController(IFavorisService favorisService, 
            IOffresEmploisService offresEmploisService, 
            UserManager<Utilisateur> userManager,
            ILogger<OffresEmploisController> logger)
        {
            _favorisService = favorisService;
            _offresEmploisService = offresEmploisService;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: FavorisController
        public async Task<ActionResult> Index(string searchString)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogInformation("Utilisateur non authentifié");
                user = new Utilisateur
                {
                    Id = "Anonyme",
                    Nom = "Anonyme",
                    Prenom = "Anonyme",
                    Type = "Anonyme",
                };
            }
            var cleIp = HttpContext.Connection.RemoteIpAddress.ToString();

            // Récupérer tous les favoris de l'utilisateur
            var favoris = await _favorisService.ObtenirTout(cleIp, user.Id);

            // Appliquer le filtre de recherche si la chaîne de recherche n'est pas vide
            if (!String.IsNullOrEmpty(searchString))
            {
                favoris = favoris.Where(e => e.Poste.ToLower().Contains(searchString.ToLower())
                                          || e.Nom.ToLower().Contains(searchString.ToLower()));
            }

            _logger.LogInformation($"Lecture de {favoris.Count()} favoris après filtrage");

            return View(favoris);
        }


        // GET: FavorisController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogInformation("Utilisateur non authentifié");
                user = new Utilisateur
                {
                    Id = "Anonyme",
                    Nom = "Anonyme",
                    Prenom = "Anonyme",
                    Type = "Anonyme",
                };
            }
            var cleIp = HttpContext.Connection.RemoteIpAddress.ToString();

            var favori = await _favorisService.ObtenirSelonId(cleIp, id, user.Id);

            if (favori == null)
            {
                return NotFound();
            }

            return View(favori);
        }

        // GET: FavorisController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogInformation("Utilisateur non authentifié");
                user = new Utilisateur
                {
                    Id = "Anonyme",
                    Nom = "Anonyme",
                    Prenom = "Anonyme",
                    Type = "Anonyme",
                };
            }
            var cleIp = HttpContext.Connection.RemoteIpAddress.ToString();

            var favori = await _favorisService.ObtenirSelonId(cleIp, id, user.Id);

            if (favori == null)
            {
                return NotFound();
            }

            return View(favori);
        }

        // POST: FavorisController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, OffreEmploi offreEmploi)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogInformation("Utilisateur non authentifié");
                user = new Utilisateur
                {
                    Id = "Anonyme",
                    Nom = "Anonyme",
                    Prenom = "Anonyme",
                    Type = "Anonyme",
                };
            }
            var cleIp = HttpContext.Connection.RemoteIpAddress.ToString();

            await _favorisService.Supprimer(cleIp, offreEmploi.Id, user.Id);
            TempData["success"] = "Favoris supprimé avec succès";

            return RedirectToAction(nameof(Index));
        }

        // GET: FavorisController/Create
        public async Task<ActionResult> Create(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogInformation("Utilisateur non authentifié");
                user = new Utilisateur
                {
                    Id = "Anonyme",
                    Nom = "Anonyme",
                    Prenom = "Anonyme",
                    Type = "Anonyme",
                };
            }
            var cleIp = HttpContext.Connection.RemoteIpAddress.ToString();
            var favori = await _offresEmploisService.ObtenirSelonId(id, user.Id);

            await _favorisService.Ajouter(cleIp, favori, user.Id);
            TempData["success"] = "Favoris ajouté avec succès";

            if (favori == null)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
