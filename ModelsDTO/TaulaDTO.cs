namespace PFG_BackEnd.ModelsDTO;

public class TaulaDTO
{
    public int IdTaula { get; set; }

    public int Ocupat { get; set; }

    public char Interiorexterior { get; set; }

    public int? TaulaPare { get; set; }

    public string? Imatge { get; set; }
    
    public int? Actiu { get; set; }
    
    public int NumTaula { get; set; }
    
    public int? TeSubTaules { get; set; } 
    
    public string NomMostrat { get; set; }

}