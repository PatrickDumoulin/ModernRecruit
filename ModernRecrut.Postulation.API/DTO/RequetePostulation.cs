namespace ModernRecrut.Postulation.API.DTO
{
    public class RequetePostulation
    {
        public string IdCandidat { get; set; }
        public int OffreDEmploiId { get; set; }
        public decimal PretentionSalariale { get; set; }
        public DateTime DateDisponibilite { get; set; }
    }
}
