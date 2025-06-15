using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<CodigoVerificacion> CodigosVerificacion { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Clave> Claves { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Producto>().ToTable("productos");
            modelBuilder.Entity<Usuario>().ToTable("usuarios");
            modelBuilder.Entity<CodigoVerificacion>().ToTable("codigosverificacion");
            modelBuilder.Entity<Clave>().ToTable("claves");
        }
    }
}
