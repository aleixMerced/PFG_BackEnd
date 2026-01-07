using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PFG_BackEnd.Models;

public class Taula
{
    [Key]
    [Column("ID")] 
    public int IdTaula { get; set; }

    public int OCUPAT {get; set;}
    
    public char INTERIOREXTERIOR { get; set; } 
    
    [Column("TAULA_PARE")]
    public int? TaulaPare  { get; set; }
    
    public string? IMATGE  { get; set; }
    
    public int? ACTIU { get; set; }
    
    [Column("NUM_TAULA")]
    public int NumTaula  { get; set; }
}