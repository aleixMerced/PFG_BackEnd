using System.ComponentModel.DataAnnotations.Schema;


namespace PFG_BackEnd.Models;
[Table("COMANDA_LINIA_PAGADA")]
public class Comanda_Linia_Pagada
{

    [Column("ID_COMANDA")]
    public int IdComanda { get; set; }
    
    public Comanda Comanda { get; set; }

    [Column("ID_PRODUCTE")]
    public int IdProducte { get; set; }
    
    public Producte Producte { get; set; }
    
    [Column("PREU_MOMENT")]    
    public decimal PreuMoment { get; set; }

    [Column("QUANTITAT")]
    public int Quantitat { get; set; }


    
}