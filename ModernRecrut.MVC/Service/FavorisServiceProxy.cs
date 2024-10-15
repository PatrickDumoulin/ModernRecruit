using Microsoft.Extensions.Logging;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using ModernRecrut.MVC.Interface;
using ModernRecrut.MVC.Models;
using System.Net.Http;

namespace ModernRecrut.MVC.Service
{
    public class FavorisServiceProxy : IFavorisService
    {
        private readonly HttpClient _httpClient;
        private const string _favorisApiUrl = "api/favoris/";
        private readonly ILogger<DocumentsServiceProxy> _logger;
        public FavorisServiceProxy(HttpClient httpClient, 
            ILogger<DocumentsServiceProxy> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<OffreEmploi>> ObtenirTout(string cle, string userId)
        {
            var response = await _httpClient.GetAsync(_favorisApiUrl + cle);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Erreur de Lecture de favoris de : {cle} par l'utilisateur : {userId}");
                return null;
            }
            else
            {
                _logger.LogInformation($"Lecture des favoris de : {cle} réeussi par l'utilisateur : {userId}");
            }

            return await response.Content.ReadFromJsonAsync<IEnumerable<OffreEmploi>>();
        }

        public async Task<OffreEmploi> ObtenirSelonId(string cle, int id, string userId)
        {
            var response = await _httpClient.GetAsync(_favorisApiUrl + cle + "/" + id);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Erreur de Lecture de favoris de : {cle} avec le chemin : {id} par l'utilisateur : {userId}");
                return null;
            }
            else
            {
                _logger.LogInformation($"Lecture d'un favoris de : {cle} réeussi avec le chemin : {id} par l'utilisateur : {userId}");
            }

            return await response.Content.ReadFromJsonAsync<OffreEmploi>();
        }

        public async Task Ajouter(string cle, OffreEmploi offreEmploi, string userId)
        {
            var response = await _httpClient.PostAsJsonAsync(_favorisApiUrl + cle, offreEmploi);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Erreur d'Ajout de l'offres d'emplois au favoris de : {cle} avec l'id : {offreEmploi.Id} par l'utilisateur : {userId}");
            }
            else
            {
                _logger.LogInformation($"Ajout d'un offres d'emplois au favoris de : {cle} réeussi avec l'id : {offreEmploi.Id} par l'utilisateur : {userId}");
            }
        }

        public async Task Supprimer(string cle, int id, string userId)
        {
            var response = await _httpClient.DeleteAsync(_favorisApiUrl + cle + "/" + id);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Erreur de Suppresion d'un offres d'emplois des favoris de : {cle} avec l'id : {id} par l'utilisateur : {userId}");
            }
            else
            {
                _logger.LogInformation($"Suppresion d'un offres d'emplois des favoris de : {cle} réeussi avec l'id : {id} réeussi par l'utilisateur : {userId}");
            }
        }
   
    }
}
