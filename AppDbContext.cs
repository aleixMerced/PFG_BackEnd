using Microsoft.EntityFrameworkCore;
using PFG_BackEnd.Models;
using PFG_BackEnd.Service;

namespace PFG_BackEnd;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Comanda> Comandes { get; set; }
    public DbSet<Taula> Taula { get; set; }
    public DbSet<Producte> Productes { get; set; }
    public DbSet<Tipusproducte> TipusProducte { get; set; }

    public DbSet<CaixaDiaria> CaixesDiaria { get; set; }
    public DbSet<Comanda_Linia> ComandaLinia { get; set; }
    public DbSet<Comanda_Linia_Pagada> ComandaLiniaPagada { get; set; }
    
    public DbSet<Comanda_Pagament> ComandaPagaments { get; set; } = null!;

    
    public DbSet<Estadistiques> Estadistiques { get; set; }
    
    public DbSet<MenuPlat> MenuPlats { get; set; }

    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Producte>()
            .HasOne(p => p.Tipus)
            .WithMany() 
            .HasForeignKey(p => p.IdTipus);
        
        modelBuilder.Entity<Comanda_Linia_Pagada>(entity =>
        {
            entity.HasKey(e => new { e.IdComanda, e.IdProducte });

            entity.HasOne(e => e.Comanda)
                .WithMany()
                .HasForeignKey(e => e.IdComanda);

            entity.HasOne(e => e.Producte)
                .WithMany()
                .HasForeignKey(e => e.IdProducte);
        });
        
        modelBuilder.Entity<Comanda_Linia>(entity =>
        {
            entity.HasKey(e => new { e.IdComanda, e.IdProducte });

            entity.HasOne(e => e.Comanda)
                .WithMany(c => c.Productes)
                .HasForeignKey(e => e.IdComanda);

            entity.HasOne(e => e.Producte)
                .WithMany(p => p.Comandes)
                .HasForeignKey(e => e.IdProducte);
        });
        
        modelBuilder.Entity<Comanda_Pagament>(entity =>
        {
            entity.HasKey(p => p.IdPagament);

            entity.HasOne(p => p.Comanda)
                .WithMany(c => c.Pagaments)
                .HasForeignKey(p => p.IdComanda);
        });
        
        modelBuilder.Entity<MenuPlat>(entity =>
        {
            entity.ToTable("MENU_PLAT");

            entity.HasKey(mp => new { mp.IdMenu, mp.IdPlat, mp.DiaMenu, mp.CategoriaMenu });

            entity.Property(mp => mp.CategoriaMenu)
                .HasMaxLength(1)
                .IsRequired();

            entity.Property(mp => mp.DiaMenu)
                .HasConversion(
                    d => d.ToDateTime(TimeOnly.MinValue),   
                    d => DateOnly.FromDateTime(d)          
                )
                .HasColumnType("date");   
            
            entity.HasOne(mp => mp.Menu)
                .WithMany()
                .HasForeignKey(mp => mp.IdMenu)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(mp => mp.Plat)
                .WithMany()
                .HasForeignKey(mp => mp.IdPlat)
                .OnDelete(DeleteBehavior.Restrict);
        });


            
    }


}