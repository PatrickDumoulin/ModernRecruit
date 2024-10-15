using System.ComponentModel.DataAnnotations;

namespace ModernRecrut.MVC.Models
{
    public class Favori
    {
        [Key]
        public string Cle { get; set; }

        public List<OffreEmploi> Contenu { get; set; }
    }
}
