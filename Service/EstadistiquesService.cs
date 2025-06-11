using Microsoft.EntityFrameworkCore;
using PFG_BackEnd.Models;
using PFG_BackEnd.ModelsDTO;

namespace PFG_BackEnd.Service;

public class EstadistiquesService : ServiceCollection
{
    private readonly AppDbContext AppDbContext;

    public EstadistiquesService(AppDbContext appDbContext)
    {
        AppDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));

    }

    public async Task<EstadistiquesDTO> GetEstadisticaByTipusDia(string tipusCaixa, DateOnly dia)
    {
        var esta = await AppDbContext.Estadistiques
            .Where(e => e.TipusCaixa == tipusCaixa && e.DiaCaixa == dia)
            .Select(e => new EstadistiquesDTO
            {
                IdCaixa          = e.IdCaixa,
                TipusCaixa       = e.TipusCaixa,
                DiaCaixa         = e.DiaCaixa,
                TotalCaixa       = e.TotalCaixa,
                TotalMenus       = e.TotalMenus,
                TotalEntrepans   = e.TotalEntrepans,
                Observacions     = e.Observacions,
                Horari           = e.Horari,
                DiaCaixaTancada  = e.DiaCaixaTancada
            })
            .FirstOrDefaultAsync();

        if (esta is null)
            throw new KeyNotFoundException($"No s'ha trobat estadística per {tipusCaixa} al dia {dia}.");

        return esta;
    }

    public async Task IncrementarPreuAsync(decimal id, decimal augment)
    {
        var esta = await AppDbContext.Estadistiques.FindAsync(id);

        if (esta is null)
        {
            throw new KeyNotFoundException("No existeix");

        }
        esta.TotalCaixa += augment;
        await AppDbContext.SaveChangesAsync();
    }
    
    public async Task<IEnumerable<EstadistiquesDTO>> GetAllTipusDatesAsync()
    {
        // Fem un GroupBy per eliminar duplicats
        return await AppDbContext.Estadistiques
            .GroupBy(c => new { c.TipusCaixa, c.DiaCaixa })
            .Select(g => new EstadistiquesDTO {
                TipusCaixa = g.Key.TipusCaixa,
                DiaCaixa   = g.Key.DiaCaixa
            })
            .ToListAsync();
    }
}