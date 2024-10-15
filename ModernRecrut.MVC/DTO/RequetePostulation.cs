using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ModernRecrut.MVC.DTO
{
    public class RequetePostulation
    {
        [DisplayName("Id du Candidat")]
        public string IdCandidat { get; set; }
        [DisplayName("Id de l'offre d'emploi")]
        public int OffreDEmploiId { get; set; }
        [DisplayName("Votre Pretention Salariale")]
        [Required(ErrorMessage = "Veuillez renseigner votre Pretention Salariale ")]
        public decimal PretentionSalariale { get; set; }
        [DisplayName("Date de Disponibilite")]
        [Required(ErrorMessage = "Veuillez renseigner la date de disponibilite")]
        [DataType(DataType.Date)]
        public DateTime DateDisponibilite { get; set; }
    }
}
