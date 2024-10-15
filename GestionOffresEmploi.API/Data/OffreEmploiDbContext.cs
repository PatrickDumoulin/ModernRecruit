using ModernRecrut.Emplois.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ModernRecrut.Emplois.API.Data
{
    public class OffreEmploiDbContext : DbContext
    {
        public OffreEmploiDbContext(DbContextOptions<OffreEmploiDbContext> options) : base(options)
        {
    
        }

        public DbSet<OffreEmploi> OffreEmploi { get; set; }
    }

    

}
