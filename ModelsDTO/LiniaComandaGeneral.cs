namespace PFG_BackEnd.ModelsDTO;

public class LiniaComandaGeneral
{
    public int idProducte { get; set; }
    public string nomProducte { get; set; } = "";
    public int unitats { get; set; }           
    public decimal? preuUnitari { get; set; }      
    public decimal total { get; set; }   
    public bool pagat  { get; set; }
    public decimal preuPagat { get; set; }
    
    public int? stockDisponible { get; set; }
}