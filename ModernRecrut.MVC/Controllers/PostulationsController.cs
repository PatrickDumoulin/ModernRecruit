using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModernRecrut.MVC.Areas.Identity.Data;
using ModernRecrut.MVC.DTO;
using ModernRecrut.MVC.Interface;
using ModernRecrut.MVC.Models;

namespace ModernRecrut.MVC.Controllers
{
    public class PostulationsController : Controller
    {
        private readonly UserManager<Utilisateur> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IDocumentsService _documentsService;
        private readonly IPostulationsService _postulationsService;
        private readonly INoteService _notesService;
        private readonly ILogger <PostulationsController>_logger;


        public PostulationsController(UserManager<Utilisateur> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<PostulationsController> logger,
            IDocumentsService documentsService,
            IPostulationsService postulationsService,
            INoteService notesService)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
            _documentsService = documentsService;
            _postulationsService = postulationsService;
            _notesService = notesService;
        }

        // Liste Postulation (Accessible - Employé ou Admin) - Index
        // POST: PostulationsController
        [Authorize(Roles = "Admin, RH")]
        [Authorize(Roles = "Admin, Employé")]
        public async Task<ActionResult> ListePostulations()
        {
            IEnumerable<Postulation> postulations = await _postulationsService.ObtenirTout() ?? new List<Postulation>();
            // Journalisation
            _logger.LogInformation($"Visite de la page liste des postulation par l'utilisateur {User.Identity.Name}");

            return View(postulations);
        }

        // Postuler (Accessible - Candidat ou Admin) - Create
        // POST: PostulationsController/Postuler
        [Authorize(Roles = "Admin, Candidat")]
        public ActionResult Postuler(int id)
        {
            // Journalisation
            _logger.LogInformation($"Visite de la page postuler par l'utilisateur {User.Identity.Name}");

            var userId = _userManager.GetUserId(User);

            RequetePostulation requetePostulation = new RequetePostulation
            {
                IdCandidat = userId,
                OffreDEmploiId = id,
            };

            return View(requetePostulation);
        }

        // POST: PostulationsController/Postuler
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Candidat")]
        public async Task<ActionResult> Postuler(RequetePostulation requetePostulation)
        {
            // Obtenir l'utilisateur courant
            IEnumerable<DocumentModel>? documents = _documentsService.ObtenirTout(requetePostulation.IdCandidat).Result;

            // Vérifier si l'utilisateur a un CV
            bool hasCV = false;
            foreach (var document in documents) 
            {
                if (document.Type == "CV")
                {
                    hasCV = true;
                    break;
                }
            }
            if (!hasCV)
            {
                // Si aucun CV n'est trouvé, renvoyer un message d'erreur
                ViewBag.ErrorMessage = "Un CV est requis pour postuler. Veuillez télécharger d'abord un CV dans votre espace Documents.";
                ModelState.AddModelError("Error", "Un CV est requis pour postuler. Veuillez télécharger d'abord un CV dans votre espace Documents.");
            }

            // Vérifier si l'utilisateur a une lettre de motivation
            bool hasLettreMotivation = false;
            foreach (var document in documents)
            {
                if (document.Type == "LettreDeMotivation")
                {
                    hasLettreMotivation = true;
                    break;
                }
            }
            if (!hasLettreMotivation)
            {
                // Si aucune Lettre de Motivation n'est trouvé, renvoyer un message d'erreur
                ViewBag.ErrorMessage = "Une lettre de motivation est requise pour postuler. Veuillez télécharger une lettre de motivation dans votre espace Documents.";
                ModelState.AddModelError("Error", "Une lettre de motivation est requise pour postuler. Veuillez télécharger une lettre de motivation dans votre espace Documents.");
            }

            //Vérifier si la date de disponibilité de l'utilisateur est suppérieur à la date du jour et inférieur ou égale à la date du jour + 45 joursà
            if (requetePostulation.DateDisponibilite < DateTime.Now)
            {
                // Si la date de disponibilité n'est pas valide, renvoyer un message d'erreur
                ViewBag.ErrorMessage = "La date de disponibilité doit être supérieur à la date du jour";
                ModelState.AddModelError("DateDisponibilite", "La date de disponibilité doit être supérieur ou égale à la date du jour");
            }
            if (requetePostulation.DateDisponibilite > DateTime.Now.AddDays(45))
            {
                // Si la date de disponibilité n'est pas valide, renvoyer un message d'erreur
                ViewBag.ErrorMessage = "La date de disponibilité doit être inférieur ou égale à la date du jour + 45 jours";
                ModelState.AddModelError("DateDisponibilite", "La date de disponibilité doit être inférieur ou égale à la date du jour + 45 jours");
            }

            //Vérifier si la prétention salariale est supérieur a 150 000
            if (requetePostulation.PretentionSalariale > 150000)
            {
                // Si la prétention salariale n'est pas valide, renvoyer un message d'erreur
                ViewBag.ErrorMessage = "Votre prétention salariale est au-delà de nos limites";
                ModelState.AddModelError("PretentionSalariale", "Votre prétention salariale est au-delà de nos limites");
            }

            // Verifier si on a un id de candidat
            if (String.IsNullOrEmpty(requetePostulation.IdCandidat))
            {
                ModelState.AddModelError("all", "Problème lors de l'ajout de la postulation, veuillez reessayer!");
            }

            if (ModelState.IsValid)
            {              
                //Ajouter la postulation dans la base de données
                Postulation? postulation = await _postulationsService.Ajouter(requetePostulation);
                if (postulation == null)
                {
                    ModelState.AddModelError("all", "Problème lors de l'ajout de la postulation, veuillez reessayer!");
                    return View(requetePostulation);
                }
                // Journaliser l'ajout
                _logger.LogInformation($"Ajout de la postulation avec l`Id : {postulation.Id}");
                TempData["success"] = "Postulation ajoutée avec succès";
                return RedirectToAction("Index", "OffresEmplois");
            }
            return View(requetePostulation);
        }


