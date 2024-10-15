using ModernRecrut.MVC.Interface;
using ModernRecrut.MVC.Models;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Services.Users;
using ModernRecrut.MVC.Areas.Identity.Data;
using System.Web.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.TeamFoundation.TestManagement.WebApi;

namespace ModernRecrut.MVC.Service
{
    public class OffresEmploisServiceProxy : IOffresEmploisService
    {
        private readonly HttpClient _httpClient;
        private const string _offreEmploiApiUrl = "api/offresemplois/";
        private readonly ILogger<DocumentsServiceProxy> _logger;
        public OffresEmploisServiceProxy(HttpClient httpClient,
            ILogger<DocumentsServiceProxy> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<OffreEmploi>> ObtenirTout(string userId)
        {
            var response = await _httpClient.GetAsync(_offreEmploiApiUrl);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Erreur de Lecture d'offres d'emplois par l'utilisateur : {userId}");
                return null;
            }
            else
            {
                _logger.LogInformation($"Lecture des offres d'emplois réeussi par l'utilisateur : {userId}");
            }

            return await response.Content.ReadFromJsonAsync<IEnumerable<OffreEmploi>>();
        }
        public async Task<OffreEmploi> ObtenirSelonId(int id, string userId)
        {
            var response = await _httpClient.GetAsync(_offreEmploiApiUrl + id);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Erreur de Lecture d'un offre d'emploi avec l'id : {id} par l'utilisateur : {userId}");
                return null;
            }
            else
            {
                _logger.LogInformation($"Lecture d'un offre d'emplois réeussi avec l'id : {id} par l'utilisateur : {userId}");
            }

            return await response.Content.ReadFromJsonAsync<OffreEmploi>();
        }

        public async Task Modifier(OffreEmploi offreEmploi, string userId)
        {
            var response = await _httpClient.PutAsJsonAsync(_offreEmploiApiUrl + offreEmploi.Id, offreEmploi);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Erreur de modification d'un offre d'emploi avec l'id : {offreEmploi.Id} par l'utilisateur : {userId}");
            }
            else
            {
                _logger.LogInformation($"Modification d'un offre d'emploi réeussi avec  l'id : {offreEmploi.Id} par l'utilisateur : {userId}");
            }
        }

        public async Task Ajouter(OffreEmploi offreEmploi, string userId)
        {
            var response = await _httpClient.PostAsJsonAsync(_offreEmploiApiUrl, offreEmploi);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Erreur d'Ajout d'un offre d'emploi avec l'id : {offreEmploi.Id} par l'utilisateur : {userId}");
            }
            else
            {
                _logger.LogInformation($"Ajout d'un offre d'emploi réeussi avec l'id : {offreEmploi.Id} par l'utilisateur : {userId}");
            }
        }

        public async Task Supprimer(int id, string userId)
        {
            var response = await _httpClient.DeleteAsync(_offreEmploiApiUrl + id);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Erreur de Suppresion d'un offre d'emploi avec l'id : {id} par l'utilisateur : {userId}");
            }
            else
            {
                _logger.LogInformation($"Suppresion d'un offre d'emploi réeussi avec l'id : {id} réeussi par l'utilisateur : {userId}");
            }
        }
    }
}
