using System.ComponentModel.DataAnnotations.Schema;

namespace PFG_BackEnd.Models;
[Table( "COMANDA_PRODUCTE" )]
public class Comanda_Producte
{
    [Column ("ID_COMANDA")]
    public int IdComanda    { get; set; }
    public Comanda Comanda  { get; set; }

    [Column ("ID_PRODUCTE")]
    public int IdProducte   { get; set; }
    public Producte Producte{ get; set; }

    [Column ("QUANTITAT")]
    public int Quantitat    { get; set; }
    
    [Column ("PREU_MOMENT")]
    public decimal PreuMoment{ get; set; }
}