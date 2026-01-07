namespace PFG_BackEnd.ModelsDTO;

public class ImprimirTicketDTO
{
    public string NomProducte { get; set; }
    
    public int Quantitat { get; set; }
    
    public double PreuUnitari { get; set; }
    
    public double TotalLinia { get; set; }
}