namespace PFG_BackEnd.ModelsDTO;

public class ComandaUpdateDto
{
    public int IdComanda { get; set; }
    public string? NomClient { get; set; }
    public string? EstatComanda { get; set; }
    public DateTime? DataComanda { get; set; }
    public DateTime? DataPagament { get; set; }
    public string? TipusPagament { get; set; }
    public double? PreuComanda { get; set; } // mantinc double perquè la teva entitat és double
    public int? IDTaula { get; set; }
    public List<ComandaLiniaUpdateDTO>? Productes { get; set; }
}