using Microsoft.VisualStudio.Services.Users;
using ModernRecrut.MVC.Models;

namespace ModernRecrut.MVC.Interface
{
    public interface IDocumentsService
    {
        Task<IEnumerable<DocumentModel>> ObtenirTout(string userId);
        Task<DocumentModel> ObtenirSelonChemin(string userId, string chemin);
        Task Ajouter(string userId, string documentType, IFormFile file);
        Task Supprimer(string userId, string chemain);
    }
}
