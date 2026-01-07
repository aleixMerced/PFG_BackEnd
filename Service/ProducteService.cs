using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PFG_BackEnd.ModelsDTO;  
using PFG_BackEnd.Models;

namespace PFG_BackEnd.Service;

public class ProducteService : ServiceCollection
{
    private readonly AppDbContext AppDbContext;
    
    private readonly IWebHostEnvironment env;


    public ProducteService(AppDbContext context, IWebHostEnvironment _env)
    {
        AppDbContext = context ?? throw new ArgumentNullException(nameof(context));
        env = _env;
    }

    public async Task<List<ProducteDTO>> GetAllAsync()
    {
        return await AppDbContext.Productes
            .Include(p => p.Tipus)
            .Select(p => new ProducteDTO
            {
                IdProducte = p.IdProducte,
                NomProducte = p.NomProducte.Trim(),
                ImatgeProducte = p.ImatgeProducte == null ? null : p.ImatgeProducte.Trim(),
                PreuVenta = p.PreuVenta,
                NomTipus = p.Tipus == null ? null : p.Tipus.NomTipus.Trim(),
                Estoc = p.Estoc,
                MinimEstoc = p.MinimEstoc,
                PreuCompra = p.PreuCompra
            })
            .ToListAsync();
    }

    public async Task<List<ProducteDTO>> GetPlatsAsync()
    {
        return await AppDbContext.Productes
            .Include(p => p.Tipus)
            .Where(p => p.IdTipus == 17)
            .Select(p => new ProducteDTO
            {
                IdProducte = p.IdProducte,
                NomProducte = p.NomProducte.Trim(),
                ImatgeProducte = p.ImatgeProducte == null ? null : p.ImatgeProducte.Trim(),
                PreuVenta = p.PreuVenta,
                NomTipus = p.Tipus == null ? null : p.Tipus.NomTipus.Trim(),
                Estoc = p.Estoc,
                MinimEstoc = p.MinimEstoc,
                PreuCompra = p.PreuCompra
            })
            .ToListAsync();
    }

