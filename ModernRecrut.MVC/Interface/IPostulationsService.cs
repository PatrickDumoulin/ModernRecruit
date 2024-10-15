using ModernRecrut.MVC.DTO;
using ModernRecrut.MVC.Models;

namespace ModernRecrut.MVC.Interface
{
    public interface IPostulationsService
    {
        Task<IEnumerable<Postulation>?> ObtenirTout();
        Task<Postulation?> ObtenirSelonId(int id);
        Task<Postulation?> Ajouter(RequetePostulation requetePostulation);
        Task Modifier(Postulation item);
        Task Supprimer(Postulation item);
    }
}
