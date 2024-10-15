using Microsoft.TeamFoundation.TestManagement.WebApi;
using ModernRecrut.MVC.Interface;
using ModernRecrut.MVC.Models;
using NuGet.Protocol;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ModernRecrut.MVC.Service
{
    public class DocumentsServiceProxy : IDocumentsService
    {
        private readonly HttpClient _httpClient;
        private const string _documentsApiUrl = "api/documents/";
        private readonly ILogger<DocumentsServiceProxy> _logger;

        public DocumentsServiceProxy(HttpClient httpClient, ILogger<DocumentsServiceProxy> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<DocumentModel>> ObtenirTout(string userId)
        {
            var response = await _httpClient.GetAsync(_documentsApiUrl + userId);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Erreur de Lecture de documents par l'utilisateur : {userId}");
                return null;
            }
            else
            {
                _logger.LogInformation($"Lecture des documents réeussi par l'utilisateur : {userId}");
            }

            return await response.Content.ReadFromJsonAsync<IEnumerable<DocumentModel>>();
        }

        public async Task<DocumentModel> ObtenirSelonChemin(string userId, string chemin)
        {
            var response = await _httpClient.GetAsync(_documentsApiUrl + userId + "/" + chemin);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Erreur de Lecture de document avec le chemin : {chemin} par l'utilisateur : {userId}");
                return null;
            }
            else
            {
                _logger.LogInformation($"Lecture d'un document réeussi avec le chemin : {chemin} par l'utilisateur : {userId}");
            }

            return await response.Content.ReadFromJsonAsync<DocumentModel>();
        }

        // DocumentsServiceProxy Ajouter
        public async Task Ajouter(string userId, string documentType, IFormFile file)
        {
            using var content = new MultipartFormDataContent();
            using var fileStream = file.OpenReadStream();
            using var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            content.Add(fileContent, "file", file.FileName);

            var response = await _httpClient.PostAsync(_documentsApiUrl + userId + "/" + documentType, content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Erreur d'Ajout de document avec le chemin : {file.FileName} par l'utilisateur : {userId}");
            }
            else
            {
                _logger.LogInformation($"Ajout d'un document réeussi avec le chemin : {file.FileName} par l'utilisateur : {userId}");
            }
        }

        public async Task Supprimer(string userId, string chemin)
        {
            var response = await _httpClient.DeleteAsync(_documentsApiUrl + chemin);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Erreur de Suppresion de documents avec le chemin : {chemin} par l'utilisateur : {userId}");
            }
            else
            {
                _logger.LogInformation($"Suppresion d'un document réeussi avec le chemin : {chemin} par l'utilisateur : {userId}");
            }
        }
    }
}