        // GET: PostulationsController/Details/5
        [Authorize(Roles = "Admin, RH")]
        [Authorize(Roles = "Admin, Employé")]
        public async Task<ActionResult> Details(int id)
        {
            // Journalisation
            _logger.LogInformation($"Visite de la page Details par l'utilisateur {User.Identity.Name}");

            Postulation? postulation = await _postulationsService.ObtenirSelonId(id);

            if (postulation == null)
                return NotFound();

            return View(postulation);
        }

        //GET: PostulationsController/Edit/5
        [Authorize(Roles = "Admin, RH")]
        [Authorize(Roles = "Admin, Employé")]
        public async Task<ActionResult> Edit(int id)
        {
            Postulation? postulation = await _postulationsService.ObtenirSelonId(id);

            if (postulation == null)
                return NotFound();

            // Journalisation
            _logger.LogInformation($"Visite de la page Edit par l'utilisateur {postulation.IdCandidat}");

            // Verifier si la date de disponibilité est inférieure ou supérieure de 5 jours à la date du jour.
            if (postulation.DateDisponibilite > DateTime.Now.AddDays(-5) && postulation.DateDisponibilite < DateTime.Now.AddDays(5))
            {          
                return BadRequest("Il n’est pas possible de modifier ou supprimer une postulation dont la date de disponibilité est\r\ninférieure ou supérieure de 5 jours à la date du jour.");
            }

            return View(postulation);
        }

        // POST: PostulationsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, RH")]
        [Authorize(Roles = "Admin, Employé")]
        public async Task<ActionResult> Edit(Postulation postulation)
        {
            // Obtenir l'utilisateur courant
            IEnumerable<DocumentModel>? documents = _documentsService.ObtenirTout(postulation.IdCandidat).Result;

            // Vérifier si l'utilisateur a un CV
            bool hasCV = false;
            foreach (var document in documents)
            {
                if (document.Type == "CV")
                {
                    hasCV = true;
                    break;
                }
            }
            if (!hasCV)
            {
                // Si aucun CV n'est trouvé, renvoyer un message d'erreur
                ViewBag.ErrorMessage = "Un CV est requis pour postuler. Veuillez télécharger d'abord un CV dans votre espace Documents.";
                ModelState.AddModelError("Error", "Un CV est requis pour postuler. Veuillez télécharger d'abord un CV dans votre espace Documents.");
            }

            // Vérifier si l'utilisateur a une lettre de motivation
            bool hasLettreMotivation = false;
            foreach (var document in documents)
            {
                if (document.Type == "LettreDeMotivation")
                {
                    hasLettreMotivation = true;
                    break;
                }
            }
            if (!hasLettreMotivation)
            {
                // Si aucune Lettre de Motivation n'est trouvé, renvoyer un message d'erreur
                ViewBag.ErrorMessage = "Une lettre de motivation est requise pour postuler. Veuillez télécharger une lettre de motivation dans votre espace Documents.";
                ModelState.AddModelError("Error", "Une lettre de motivation est requise pour postuler. Veuillez télécharger une lettre de motivation dans votre espace Documents.");
            }

            //Vérifier si la date de disponibilité de l'utilisateur est suppérieur à la date du jour et inférieur ou égale à la date du jour + 45 joursà
            if (postulation.DateDisponibilite < DateTime.Now)
            {
                // Si la date de disponibilité n'est pas valide, renvoyer un message d'erreur
                ViewBag.ErrorMessage = "La date de disponibilité doit être supérieur à la date du jour";
                ModelState.AddModelError("DateDisponibilite", "La date de disponibilité doit être supérieur à la date du jour");
            }
            if (postulation.DateDisponibilite > DateTime.Now.AddDays(45))
            {
                // Si la date de disponibilité n'est pas valide, renvoyer un message d'erreur
                ViewBag.ErrorMessage = "La date de disponibilité doit être inférieur ou égale à la date du jour + 45 jours";
                ModelState.AddModelError("DateDisponibilite", "La date de disponibilité doit être inférieur ou égale à la date du jour + 45 jours");
            }

            //Vérifier si la prétention salariale est supérieur a 150 000
            if (postulation.PretentionSalariale > 150000)
            {
                // Si la prétention salariale n'est pas valide, renvoyer un message d'erreur
                ViewBag.ErrorMessage = "Votre prétention salariale est au-delà de nos limites";
                ModelState.AddModelError("PretentionSalariale", "Votre prétention salariale est au-delà de nos limites");
            }

            // Verifier si on a un id de candidat
            if (String.IsNullOrEmpty(postulation.IdCandidat))
            {
                ModelState.AddModelError("all", "Problème lors de l'ajout de la postulation, veuillez reessayer!");
            }

            if (ModelState.IsValid)
            {
                await _postulationsService.Modifier(postulation);
                // Journaliser la modification
                _logger.LogInformation($"Modification de la postulation avec l`Id : {postulation.Id}");
                TempData["success"] = "Postulation modifiée avec succès";
                return RedirectToAction(nameof(ListePostulations));
            }

            return View(postulation);
        }

