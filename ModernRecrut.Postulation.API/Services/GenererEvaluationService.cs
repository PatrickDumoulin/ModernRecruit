using ModernRecrut.Postulation.API.DTO;
using ModernRecrut.Postulation.API.Interfaces;
using ModernRecrut.Postulation.API.Models;

namespace ModernRecrut.Postulation.API.Services
{
    public class GenererEvaluationService : IGenererEvaluationService
    {
        public readonly INoteService _INoteService;
        public GenererEvaluationService(INoteService noteService)
        {
            _INoteService = noteService;
        }

        public async Task<NoteDetail?> GenererNote(PostulationDetail postulation)
        {
            if (postulation == null)
            {
                // Log an error or throw an exception
                return null;
            }

            string noteGenere = "";

            if(postulation.PretentionSalariale < 20000)
            {
                noteGenere = "Salaire inférieur à la norme";
            }
            else if (postulation.PretentionSalariale >= 20000 && postulation.PretentionSalariale <= 39999)
            {
                noteGenere = "Salaire proche mais inférieur à la norme";
            }
            else if (postulation.PretentionSalariale >= 40000 && postulation.PretentionSalariale <= 79999)
            {
                noteGenere = "Salaire dans la norme";
            }
            else if (postulation.PretentionSalariale >= 80000 && postulation.PretentionSalariale <= 99999)
            {
                noteGenere = "Salaire proche mais supérieur à la norme";
            }
            else if (postulation.PretentionSalariale >= 100000)
            {
                noteGenere = "Salaire supérieur à la norme";
            }

            RequeteNote note = new RequeteNote
            {
                IdPostulation = postulation.Id,
                Note = noteGenere,
                NomEmeteur = "ApplicationPostulation",
            };

            return await _INoteService.Ajouter(note);
        }
    }
}
