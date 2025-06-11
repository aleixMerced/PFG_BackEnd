using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PFG_BackEnd.ModelsDTO;  
using PFG_BackEnd.Models;

namespace PFG_BackEnd.Service;

public class ProducteService : ServiceCollection
{
    private readonly AppDbContext AppDbContext;

    public ProducteService(AppDbContext context)
    {
        AppDbContext = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<List<ProducteDTO>> GetAllAsync()
    {
        return await AppDbContext.Productes
            .Include(p => p.Tipus)
            .Select(p => new ProducteDTO
            {
                IdProducte = p.IdProducte,
                NomProducte = p.NomProducte.Trim(),
                ImatgeProducte = p.ImatgeProducte.Trim(),
                PreuVenta = p.PreuVenta,
                NomTipus = p.Tipus.NomTipus.Trim(),
                Estoc = p.Estoc,
                MinimEstoc = p.MinimEstoc,
                PreuCompra = p.PreuCompra
            })
            .ToListAsync();
    }
}
