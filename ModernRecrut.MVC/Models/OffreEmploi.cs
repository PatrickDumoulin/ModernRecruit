using System.ComponentModel.DataAnnotations;

namespace ModernRecrut.MVC.Models
{
    public class OffreEmploi
    {
        [Key]
        [Display(Name = "Identifiant")]
        public int Id { get; set; }
        [Required(ErrorMessage = "Le champ est obligatoire")]
        public string Nom { get; set; }
        [Required(ErrorMessage = "Le champ est obligatoire")]
        public string Poste { get; set; }
        [Required(ErrorMessage = "Le champ est obligatoire")]
        public string Description { get; set; }
        [Display(Name = "Date d'affichage")]
        [DataType(DataType.Date)]
        public DateTime DateDebut { get; set; }
        [Display(Name = "Date de fin d'affichage")]
        [DataType(DataType.Date)]
        public DateTime? DateFin { get; set; }

    }
}
