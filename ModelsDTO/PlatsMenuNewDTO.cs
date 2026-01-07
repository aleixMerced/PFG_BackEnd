namespace PFG_BackEnd.ModelsDTO;

public class PlatsMenuNewDTO
{
    public DateOnly DiaMenu { get; set; }
    public List<int> Primers { get; set; } 
    public List<int> Segons { get; set; }
    
    public int IdMenu { get; set; }
}