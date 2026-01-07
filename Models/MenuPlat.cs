namespace PFG_BackEnd.Models;

public class MenuPlat
{
    public int IdMenu { get; set; }       
    public int IdPlat { get; set; }       
    public string CategoriaMenu { get; set; }
    public DateOnly DiaMenu { get; set; }

    // Navigations
    public Producte Menu { get; set; }
    public Producte Plat { get; set; }
}