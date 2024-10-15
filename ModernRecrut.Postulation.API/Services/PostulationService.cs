using ModernRecrut.Postulation.API.DTO;
using ModernRecrut.Postulation.API.Interfaces;
using ModernRecrut.Postulation.API.Models;

namespace ModernRecrut.Postulation.API.Services
{
    public class PostulationService : IPostulationService
    {
        public readonly IAsyncRepository<PostulationDetail> _postulationDetailRepository;
        public readonly IGenererEvaluationService _genererEvaluation;
        public PostulationService(IAsyncRepository<PostulationDetail> postulationDetailRepository, IGenererEvaluationService genererEvaluation) 
        {
            _postulationDetailRepository = postulationDetailRepository;
            _genererEvaluation = genererEvaluation;
        }

        public async Task<PostulationDetail?> Ajouter(RequetePostulation requetePostulation)
        {
            if (requetePostulation == null)
                return null;
         
            PostulationDetail postulation = new PostulationDetail
            {
                IdCandidat = requetePostulation.IdCandidat,
                OffreDEmploiId = requetePostulation.OffreDEmploiId,
                PretentionSalariale = requetePostulation.PretentionSalariale,
                DateDisponibilite = requetePostulation.DateDisponibilite,
            };

            PostulationDetail newPostulation = await _postulationDetailRepository.AddAsync(postulation);

            //Appeler GenererEvaluation pour ajouter une note
            NoteDetail? note = await _genererEvaluation.GenererNote(newPostulation);
            if (note == null)
                return null;

            return newPostulation;
        }

        public async Task<bool> Modifier(PostulationDetail postulationDetail)
        {
            if (postulationDetail == null)
                return false;

            await _postulationDetailRepository.EditAsync(postulationDetail);
            return true;
        }

        public  Task<PostulationDetail?> ObtenirSelonId(int id)
        {
            return _postulationDetailRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<PostulationDetail>> ObtenirTout()
        {        
            return await _postulationDetailRepository.ListAsync();
        }

        public Task Supprimer(PostulationDetail postulationDetail)
        {
            return _postulationDetailRepository.DeleteAsync(postulationDetail);
        }
    }
}
