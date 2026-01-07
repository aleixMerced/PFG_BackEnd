namespace PFG_BackEnd.ModelsDTO;

public class ProducteDTO
{
    public int IdProducte { get; set; }
    public string NomProducte { get; set; }
    public string? ImatgeProducte { get; set; }
    public decimal? PreuVenta { get; set; }
    public string? NomTipus { get; set; }
    public int? Estoc { get; set; }
    public int? MinimEstoc { get; set; }
    public decimal? PreuCompra { get; set; }
}