    public async Task<List<ProducteDTO>> GetProductesByTipus(string tipus)
    {
        return await AppDbContext.Productes
            .Include(p => p.Tipus)
            .Where(p => p.Tipus.NomTipus == tipus)
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
    
    private int getLastID()
    {
        if (!AppDbContext.Productes.Any())
            return 0;

        return AppDbContext.Productes
            .OrderByDescending(p => p.IdProducte)
            .Select(p => p.IdProducte)
            .First();
    }
    
    public async Task<Producte> CrearProducteAsync(ProducteNewDTO dto)
    {
        string? relativePath = null;
        int id = getLastID() +1;
        if (dto.Imatge != null && dto.Imatge.Length > 0)
        {
            var uploadsRoot = Path.Combine(env.WebRootPath, "uploads", "img", "TipusProducte");
            Directory.CreateDirectory(uploadsRoot);

            var extension = Path.GetExtension(dto.Imatge.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsRoot, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Imatge.CopyToAsync(stream);
            }

            relativePath = $"/uploads/img/Productes/{fileName}";
        }

        var producte = new Producte
        {
            IdProducte = id,
            NomProducte = dto.NomProducte.Trim(),
            IdTipus     = dto.IdTipus,
            PreuVenta   = dto.PreuVenta,
            PreuCompra  = dto.PreuCompra,
            Estoc      = dto.Estoc == 0 ? null : dto.Estoc,
            MinimEstoc = dto.MinimEstoc == 0 ? null : dto.MinimEstoc,
            ImatgeProducte = relativePath
        };

        AppDbContext.Productes.Add(producte);
        await AppDbContext.SaveChangesAsync();

        return producte;
    }

    public async Task<ProducteDTO> GetByIDAsync(int id)
    {
        var dto = await AppDbContext.Productes
            .Include(p => p.Tipus)
            .Where(p => p.IdProducte == id)
            .Select(p => new ProducteDTO
            {
                IdProducte      = p.IdProducte,
                NomProducte     = p.NomProducte.Trim(),
                ImatgeProducte  = p.ImatgeProducte.Trim(),
                PreuVenta       = p.PreuVenta,
                NomTipus        = p.Tipus.NomTipus.Trim(),
                Estoc           = p.Estoc,
                MinimEstoc      = p.MinimEstoc,
                PreuCompra      = p.PreuCompra
            })
            .FirstOrDefaultAsync();

        if (dto == null)
            throw new KeyNotFoundException($"No s'ha trobat cap producte amb id {id}.");

        return dto;
    }
    
    public async Task<Producte> ActualitzarProducteAsync(ProducteNewDTO dto)
    {
        if (dto.idProducte is null)
            throw new ArgumentException("Falta l'IdProducte per actualitzar");

        var producte = await AppDbContext.Productes
            .FirstOrDefaultAsync(p => p.IdProducte == dto.idProducte.Value);

        if (producte == null)
            throw new KeyNotFoundException($"No s'ha trobat el producte amb id {dto.idProducte}");

        // Actualitzar camps
        producte.NomProducte = dto.NomProducte.Trim();
        producte.IdTipus     = dto.IdTipus;
        producte.PreuVenta   = dto.PreuVenta;
        producte.PreuCompra  = dto.PreuCompra;
        producte.Estoc       = dto.Estoc;
        producte.MinimEstoc  = dto.MinimEstoc;

        if (dto.Imatge is not null && dto.Imatge.Length > 0)
        {
            if (!string.IsNullOrWhiteSpace(producte.ImatgeProducte))
            {
                var oldPath = Path.Combine(
                    env.WebRootPath,
                    producte.ImatgeProducte.TrimStart('/', '\\'));

                if (File.Exists(oldPath))
                    File.Delete(oldPath);
            }

            var uploadsRoot = Path.Combine(env.WebRootPath, "uploads", "img", "Productes");
            Directory.CreateDirectory(uploadsRoot);

            var extension = Path.GetExtension(dto.Imatge.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsRoot, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Imatge.CopyToAsync(stream);
            }

            var relativePath = $"/uploads/img/Productes/{fileName}";
            producte.ImatgeProducte = relativePath;
        }

        await AppDbContext.SaveChangesAsync();
        return producte;
    }
    
    public async Task<Producte> DeleteProductByIDAsync(int id)
    {
        var producte = await AppDbContext.Productes
            .FirstOrDefaultAsync(p => p.IdProducte == id);

        if (producte == null)
        {
            throw new KeyNotFoundException($"No s'ha trobat el producte amb id {id}");
        }

        // Eliminar la imatge del disc si existeix
        if (!string.IsNullOrWhiteSpace(producte.ImatgeProducte))
        {
            var imgPath = Path.Combine(
                env.WebRootPath,
                producte.ImatgeProducte.TrimStart('/', '\\'));

            if (File.Exists(imgPath))
            {
                File.Delete(imgPath);
            }
        }

        AppDbContext.Productes.Remove(producte);
        await AppDbContext.SaveChangesAsync();

        return producte;
    }

    public async Task<UpdateStockResultatDTO?> UpdateStockAsync(int idProducte, int nouStock)
    {
        var producte = await AppDbContext.Productes.FirstOrDefaultAsync(p => p.IdProducte == idProducte);

        if (producte == null) return null;
        
        var stockFinal = producte.Estoc + nouStock;
        
        if (stockFinal < 0)
        {
            return new UpdateStockResultatDTO
            {
                Suficient = false,
                NewStock = producte.Estoc,
                Warning = false,
                Message = "No hi ha estoc suficient"
            };
        }
        
        producte.Estoc = stockFinal;
        await AppDbContext.SaveChangesAsync();

        return new UpdateStockResultatDTO
        {
            Suficient = true,
            NewStock = stockFinal,
            Warning = stockFinal <= producte.MinimEstoc,
            Message = stockFinal <= producte.MinimEstoc ? $"Estoc per sota minims ({stockFinal})" : null
        };
    }


    


   
}
