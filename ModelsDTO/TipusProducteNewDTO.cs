namespace PFG_BackEnd.ModelsDTO;

public class TipusProducteNewDTO
{
    public int? IdTipus { get; set; }
    public string NomTipus { get; set; }
    public IFormFile? Imatge { get; set; }
}