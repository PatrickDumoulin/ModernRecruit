using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModernRecrut.MVC.Models
{
    public class DocumentModel
    {
        [Key]
        [Display(Name = "Identifiant")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Le type de document est obligatoire")]
        public string Type { get; set; }
        public string Chemin { get; set; }

       
    }
}
