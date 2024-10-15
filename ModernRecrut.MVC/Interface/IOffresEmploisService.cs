using Microsoft.VisualStudio.Services.Users;
using ModernRecrut.MVC.Models;

namespace ModernRecrut.MVC.Interface
{
    public interface IOffresEmploisService
    {
        Task<IEnumerable<OffreEmploi>> ObtenirTout(string userId);
        Task<OffreEmploi> ObtenirSelonId(int id, string userId);
        Task Ajouter(OffreEmploi offreEmploi, string userId);
        Task Supprimer(int id, string userId);
        Task Modifier(OffreEmploi offreEmploi, string userId);
    }
}
