using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Project_AG.Models;
using System.Text;

namespace Project_AG.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<TipoGasto> TiposGasto { get; set; }
        public DbSet<FondoMonetario> FondosMonetarios { get; set; }
        public DbSet<Deposito> Depositos { get; set; }
        public DbSet<RegistroGasto> RegistrosGasto { get; set; }
        public DbSet<DetalleGasto> DetallesGasto { get; set; }

        public DbSet<PresupuestoTipoGasto> PresupuestosTipoGasto { get; set; } = default!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Deposito>()
                .Property(d => d.Monto)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<DetalleGasto>()
                .Property(d => d.Monto)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PresupuestoTipoGasto>()
                .Property(p => p.MontoPresupuestado)
                .HasColumnType("decimal(18,2)");


            string hashedPassword = HashPassword("admin", "admin");

            modelBuilder.Entity<Usuario>().HasData(new Usuario
            {
                Id = 1,
                NombreUsuario = "admin",
                Contrasena = hashedPassword
            });
        }

        private static string HashPassword(string password, string salt)
        {
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);

            byte[] hashed = KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);

            return Convert.ToBase64String(hashed);
        }
    }
}
