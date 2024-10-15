using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Services.Users;
using ModernRecrut.MVC.Areas.Identity.Data;
using ModernRecrut.MVC.Models;
using ModernRecrut.MVC.Service;

namespace ModernRecrut.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly UserManager<Utilisateur> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<DocumentsServiceProxy> _logger;

        public RolesController(UserManager<Utilisateur> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<DocumentsServiceProxy> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
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
            _logger.LogError($"Obtention des roles par l'utilisateur : {user.Id}");

            return View(await _roleManager.Roles.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(IdentityRole identityRole)
        {
            var user = await _userManager.GetUserAsync(User);

            if (await _roleManager.Roles.AnyAsync(r => r.Name.ToLower() == identityRole.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "Le role existe déjà");
            }

            if (ModelState.IsValid)
            {
                var creation = await _roleManager.CreateAsync(identityRole);
                if (!creation.Succeeded)
                {
                    _logger.LogError($"Erreur lors de la création du role : {identityRole} par l'utilisateur : {user.Id}");
                }
                else
                {
                    _logger.LogInformation($"Création réussi du role : {identityRole} par l'utilisateur : {user.Id}");
                    TempData["success"] = "Role crée avec succès";
                }
                return RedirectToAction("Index");
            }

            return View(identityRole);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {          
            return View(await _roleManager.FindByIdAsync(id));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(IdentityRole identityRole)
        {
            if (await _roleManager.Roles.AnyAsync(r => r.Name.ToLower() == identityRole.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "Le role existe déjà");
            }

            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(identityRole.Id);
                var roleNomSet = await _roleManager.SetRoleNameAsync(role, identityRole.Name);
                if (!roleNomSet.Succeeded)
                {
                    _logger.LogError($"Erreur lors de la modification du nom du role: {role.Name} a : {identityRole.Name}");
                }
                else
                {
                    TempData["success"] = "Role modifié avec succès";
                    _logger.LogInformation($"Modification réussi du nom du role: {role.Name} a : {identityRole.Name}");
                }
                var roleUpdate = await _roleManager.UpdateAsync(role);
                if (!roleUpdate.Succeeded)
                {
                    _logger.LogError($"Erreur lors de la modification du role: {identityRole.Name}");
                }
                else
                {
                    _logger.LogInformation($"Modification réussi du role: {identityRole.Name}");
                }

                return RedirectToAction("Index");
            }

            return View(identityRole);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Assigner()
        {
            await RemplirSelectList();
            
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Assigner(UserRoleViewModel userRoleViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(userRoleViewModel.UserId);
                if (user == null)
                {
                    _logger.LogError($"Erreur lors de l'obtetion de l'utilisateur avec l'id : {userRoleViewModel.UserId}");
                }
                else
                {
                    _logger.LogInformation($"Obtetion réussi de l'utilisateur avec l'id : {userRoleViewModel.UserId}");
                }
                var roleAssigne = await _userManager.AddToRoleAsync(user, userRoleViewModel.RoleName);
                if (!roleAssigne.Succeeded)
                {
                    _logger.LogError($"Erreur lors de l'assignation du role: {userRoleViewModel.RoleName}");
                }
                else
                {
                    TempData["success"] = "Role assigné avec succès";
                    _logger.LogInformation($"Assignation réussi du role: {userRoleViewModel.RoleName}");
                }
                return RedirectToAction("Index");
            }

            await RemplirSelectList();
            return View(userRoleViewModel);
        }

        private async Task RemplirSelectList()
        {

            ViewBag.Users = new SelectList(await _userManager.Users.ToListAsync(), "Id", "UserName");

            ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
        }
    }
}