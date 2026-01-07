using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PFG_BackEnd.Models;

[Table("PRODUCTE")]
public class Producte
{
    
    [Key]
    [Column("ID")] 
    public int IdProducte { get; set; }
    
    [Required]
    public string NomProducte { get; set; }
    
    [Column("ID_TIPUS")]
    public int? IdTipus { get; set; }
    
    [ForeignKey("IdTipus")]
    public Tipusproducte? Tipus { get; set; }
    
    public string? ImatgeProducte { get; set; }
    
    public decimal? PreuVenta { get; set; }
    
    public int? Estoc { get; set; }
    
    public int? MinimEstoc { get; set; }
    
    public decimal? PreuCompra { get; set; }
    
    public ICollection<Comanda_Linia> Comandes { get; set; } = new List<Comanda_Linia>();

    
    //public List<string> Alergies { get; set; } // mirar com ferho 
    
    
    
    
}