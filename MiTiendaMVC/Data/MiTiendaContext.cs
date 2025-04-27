// Data/MiTiendaContext.cs
using Microsoft.EntityFrameworkCore;
using MiTiendaMVC.Models;

namespace MiTiendaMVC.Data;

public class MiTiendaContext : DbContext
{
    public MiTiendaContext(DbContextOptions<MiTiendaContext> options)
        : base(options) { }

    public DbSet<Cliente> Clientes { get; set; } = null!;
    public DbSet<Producto> Productos { get; set; } = null!;
    public DbSet<Canasta> Canastas { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>()
            .HasOne(c => c.Canasta)
            .WithOne(ca => ca.Cliente)
            .HasForeignKey<Canasta>(ca => ca.ClienteId);

        base.OnModelCreating(modelBuilder);
    }
}
