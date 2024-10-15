using System.ComponentModel.DataAnnotations;

namespace ModernRecrut.Favoris.API.Models
{
    public class OffreEmploi
    {
        [Key]
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Poste { get; set; }
        public string Description { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
    }
}
