using Microsoft.EntityFrameworkCore;
using PFG_BackEnd.Models;
using PFG_BackEnd.ModelsDTO;


namespace PFG_BackEnd.Service;

public class ComandaPagadaService : ServiceCollection
{
    
    private readonly AppDbContext AppDbContext;

    public ComandaPagadaService(AppDbContext context)
    {
        AppDbContext = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public async Task<List<ProducteComandaDTO>> GetProducteComanda(int idComanda)
    {
        var productes = await AppDbContext.ComandaLiniaPagada        
            .Where(clp => clp.IdComanda == idComanda)
            .Include(clp => clp.Producte)                      
            .ThenInclude(p => p.Tipus)
            .Select(clp => new ProducteComandaDTO()
            {
                IdProducte    = clp.Producte.IdProducte,
                NomProducte   = clp.Producte.NomProducte.Trim(),
                Estoc         = clp.Producte.Estoc,
                ImatgeProducte= clp.Producte.ImatgeProducte.Trim(),
                PreuVenta     = clp.Producte.PreuVenta,
                PreuCompra    = clp.Producte.PreuCompra,
                MinimEstoc    = clp.Producte.MinimEstoc,
                NomTipus      = clp.Producte.Tipus.NomTipus.Trim(),
                Quantitat     = clp.Quantitat,
                PreuMoment    = clp.PreuMoment
            })
            .ToListAsync();                                 

        return productes;
    }
    
    public async Task<List<ImprimirTicketDTO>> GetLiniesTicketAsync(int idComandaPagada)
    {
        var linies = await AppDbContext.ComandaLiniaPagada
            .Include(cp => cp.Producte)
            .Where(cp => cp.IdComanda == idComandaPagada) 
            .Select(cp => new ImprimirTicketDTO()
            {
                NomProducte = cp.Producte.NomProducte.Trim(),
                Quantitat   = cp.Quantitat,
                PreuUnitari  = (double)cp.Producte.PreuVenta,
                TotalLinia  = (double)cp.PreuMoment
            })
            .ToListAsync();

        return linies;
    }
    
}