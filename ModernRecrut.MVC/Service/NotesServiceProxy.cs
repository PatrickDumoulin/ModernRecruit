using Microsoft.Azure.Management.ResourceManager.Fluent.Core.DAG;
using ModernRecrut.MVC.DTO;
using ModernRecrut.MVC.Interface;
using ModernRecrut.MVC.Models;

namespace ModernRecrut.MVC.Service
{
    public class NotesServiceProxy : INoteService
    {
        #region Attributs
        private readonly HttpClient _httpClient;
        private const string _noteApiUrl = "api/Notes/";
        private readonly ILogger<NotesServiceProxy> _logger;
        #endregion

        #region Constructor
        public NotesServiceProxy(HttpClient httpClient, ILogger<NotesServiceProxy> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        #endregion

        public async Task<NoteDetail?> Ajouter(RequeteNote requeteNote)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_noteApiUrl, requeteNote);

            if (!response.IsSuccessStatusCode)
            {
                // Journalisation
                int statusCode = (int)response.StatusCode;
                if (statusCode >= 400 && statusCode < 500)
                {
                    // Journalisation comme erreur
                    _logger.LogError($"Erreur lors de la création d'une note.");
                }
                else if (statusCode >= 500 && statusCode < 600)
                {
                    // Journalisation comme critique
                    _logger.LogCritical($"Erreur critique lors de la création d'une note.");
                }
                return null;
            }

            return await response.Content.ReadFromJsonAsync<NoteDetail?>();
        }

    }
}
