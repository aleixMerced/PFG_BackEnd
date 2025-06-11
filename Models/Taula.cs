using System.ComponentModel.DataAnnotations;

namespace PFG_BackEnd.Models;

public class Taula
{
    [Key]
    public int IdTaula { get; set; }

    public int NumeroPersones {get; set;}
    
}