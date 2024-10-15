using ModernRecrut.Postulation.API.Entites;

namespace ModernRecrut.Postulation.API.Models
{
    public class NoteDetail : BaseEntity
    {
        public int IdPostulation { get; set; }
        public string Note { get; set; }
        public string NomEmeteur { get; set; }

    }
}
