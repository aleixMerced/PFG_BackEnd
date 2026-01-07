namespace PFG_BackEnd.ModelsDTO;

public class UpdateStockResultatDTO
{
    public bool Suficient { get; set; }
    public int? NewStock { get; set; }
    public bool Warning { get; set; }
    public string? Message { get; set; }
}