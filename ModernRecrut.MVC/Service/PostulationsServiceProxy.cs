using ModernRecrut.MVC.DTO;
using ModernRecrut.MVC.Interface;
using ModernRecrut.MVC.Models;

namespace ModernRecrut.MVC.Service
{
    public class PostulationsServiceProxy : IPostulationsService
    {

        #region Attributs
        private readonly HttpClient _httpClient;
        private const string _postulationApiUrl = "api/Postulations/";
        private readonly ILogger<PostulationsServiceProxy> _logger;
        #endregion

        #region Constructor
        public PostulationsServiceProxy(HttpClient httpClient, ILogger<PostulationsServiceProxy> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        #endregion


        public async Task<Postulation?> Ajouter(RequetePostulation requetePostulation)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_postulationApiUrl, requetePostulation);

            if (!response.IsSuccessStatusCode)
            {
                // Journalisation
                int statusCode = (int)response.StatusCode;
                if (statusCode >= 400 && statusCode < 500)
                {
                    // Journalisation comme erreur
                    _logger.LogError($"Erreur lors de la création d'une postulation.");
                }
                else if (statusCode >= 500 && statusCode < 600)
                {
                    // Journalisation comme critique
                    _logger.LogCritical($"Erreur critique lors de la création d'une postulation.");
                }
                return null;
            }

            return await response.Content.ReadFromJsonAsync<Postulation?>();
        }

        public async Task Modifier(Postulation item)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync(_postulationApiUrl + item.Id, item);

            if (!response.IsSuccessStatusCode)
            {
                // Journalisation
                int statusCode = (int)response.StatusCode;
                if (statusCode >= 400 && statusCode < 500)
                {
                    // Journalisation comme erreur
                    _logger.LogError($"Erreur - Modification postulation ID {item.Id}");
                }
                else if (statusCode >= 500 && statusCode < 600)
                {
                    // Journalisation comme critique
                    _logger.LogCritical($"Erreur critique - Modification postulation ID {item.Id}");
                }
            }
        }

        public async Task<Postulation?> ObtenirSelonId(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(_postulationApiUrl + id);

            if (!response.IsSuccessStatusCode)
            {
                // Journalisation
                int statusCode = (int)response.StatusCode;
                if (statusCode >= 400 && statusCode < 500)
                {
                    // Journalisation comme erreur
                    _logger.LogError($"Erreur - ObtenirSelon postulation ID {id}");
                }
                else if (statusCode >= 500 && statusCode < 600)
                {
                    // Journalisation comme critique
                    _logger.LogCritical($"Erreur critique - ObtenirSelonId postulation ID {id}");
                }

                return null;
            }

            return await response.Content.ReadFromJsonAsync<Postulation>();
        }

        public async Task<IEnumerable<Postulation>?> ObtenirTout()
        {
            HttpResponseMessage response = await _httpClient.GetAsync(_postulationApiUrl);

            if (!response.IsSuccessStatusCode)
            {
                // Journalisation
                int statusCode = (int)response.StatusCode;
                if (statusCode >= 400 && statusCode < 500)
                {
                    // Journalisation comme erreur
                    _logger.LogError("Erreur lors de la requête pour obtenir toutes les postulations");
                }
                else if (statusCode >= 500 && statusCode < 600)
                {
                    // Journalisation comme critique
                    _logger.LogCritical("Erreur critique lors de la requête pour obtenir toutes les postulations");
                }
                return null;
            }

            IEnumerable<Postulation>? postulations = await response.Content.ReadFromJsonAsync<IEnumerable<Postulation>>();

            return postulations;
        }

        public async Task Supprimer(Postulation item)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync(_postulationApiUrl + item.Id);

            if (!response.IsSuccessStatusCode)
            {
                // Journalisation
                int statusCode = (int)response.StatusCode;
                if (statusCode >= 400 && statusCode < 500)
                {
                    // Journalisation comme erreur
                    _logger.LogError($"Erreur lors de la suppression de la postulation ID: {item.Id}");
                }
                else if (statusCode >= 500 && statusCode < 600)
                {
                    // Journalisation comme critique
                    _logger.LogCritical($"Erreur critique lors de la suppression de la postulation ID: {item.Id}");
                }
            }
        }
    }
}
