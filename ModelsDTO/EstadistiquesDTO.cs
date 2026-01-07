namespace PFG_BackEnd.ModelsDTO;

public class EstadistiquesDTO
{
    public decimal Total { get; set; }
    public int ProductesTotals { get; set; }
    public int MenusFets { get; set; }
    public int? ProducteMesVenutId { get; set; }
    public string? NomMesVenut { get; set; } 
    public int? UnitatsMesVenut { get; set; }
}