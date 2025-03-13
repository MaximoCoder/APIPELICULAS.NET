using ApiPeliculas.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas.Data
{
    // Sin identity : DbContext
    public class ApplicationDbContext : IdentityDbContext<AppUsuario>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) 
        {
        }
        // Para permitir identity
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        // Pasamos todos los modelos
        public DbSet<Categoria> Categorias {  get; set; }
        public DbSet<Pelicula> Pelicula { get; set; }
        // Implementar sin identity:
        public DbSet<Usuario> Usuario { get; set; }

        // Implementar con Identity:
        public DbSet<AppUsuario> AppUsuario { get; set; }
    }
}
