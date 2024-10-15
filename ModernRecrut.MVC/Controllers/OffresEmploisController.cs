using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModernRecrut.MVC.Areas.Identity.Data;
using ModernRecrut.MVC.Interface;
using ModernRecrut.MVC.Models;

namespace ModernRecrut.MVC.Controllers
{
    public class OffresEmploisController : Controller
    {
        private readonly IOffresEmploisService _offresEmploisService;
        private readonly ILogger<OffresEmploisController> _logger;
        private readonly UserManager<Utilisateur> _userManager;
        public OffresEmploisController(IOffresEmploisService offresEmploisService, 
            ILogger<OffresEmploisController> logger,
           UserManager<Utilisateur> userManager)
        {
            _offresEmploisService = offresEmploisService;
            _logger = logger;
            _userManager = userManager;
        }

        // GET: OffresEmploisController
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
            var offresEmplois = await _offresEmploisService.ObtenirTout(user.Id);

            offresEmplois = offresEmplois.Where(e => e.DateDebut <= DateTime.UtcNow).Where(e => e.DateFin >= DateTime.UtcNow);

            //Historique d'emprunts ou emprunts en cours
            if (!String.IsNullOrEmpty(searchString))
            {
                offresEmplois = offresEmplois.Where(e => e.Poste.ToLower().Contains(searchString.ToLower())
                                          || e.Nom.ToLower().Contains(searchString.ToLower()));
            }

            _logger.LogInformation($"Lecture de {offresEmplois.Count()} offres d'emplois");

            return View(offresEmplois);
        }
    

        // GET: OffresEmploisController/Details/5
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
            var offreEmploi = await _offresEmploisService.ObtenirSelonId(id, user.Id);

            if (offreEmploi == null)
            {
                return NotFound();
            }

            return View(offreEmploi);
        }

        // GET: OffresEmploisController/Create
        [Authorize(Roles = "Admin, Employé")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: OffresEmploisController/Create
        [Authorize(Roles = "Admin, Employé")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(OffreEmploi offreEmploi)
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
            if (ModelState.IsValid)
            {
                await _offresEmploisService.Ajouter(offreEmploi, user.Id);
                TempData["success"] = "Offre d'emploi crée avec succès";
                return RedirectToAction(nameof(Index));
            }
            return View(offreEmploi);
        }

        // GET: OffresEmploisController/Edit/5
        [Authorize(Roles = "Admin, Employé")]
        public async Task<ActionResult> Edit(int id)
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
            var offreEmploi = await _offresEmploisService.ObtenirSelonId(id, user.Id);

            if (offreEmploi == null)
            {
                return NotFound();
            }

            return View(offreEmploi);
        }

        // POST: OffresEmploisController/Edit/5
        [Authorize(Roles = "Admin, Employé")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(OffreEmploi offreEmploi)
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
            if (ModelState.IsValid)
            {
                await _offresEmploisService.Modifier(offreEmploi, user.Id);
                TempData["success"] = "Offre d'emploi modifié avec succès";
                return RedirectToAction(nameof(Index));
            }
            return View(offreEmploi);
        }

        // GET: OffresEmploisController/Delete/5
        [Authorize(Roles = "Admin, Employé")]
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
            var offreEmploi = await _offresEmploisService.ObtenirSelonId(id, user.Id);

            if (offreEmploi == null)
            {
                return NotFound();
            }

            return View(offreEmploi);
        }

        // POST: OffresEmploisController/Delete/5
        [Authorize(Roles = "Admin, Employé")]
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
            await _offresEmploisService.Supprimer(offreEmploi.Id, user.Id);
            TempData["success"] = "Offre d'emploi supprimé avec succès";

            return RedirectToAction(nameof(Index));
        }
    }
}
