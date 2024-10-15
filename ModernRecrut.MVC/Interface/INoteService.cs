using ModernRecrut.MVC.DTO;
using ModernRecrut.MVC.Models;

namespace ModernRecrut.MVC.Interface
{
    public interface INoteService
    {
        Task<NoteDetail?> Ajouter(RequeteNote requeteNote);
    }
}
