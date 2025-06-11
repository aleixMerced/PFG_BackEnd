using Microsoft.EntityFrameworkCore;
using PFG_BackEnd.Models;
using PFG_BackEnd.ModelsDTO;

namespace PFG_BackEnd.Service;


public class ComandaService : ServiceCollection
{
    
    private readonly AppDbContext AppDbContext;

    public ComandaService(AppDbContext context)
    {
        AppDbContext = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<List<ComandaDTO>> GetAllAsync()
    {
        return await AppDbContext.Comandes
            .Select(c => new ComandaDTO
            {
                IdComanda = c.IdComanda,
                NomClient = c.NomClient.Trim(),
                EstatComanda = c.EstatComanda.Trim(),
                TipusPagament = c.TipusPagament.Trim(),
                DataComanda = c.DataComanda,
                DataPagament = c.DataPagament,
                PreuComanda = c.PreuComanda,
                IDTaula = c.IDTaula,
            })
            .ToListAsync();
    }

    public async Task<List<ComandaDTO>> GetComandaByName(string name)
    {
        return await AppDbContext.Comandes
            .Where(c => c.NomClient.Trim() == name.Trim())
            .Select(c => new ComandaDTO
            {
                IdComanda = c.IdComanda,
                NomClient = c.NomClient.Trim(),
                EstatComanda = c.EstatComanda.Trim(),
                TipusPagament = c.TipusPagament.Trim(),
                DataComanda = c.DataComanda,
                DataPagament = c.DataPagament,
                PreuComanda = c.PreuComanda,
                IDTaula = c.IDTaula
            })
            .ToListAsync(); 
    }

    public async Task<ComandaDTO> GetComandaByID(int id)
    {
        return await AppDbContext.Comandes
            .Where(c => c.IdComanda == id)
            .Select(c => new ComandaDTO
            {
                IdComanda = c.IdComanda,
                NomClient = c.NomClient.Trim(),
                EstatComanda = c.EstatComanda.Trim(),
                TipusPagament = c.TipusPagament.Trim(),
                DataComanda = c.DataComanda,
                DataPagament = c.DataPagament,
                PreuComanda = c.PreuComanda,
                IDTaula = c.IDTaula
            })
            .FirstOrDefaultAsync(); 
    }
    
    public async Task<Comanda> CreateComanda(Comanda comanda)
    {
        AppDbContext.Comandes.Add(comanda);
        await AppDbContext.SaveChangesAsync();

        return comanda;
    }
    
    public async Task<Comanda> GetLastID()
    {
        return await AppDbContext.Comandes
            .Where(c => c.IdComanda == AppDbContext.Comandes.Max(x => x.IdComanda))
            .FirstOrDefaultAsync();
    }
    
    
    public async Task<Comanda_Producte> AfegirProducteComanda(int idProducte, int quantitat, int idComanda, decimal preuMoment)
    {
        var existeixComanda = await AppDbContext.Comandes.AnyAsync(c => c.IdComanda == idComanda);
        var existeixProducte = await AppDbContext.Productes.AnyAsync(p => p.IdProducte == idProducte);

        if (!existeixComanda || !existeixProducte)
        {
            throw new InvalidOperationException($"No existeix la comanda o el producte");
        }

        var comandaProducte = await AppDbContext.ComandaProducte
            .FirstOrDefaultAsync(c => c.IdComanda == idComanda && c.IdProducte == idProducte);

        if (comandaProducte != null)
        {
            //Si ja existeix el producte en aquella comanda 
            comandaProducte.PreuMoment = preuMoment;
            comandaProducte.Quantitat = quantitat;
            await AppDbContext.SaveChangesAsync();
            return comandaProducte;
        } 
        
        var comprod = new Comanda_Producte
        {
            IdComanda = idComanda,
            IdProducte = idProducte,
            Quantitat = quantitat,
            PreuMoment = preuMoment
        };
        AppDbContext.ComandaProducte.Add(comprod);
        await AppDbContext.SaveChangesAsync(); 

        return comprod;

    }

    public async Task<List<ProducteDTO>> GetProducteComanda(int idComanda)
    {
        var productes = await AppDbContext.ComandaProducte
            .Where(cp => cp.IdComanda == idComanda)
            .Include(cp => cp.Producte)
            .Select(cp => new ProducteDTO
            {
                IdProducte = cp.Producte.IdProducte,
                NomProducte = cp.Producte.NomProducte.Trim(),
                Estoc = cp.Producte.Estoc,
                ImatgeProducte = cp.Producte.ImatgeProducte.Trim(),
                PreuVenta = cp.Producte.PreuVenta,
                PreuCompra = cp.Producte.PreuCompra,
                MinimEstoc = cp.Producte.MinimEstoc,
                NomTipus = cp.Producte.Tipus.NomTipus.Trim(),
                Quantitat = cp.Quantitat,
                PreuMoment = cp.PreuMoment,
            })
            .ToListAsync();

        return productes;

    }

    public async Task<Comanda> ActualitzarComanda(Comanda novaComanda)
    {
        var antiguaComanda = await AppDbContext.Comandes.FindAsync(novaComanda.IdComanda);

        if (antiguaComanda == null)
        {
            return null;
        }

        antiguaComanda.NomClient = novaComanda.NomClient;
        antiguaComanda.EstatComanda = novaComanda.EstatComanda;
        antiguaComanda.TipusPagament = novaComanda.TipusPagament;
        antiguaComanda.DataComanda = novaComanda.DataComanda;
        antiguaComanda.DataPagament = novaComanda.DataPagament;
        antiguaComanda.PreuComanda = novaComanda.PreuComanda;
        antiguaComanda.IDTaula = novaComanda.IDTaula;
        
        await AppDbContext.SaveChangesAsync();
        return antiguaComanda;
    }
}