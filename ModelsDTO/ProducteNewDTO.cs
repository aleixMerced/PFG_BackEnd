namespace PFG_BackEnd.ModelsDTO;

public class ProducteNewDTO
{
    public int? idProducte { get; set; }
    public string NomProducte { get; set; }
    public int IdTipus { get; set; }
    public decimal PreuVenta { get; set; }
    public decimal PreuCompra { get; set; }
    public int Estoc { get; set; }
    public int MinimEstoc { get; set; }
    public IFormFile? Imatge { get; set; }
}