using ModernRecrut.Postulation.API.DTO;
using ModernRecrut.Postulation.API.Models;

namespace ModernRecrut.Postulation.API.Interfaces
{
    public interface INoteService
    {
        public Task<NoteDetail?> Ajouter(RequeteNote requetenote);
        public Task<NoteDetail?> ObtenirSelonId(int id);
        public Task<IEnumerable<NoteDetail>> ObtenirTout();
        public Task<bool> Modifier(NoteDetail item);
        public Task Supprimer(NoteDetail item);
    }
}
