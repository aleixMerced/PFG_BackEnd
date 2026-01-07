namespace PFG_BackEnd.ModelsDTO;

public class ComandaPagadaDTO
{
    public int  IdComandaPagada { get; set; }
    public int  IdComanda       { get; set; }

    public decimal QuantitatComanda   { get; set; }
    public DateTime? DataPagament { get; set; }
}