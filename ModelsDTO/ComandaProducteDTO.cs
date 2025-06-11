namespace PFG_BackEnd.ModelsDTO;

public class ComandaProducteDTO
{
    public int    idProducte  { get; set; }
    public int    quantitat   { get; set; }
    public int idComanda { get; set; }
    public decimal preuMoment  { get; set; }
}