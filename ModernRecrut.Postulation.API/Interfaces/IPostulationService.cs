using ModernRecrut.Postulation.API.DTO;
using ModernRecrut.Postulation.API.Models;

namespace ModernRecrut.Postulation.API.Interfaces
{
    public interface IPostulationService
    {
        public Task<PostulationDetail?> Ajouter(RequetePostulation requetePostulation);
        public Task<PostulationDetail?> ObtenirSelonId(int id);
        public Task<IEnumerable<PostulationDetail>> ObtenirTout();
        public Task<bool> Modifier(PostulationDetail item);
        public Task Supprimer(PostulationDetail item);
    }
}
