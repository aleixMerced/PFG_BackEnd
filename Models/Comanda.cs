using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PFG_BackEnd.Models;

[Table("COMANDA")]
public class Comanda
{
    [Key]
    [Column("ID")] 
    public int IdComanda { get; set; }
    
    public string NomClient {get; set;}
    
    public string? EstatComanda {get; set;}

    public string? TipusPagament {get; set;}
    
    public DateTime DataComanda {get; set;} 
    
    public DateTime? DataPagament {get; set;}
    
    public double PreuComanda {get; set;}
    
    [Column("IDTaula")]
    public int IDTaula { get; set; }
    
    public char?  Pagat { get; set; }

    
    public ICollection<Comanda_Linia> Productes { get; set; } = new List<Comanda_Linia>();
    
    public ICollection<Comanda_Pagament> Pagaments { get; set; } = new List<Comanda_Pagament>();


    
}