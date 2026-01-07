using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PFG_BackEnd.Models;

[Table("CAIXA_DIARIA")]
public class CaixaDiaria
{
    [Key]
    [Column("ID")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public decimal Id { get; set; }

    [Required]
    [Column("DataCaixa", TypeName = "datetime")]
    public DateTime DataCaixa { get; set; }

    [Column("MatiTargeta", TypeName = "decimal(18,2)")]
    public decimal? MatiTargeta { get; set; }

    [Column("MatiEfectiu", TypeName = "decimal(18,2)")]
    public decimal? MatiEfectiu { get; set; }

    [Column("MatiTotal", TypeName = "decimal(18,2)")]
    public decimal? MatiTotal { get; set; }

    [Column("TardaTargeta", TypeName = "decimal(18,2)")]
    public decimal? TardaTargeta { get; set; }

    [Column("TardaEfectiu", TypeName = "decimal(18,2)")]
    public decimal? TardaEfectiu { get; set; }

    [Column("TardaTotal", TypeName = "decimal(18,2)")]
    public decimal? TardaTotal { get; set; }

    [Column("TotalDia", TypeName = "decimal(18,2)")]
    public decimal? TotalDia { get; set; }

    [Column("Observacions")]
    [MaxLength(500)] // posa la mida que tingui la columna
    public string? Observacions { get; set; }
}
