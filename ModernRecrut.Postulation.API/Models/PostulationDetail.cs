using ModernRecrut.Postulation.API.Entites;

namespace ModernRecrut.Postulation.API.Models
{
    public class PostulationDetail : BaseEntity
    {       
        public string IdCandidat { get; set; }
        public int OffreDEmploiId { get; set; }
        public decimal PretentionSalariale { get; set; }
        public DateTime DateDisponibilite { get; set; }
    }
}
