using Microsoft.EntityFrameworkCore;
using WorksheetGen.Api.Models;

namespace WorksheetGen.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        internal DbSet<Worksheet> Worksheets { get; set; }
    }
}
