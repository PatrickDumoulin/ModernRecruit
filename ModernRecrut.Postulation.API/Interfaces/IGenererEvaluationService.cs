using ModernRecrut.Postulation.API.DTO;
using ModernRecrut.Postulation.API.Models;

namespace ModernRecrut.Postulation.API.Interfaces
{
    public interface IGenererEvaluationService
    {
        public Task<NoteDetail?> GenererNote(PostulationDetail postulation);
    }
}