        // GET: PostulationsController/Delete/5
        [Authorize(Roles = "Admin, RH")]
        [Authorize(Roles = "Admin, Employé")]
        public async Task<ActionResult> Delete(int id)
        {
            Postulation? postulation = await _postulationsService.ObtenirSelonId(id);

            if (postulation == null)
                return NotFound();

            // Verifier si la date de disponibilité est inférieure ou supérieure de 5 jours à la date du jour.
            if (postulation.DateDisponibilite > DateTime.Now.AddDays(-5) && postulation.DateDisponibilite < DateTime.Now.AddDays(5))
            {
                return BadRequest("Il n’est pas possible de modifier ou supprimer une postulation dont la date de disponibilité est\r\ninférieure ou supérieure de 5 jours à la date du jour.");
            }

            return View(postulation);
        }

        // POST: PostulationsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, RH")]
        [Authorize(Roles = "Admin, Employé")]
        public async Task<ActionResult> Delete(Postulation postulation)
        {
            await _postulationsService.Supprimer(postulation);
            // Journaliser la modification
            _logger.LogInformation($"Suppression de la postulation avec l'id: {postulation.Id}");
            TempData["success"] = "Postulation supprimée avec succès";
            return RedirectToAction(nameof(ListePostulations));
        }

        // Notes (Accessible RH ou Admin) - Create (Note)
        // Bouton Ajouter note pour chaque object de listePostulation
        // GET: PostulationsController/Notes
        [Authorize(Roles = "Admin, RH")]
        [Authorize(Roles = "Admin, Employé")]
        public async Task<ActionResult> Noter(int id)
        {           
            // Journalisation
            _logger.LogInformation($"Visite de la page notes() par l'utilisateur {User.Identity.Name}");
         
            RequeteNote requeteNote = new RequeteNote
            {
                IdPostulation = id,
                Note = "-Note-",
                NomEmeteur = User.Identity.Name
            };

            return View(requeteNote);
        }

        // POST: PostulationsController/Notes
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, RH")]
        [Authorize(Roles = "Admin, Employé")]
        public async Task<ActionResult> Noter(RequeteNote requeteNote)
        {
            Postulation postulation = await _postulationsService.ObtenirSelonId(requeteNote.IdPostulation);

            if(postulation.DateDisponibilite > DateTime.Now)
            {
                ModelState.AddModelError("all", "Vous ne pouvez pas encore ajouter une note attendez apres la rencontre!");
            }

            if (ModelState.IsValid)
            {
                NoteDetail? note = await _notesService.Ajouter(requeteNote);

                if(note == null)
                {
                    ModelState.AddModelError("all", "Problème lors de l'ajout de la note, veuillez reessayer!");
                    return View(requeteNote);
                }

                return RedirectToAction(nameof(ListePostulations));
            }
            return View(requeteNote);
        }
    }
}
