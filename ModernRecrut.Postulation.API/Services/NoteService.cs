using ModernRecrut.Postulation.API.DTO;
using ModernRecrut.Postulation.API.Interfaces;
using ModernRecrut.Postulation.API.Models;

namespace ModernRecrut.Postulation.API.Services
{
    public class NoteService : INoteService
    {
        public readonly IAsyncRepository<NoteDetail> _noteDetailRepository;

        public NoteService(IAsyncRepository<NoteDetail> noteDetailRepository)
        {
            _noteDetailRepository = noteDetailRepository;
        }
        public async Task<NoteDetail?> Ajouter(RequeteNote requetenote)
        {
            //if (!valider)
            //    return null;

            NoteDetail note = new NoteDetail
            {
                IdPostulation = requetenote.IdPostulation,
                Note = requetenote.Note,
                NomEmeteur = requetenote.NomEmeteur,
            };

            return await _noteDetailRepository.AddAsync(note);
        }

        public async Task<bool> Modifier(NoteDetail noteDetail)
        {
            //if (!valider)
            //    return false;

            await _noteDetailRepository.EditAsync(noteDetail);
            return true;
        }

        public Task<NoteDetail?> ObtenirSelonId(int id)
        {
            return _noteDetailRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<NoteDetail>> ObtenirTout()
        {
            return await _noteDetailRepository.ListAsync();
        }

        public Task Supprimer(NoteDetail noteDetail)
        {
            return _noteDetailRepository.DeleteAsync(noteDetail);
        }
    }
}
