using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ModernRecrut.MVC.Models
{
    public class Postulation
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [DisplayName("Id du Candidat")]
        public string IdCandidat { get; set; }
        [DisplayName("Id de l'offre d'emploi")]
        public int OffreDEmploiId { get; set; }
        [DisplayName("Pretention Salariale")]
        [Required(ErrorMessage = "Veuillez renseigner votre Pretention Salariale ")]
        public decimal PretentionSalariale { get; set; }
        [DisplayName("Date de Disponibilite")]
        [Required(ErrorMessage = "Veuillez renseigner la date de disponibilite")]
        [DataType(DataType.Date)]
        public DateTime DateDisponibilite { get; set; }
    }
}
