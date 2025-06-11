namespace PFG_BackEnd.Models;

public class Comanda_Menu
{
    public int IdComanda { get; set; }
    public Comanda Comanda {get; set;}
    
    public int IdMenu { get; set; }
    public Menu Menu {get; set;}
}