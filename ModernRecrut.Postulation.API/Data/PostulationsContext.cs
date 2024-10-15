using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ModernRecrut.Postulation.API.Models;

namespace ModernRecrut.Postulation.API.Data
{
    public class PostulationsContext : DbContext
    {
        public PostulationsContext (DbContextOptions<PostulationsContext> options)
            : base(options)
        {
        }

        public DbSet<PostulationDetail> PostulationDetail { get; set; } = default!;
        public DbSet<NoteDetail> NoteDetail { get; set; } = default!;
    }
}
