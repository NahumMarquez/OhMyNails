using Microsoft.EntityFrameworkCore;
using OhMyNails.Models;

namespace OhMyNails.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Cita> Citas { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
    }
}
