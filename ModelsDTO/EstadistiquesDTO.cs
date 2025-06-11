namespace PFG_BackEnd.ModelsDTO;

public class EstadistiquesDTO
{

    public decimal IdCaixa { get; set; }
    
    public string TipusCaixa { get; set; }
    
    public DateOnly DiaCaixa { get; set; }
    
    public decimal TotalCaixa { get; set; }
    
    public decimal? TotalMenus { get; set; }
    
    public decimal? TotalEntrepans { get; set; }
    
    public string? Observacions { get; set; }
    
    public string? Horari { get; set; }
    
    public DateOnly? DiaCaixaTancada { get; set; }
}