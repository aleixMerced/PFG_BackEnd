namespace PFG_BackEnd.ModelsDTO;

public class EnviarCuinaDTO
{
    public string IdTaula { get; set; }

    public List<PlatsCuinaDTO> PrimersPlats { get; set; } = new();
    public List<PlatsCuinaDTO> SegonsPlats { get; set; } = new();
}