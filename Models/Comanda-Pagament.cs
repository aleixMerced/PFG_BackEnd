using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PFG_BackEnd.Models;

[Table("COMANDA_PAGAMENT", Schema = "dbo")]
public class Comanda_Pagament
{
    [Key]
    [Column("ID_PAGAMENT")]
    public int IdPagament { get; set; }

    [Required]
    [Column("ID_COMANDA")]
    public int IdComanda { get; set; }

    [Required]
    [Column("TIPUS_PAGAMENT")]
    public string TipusPagament { get; set; } 

    [Required]
    [Column("IMPORT", TypeName = "decimal(18,2)")]
    public decimal Import { get; set; }

    [Required]
    [Column("DATA_PAGAMENT", TypeName = "datetime")]
    public DateTime DataPagament { get; set; }

    [Column("OBSERVACIONS")]
    public string? Observacions { get; set; }

    public Comanda Comanda { get; set; } = null!;
}