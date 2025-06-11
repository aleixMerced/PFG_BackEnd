using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PFG_BackEnd.Models;

[Table("ESTADISTIQUES")]
public class Estadistiques
{
    [Key]
    [Column("ID")]  
    public decimal IdCaixa { get; set; }
    
    [Required]
    public string TipusCaixa { get; set; }
    
    [Required]
    public DateOnly DiaCaixa { get; set; }
    
    [Required]
    public decimal TotalCaixa { get; set; }
    
    public decimal? TotalMenus { get; set; }
    
    public decimal? TotalEntrepans { get; set; }
    
    public string? Observacions { get; set; }
    
    public string? Horari { get; set; }
    
    public DateOnly? DiaCaixaTancada { get; set; }
    
}