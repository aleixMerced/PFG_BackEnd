using PFG_BackEnd.Models;

namespace PFG_BackEnd.ModelsDTO;

public class ComandaDTO
{
    public int IdComanda { get; set; }
    
    public string NomClient { get; set; }
    
    public string EstatComanda { get; set; }
    
    public string TipusPagament { get; set; }
    
    public DateTime DataComanda { get; set; }
    
    public DateTime? DataPagament { get; set; }
    
    public double PreuComanda { get; set; }
    
    public int IDTaula { get; set; }
    

}