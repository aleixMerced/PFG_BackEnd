using System.Collections;
using Microsoft.EntityFrameworkCore;
using PFG_BackEnd.Models;
using PFG_BackEnd.ModelsDTO;

namespace PFG_BackEnd.Service;

public class MenuService : ServiceCollection
{
 
    private readonly AppDbContext AppDbContext;

    public MenuService(AppDbContext context)
    {
        AppDbContext = context;
    }
    public async Task<IEnumerable<PlatsMenuDTO>> GetPlatsMenuAsync(int? idMenu, DateOnly dia)
    {

        var query = AppDbContext.MenuPlats
            .Include(mp => mp.Plat)
            .Where(mp => mp.DiaMenu == dia);

        if (idMenu.HasValue && idMenu.Value > 0)
            query = query.Where(mp => mp.IdMenu == idMenu.Value);
        
        var result = await query
            .Select(mp => new PlatsMenuDTO
            {
                IdPlat = mp.IdPlat,
                NomPlat = mp.Plat.NomProducte.Trim(),
                Categoria = mp.CategoriaMenu,
                DiaMenu = mp.DiaMenu
            })
            .ToListAsync();

        return result;
    }

    public async Task<IEnumerable<PlatsMenuDTO>> GetMenuDiaAsync(DateOnly data)
    {
        var query = AppDbContext.MenuPlats
            .Include(mp => mp.Plat)
            .Where(mp =>  mp.DiaMenu == data);

        var result = await query
            .Select(mp => new PlatsMenuDTO
            {
                IdPlat = mp.IdPlat,
                NomPlat = mp.Plat.NomProducte.Trim(),
                Categoria = mp.CategoriaMenu,
                DiaMenu = mp.DiaMenu,
                IdMenu = mp.IdMenu
            })
            .ToListAsync();

        return result;
    }
    
    public async Task SaveMenuDiaAsync(PlatsMenuNewDTO dto)
    {
        var existents = await AppDbContext.MenuPlats
            .Where(x => x.DiaMenu == dto.DiaMenu && x.IdMenu == dto.IdMenu) 
            .ToListAsync();

        if (existents.Count > 0)
        {
            AppDbContext.MenuPlats.RemoveRange(existents);
            await AppDbContext.SaveChangesAsync();
        }
        

        var nous = new List<MenuPlat>();

        foreach (var idPlat in dto.Primers.Distinct())
        {
            nous.Add(new MenuPlat
            {
                DiaMenu = dto.DiaMenu,
                IdPlat = idPlat,
                CategoriaMenu = "P",
                IdMenu =  dto.IdMenu,
            });
        }

        foreach (var idPlat in dto.Segons.Distinct())
        {
            nous.Add(new MenuPlat
            {
                DiaMenu = dto.DiaMenu,
                IdPlat = idPlat,
                CategoriaMenu = "S",
                IdMenu =  dto.IdMenu,
            });
        }

        await AppDbContext.MenuPlats.AddRangeAsync(nous);

        // 3) Guardar
        await AppDbContext.SaveChangesAsync();
    }
    
    public async Task<int> DeleteMenuDiaAsync(DateOnly diaMenu, int idMenu)
    {
        var deleted = await AppDbContext.MenuPlats
            .Where(x => x.DiaMenu == diaMenu && x.IdMenu == idMenu)
            .ExecuteDeleteAsync();

        return deleted; 
    }
}