using System.ComponentModel.DataAnnotations;

namespace ModernRecrut.Favoris.API.Models
{
    public class Favori
    {
        [Key]
        public string Cle { get; set; }
        public List<OffreEmploi> Contenu { get; set; }
    }
}
