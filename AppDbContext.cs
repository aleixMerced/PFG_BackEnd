using Microsoft.EntityFrameworkCore;
using PFG_BackEnd.Models;
using PFG_BackEnd.Service;

namespace PFG_BackEnd;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Comanda> Comandes { get; set; }
    
    public DbSet<Taula> Taules { get; set; }
    public DbSet<Producte> Productes { get; set; }
    public DbSet<Tipusproducte> TipusProducte { get; set; }
    
    public DbSet<Comanda_Producte> ComandaProducte { get; set; }
    
    public DbSet<Estadistiques> Estadistiques { get; set; }
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Producte>()
            .HasOne(p => p.Tipus)
            .WithMany() 
            .HasForeignKey(p => p.IdTipus);
        
        modelBuilder.Entity<Comanda_Producte>()
            .HasKey(cp => new { cp.IdComanda, cp.IdProducte });
        
        modelBuilder.Entity<Comanda_Producte>()
            .HasOne(cp => cp.Comanda)
            .WithMany(c => c.Productes)
            .HasForeignKey(cp => cp.IdComanda);

        modelBuilder.Entity<Comanda_Producte>()
            .HasOne(cp => cp.Producte)
            .WithMany(p => p.Comandes)
            .HasForeignKey(cp => cp.IdProducte);
    }


}