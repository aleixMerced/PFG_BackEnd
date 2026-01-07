using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PFG_BackEnd.Models;

[Table("TIPUS_PRODUCTE")]
public class Tipusproducte
{
    [Key] 
    [Column("ID")]
    public int IdTipus { get; set; }
    
    [Required]
    public string NomTipus { get; set; }
    
    
    public string? FotoTipus { get; set; }
}