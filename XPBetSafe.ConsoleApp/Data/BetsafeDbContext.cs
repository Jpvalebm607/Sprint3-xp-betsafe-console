using Microsoft.EntityFrameworkCore;
using XPBetSafe.ConsoleApp.Domain;

namespace XPBetSafe.ConsoleApp.Data;

public class BetsafeDbContext : DbContext
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<RegistroAposta> Registros => Set<RegistroAposta>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlite("Data Source=betsafe.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(e =>
        {
            e.Property(p => p.Nome).IsRequired().HasMaxLength(100);
            e.Property(p => p.Email).IsRequired().HasMaxLength(120);
            e.HasIndex(p => p.Email).IsUnique();
        });

        modelBuilder.Entity<RegistroAposta>(e =>
        {
            e.Property(p => p.Plataforma).HasMaxLength(60);
            e.Property(p => p.Categoria).HasMaxLength(60);
            e.HasOne(r => r.Usuario)
             .WithMany(u => u.Registros)
             .HasForeignKey(r => r.UsuarioId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
