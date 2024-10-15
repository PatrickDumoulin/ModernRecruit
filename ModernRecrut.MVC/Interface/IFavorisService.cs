using ModernRecrut.MVC.Models;

namespace ModernRecrut.MVC.Interface
{
    public interface IFavorisService
    {
        Task<IEnumerable<OffreEmploi>> ObtenirTout(string cle, string userId);
        Task<OffreEmploi> ObtenirSelonId(string cle, int id, string userId);
        Task Ajouter(string cle, OffreEmploi offreEmploi, string userId);
        Task Supprimer(string cle, int id, string userId);
 
    }
}
