using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ModernRecrut.MVC.Areas.Identity.Data
{
    public class Utilisateur : IdentityUser
    {
        public string Nom { get; set; }
        public string Prenom { get; set; }      
        public string Type { get; set; }
    }
}
