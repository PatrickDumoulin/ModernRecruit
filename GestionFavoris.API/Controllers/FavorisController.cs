using ModernRecrut.Favoris.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ModernRecrut.Favoris.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavorisController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<FavorisController> _logger;

        public FavorisController(IMemoryCache cache, ILogger<FavorisController> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        [HttpPost("{cle}")]
        public IActionResult AjouterAuxFavoris(string cle, OffreEmploi offreEmploi)
        {

            // Options de cache
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSize(1)
                .SetSlidingExpiration(TimeSpan.FromHours(6))
                .SetAbsoluteExpiration(TimeSpan.FromHours(24));

            // Récupérer les favoris actuels du cache ou créer un nouveau si inexistant
            if (!_cache.TryGetValue(cle, out Favori favorisActuels))
            {
                favorisActuels = new Favori
                {
                    Cle = cle,
                    Contenu = new List<OffreEmploi>()
                };
            }

            if(favorisActuels.Contenu.Where(o => o.Id == offreEmploi.Id).Count() == 0)
            {
                // Ajouter l'offre d'emploi aux favoris de l'utilisateur
                favorisActuels.Contenu.Add(offreEmploi);
            }        

            // Mise à jour du cache avec les nouveaux favoris de l'utilisateur
            _cache.Set(cle, favorisActuels, cacheEntryOptions);

            _logger.LogInformation("Ajout de l'offre d'emploi avec ID {OffreEmploiId} aux favoris.", offreEmploi.Id);

            return Ok();
        }

        [HttpDelete("{cle}/{id}")]
        public IActionResult SupprimerUnFavoris(string cle, int id)
        {

            if (_cache.TryGetValue(cle, out Favori favorisActuels))
            {

                favorisActuels.Contenu.RemoveAll(o => o.Id == id);


                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSize(1) // 
                    .SetSlidingExpiration(TimeSpan.FromHours(6))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(24));

                // Mise à jour du cache
                _cache.Set(cle, favorisActuels, cacheEntryOptions);

                _logger.LogInformation("Suppression de l'offre d'emploi avec ID {OffreEmploiId} des favoris.", id);

                return Ok();
            }

            return NotFound();
        }

        // Récupérer les favoris d'un utilisateur
        [HttpGet("{cle}")]
        public IActionResult GetFavoris(string cle)
        {

            // Essayez de récupérer les favoris du cache
            
            if (_cache.TryGetValue(cle, out Favori favorisActuels))
            {
                if (favorisActuels.Contenu == null || !favorisActuels.Contenu.Any())
                {
                    return Ok(Enumerable.Empty<OffreEmploi>());
                }

                return Ok(favorisActuels.Contenu);
            }
            
            else
            {

                return Ok(Enumerable.Empty<OffreEmploi>());
            }
        }

        // Récupérer les favoris d'un utilisateur par Id
        [HttpGet("{cle}/{id}")]
        public IActionResult GetFavorisSelonId(string cle, int id)
        {

            // Essayez de récupérer les favoris du cache

            if (_cache.TryGetValue(cle, out Favori favorisActuels))
            {
                if (favorisActuels.Contenu == null || !favorisActuels.Contenu.Any())
                {
                    return NotFound("Les favoris sont vides");
                }
                var favoriActuel = favorisActuels.Contenu.FirstOrDefault(x => x.Id == id);

                if (favoriActuel == null)
                {
                    return NotFound("Ce favoris n'existe pas");
                }
                return Ok(favoriActuel);
            }

            else
            {

                return NotFound("Aucun favori trouvé pour cette adresse IP.");
            }
        }
    }
}
