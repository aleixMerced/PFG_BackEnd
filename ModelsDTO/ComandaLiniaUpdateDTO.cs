namespace PFG_BackEnd.ModelsDTO;

public class ComandaLiniaUpdateDTO
{
    public int IdProducte { get; set; }
    public string NomProducte { get; set; }
    public int Unitats { get; set; }
    public decimal PreuUnitari { get; set; }
    public decimal Total { get; set; }
    public bool Pagat { get; set; }
    public decimal PreuPagat { get; set; }
}