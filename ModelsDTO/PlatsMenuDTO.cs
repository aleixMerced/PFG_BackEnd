    namespace PFG_BackEnd.ModelsDTO;

    public class PlatsMenuDTO
    {
        public string NomPlat { get; set; }
        public int IdPlat { get; set; }
        public string Categoria { get; set; }
        public DateOnly DiaMenu { get; set; } 
        
        public int? IdMenu { get; set; }   
    }