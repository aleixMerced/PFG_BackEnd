namespace PFG_BackEnd.ModelsDTO;

public class ProducteComandaDTO
{
    public int IdProducte { get; set; }
    
    public string NomProducte { get; set; }
    
    public string? ImatgeProducte { get; set; }
    
    public decimal? PreuVenta { get; set; }
    
    public string? NomTipus { get; set; }    
    
    public int? Estoc { get; set; }
    
    public int? MinimEstoc { get; set; }
    
    public decimal? PreuCompra { get; set; }
    
    //PER TREURE PRODUCTES D COMANDA
    public int Quantitat { get; set; }          
    public decimal PreuMoment { get; set; } 
}