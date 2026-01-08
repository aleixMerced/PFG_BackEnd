using Microsoft.EntityFrameworkCore;
using PFG_BackEnd.Models;
using PFG_BackEnd.ModelsDTO;

namespace PFG_BackEnd.Service;

public class TaulaService : ServiceCollection
{
    private readonly AppDbContext AppDbContext;

    public TaulaService(AppDbContext context)
    {
        AppDbContext = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<List<Taula>> GetAllInteriorAsync()
    {
        return await AppDbContext.Taula
            .Where(t => t.IdTaula < 99 && t.INTERIOREXTERIOR == 'I' && (
                t.ACTIU == 1
                || (t.TaulaPare == null && AppDbContext.Taula.Any(f => f.TaulaPare == t.IdTaula && f.ACTIU == 1))
            ))
            .OrderBy(t => t.NumTaula)
            .ToListAsync();
    }
    
    public async Task<List<Taula>> GetAllExteriorAsync()
    {
        return await AppDbContext.Taula
            .Where(t => t.IdTaula < 99 && t.INTERIOREXTERIOR == 'E' && (
                t.ACTIU == 1
                || (t.TaulaPare == null && AppDbContext.Taula.Any(f => f.TaulaPare == t.IdTaula && f.ACTIU == 1))
            ))
            .OrderBy(t => t.NumTaula)
            .ToListAsync();
    }

    public async Task<List<Taula>> GetAllAsync()
    {
        return await AppDbContext.Taula
            .Where(t => t.TaulaPare == null)
            .ToListAsync();
    }
    
public async Task<List<TaulaDTO>> GetTaulesActivesAsync()
{
    // Carreguem totes les taules actives (pares i subtaules)
    var taulesActives = await AppDbContext.Taula
        .Where(t => t.ACTIU == 1 && t.IdTaula < 99)
        .OrderBy(t => t.NumTaula)
        .ThenBy(t => t.IdTaula)
        .ToListAsync();

    var resultat = new List<TaulaDTO>();

    // Agrupem subtaules per TaulaPare
    var subTaulesPerPare = taulesActives
        .Where(t => t.TaulaPare != null)
        .GroupBy(t => t.TaulaPare!.Value)
        .ToDictionary(
            g => g.Key,
            g => g.OrderBy(st => st.IdTaula).ToList()
        );

    // Taules pare actives (sense pare) -> NomMostrat = NumTaula
    var taulesPareActives = taulesActives
        .Where(t => t.TaulaPare == null)
        .OrderBy(t => t.NumTaula)
        .ToList();

    foreach (var t in taulesPareActives)
    {
        resultat.Add(new TaulaDTO
        {
            IdTaula          = t.IdTaula,
            Ocupat          = t.OCUPAT,
            Interiorexterior = t.INTERIOREXTERIOR,
            TaulaPare        = t.TaulaPare,
            Imatge           = t.IMATGE,
            Actiu            = t.ACTIU,
            NumTaula         = t.NumTaula,
            TeSubTaules      = subTaulesPerPare.ContainsKey(t.IdTaula)
                                ? subTaulesPerPare[t.IdTaula].Count
                                : 0,
            NomMostrat       = t.NumTaula.ToString()   // "2"
        });
    }

    // Subtaules actives -> NomMostrat = NumTaula + ".index" (2.1, 2.2, ...)
    foreach (var kv in subTaulesPerPare)
    {
        var subTaules = kv.Value; // ja estan ordenades per IdTaula

        for (int i = 0; i < subTaules.Count; i++)
        {
            var st = subTaules[i];

            resultat.Add(new TaulaDTO
            {
                IdTaula          = st.IdTaula,
                Ocupat          = st.OCUPAT,
                Interiorexterior = st.INTERIOREXTERIOR,
                TaulaPare        = st.TaulaPare,
                Imatge           = st.IMATGE,
                Actiu            = st.ACTIU,
                NumTaula         = st.NumTaula,
                TeSubTaules      = 0,
                NomMostrat       = $"{st.NumTaula}.{i + 1}"  // "2.1", "2.2"
            });
        }
    }

    return resultat
        .OrderBy(t => t.NumTaula)
        .ThenBy(t => t.NomMostrat)
        .ToList();
}


    public async Task<List<TaulaDTO>> GetTaulesPareAsync()
    {
        return await AppDbContext.Taula
            .Where(t =>
                t.TaulaPare == null &&       
                t.IdTaula < 99 &&        
                t.OCUPAT == 0 &&                
                !AppDbContext.Taula.Any(st =>  
                    st.TaulaPare == t.IdTaula &&
                    st.ACTIU == 1 &&
                    st.OCUPAT != 0)
            )
            .OrderBy(t => t.NumTaula)
            .Select(t => new TaulaDTO
            { 
                IdTaula          = t.IdTaula,
                NumTaula         = t.NumTaula,
                Ocupat          = t.OCUPAT,
                Interiorexterior = t.INTERIOREXTERIOR,
                TaulaPare        = t.TaulaPare,
                Imatge           = t.IMATGE,
                Actiu            = t.ACTIU,

                TeSubTaules = AppDbContext.Taula
                    .Any(st => st.TaulaPare == t.IdTaula && st.OCUPAT != 1) ? 1 : 0
                
            })
            .ToListAsync();
    }

    public async Task<bool> CanviarEstatAsync(int idTaula)
    {
        var taula = await AppDbContext.Taula.FindAsync(idTaula);
        if (taula == null) return false;

        if (taula.OCUPAT == 1)
        {
            taula.IMATGE = taula.TaulaPare != null
                ? "uploads/img/taula2.png"
                : "uploads/img/taula4.png";

            taula.OCUPAT = 0;
        }
        else
        {
            // passa a ocupada
            taula.IMATGE = taula.TaulaPare != null
                ? "uploads/img/taulaVermell2.png"
                : "uploads/img/taulaVermell4.png";

            taula.OCUPAT = 1;
        }

        await AppDbContext.SaveChangesAsync();
        return true;
    }


    public async Task<TaulaDTO?> GetTaulaByIDAsync(int idtaula)
    {
        var taula = await AppDbContext.Taula
            .FirstOrDefaultAsync(t => t.IdTaula == idtaula);

        if (taula == null)
            return null;

        var teSubTaules = await AppDbContext.Taula
            .AnyAsync(st => st.TaulaPare == taula.IdTaula);

        string nomMostrat;

        if (taula.TaulaPare == null)
        {
            nomMostrat = taula.NumTaula.ToString();
        }
        else
        {
            var germanes = await AppDbContext.Taula
                .Where(st => st.TaulaPare == taula.TaulaPare)
                .OrderBy(st => st.IdTaula)
                .ToListAsync();

            var index = germanes.FindIndex(st => st.IdTaula == taula.IdTaula);
            var posicio = index >= 0 ? index + 1 : 1;

            nomMostrat = $"{taula.NumTaula}.{posicio}";
        }

        return new TaulaDTO
        {
            IdTaula          = taula.IdTaula,
            Ocupat          = taula.OCUPAT,
            Interiorexterior = taula.INTERIOREXTERIOR,
            TaulaPare        = taula.TaulaPare,
            Imatge           = taula.IMATGE,
            Actiu            = taula.ACTIU,
            NumTaula         = taula.NumTaula,
            TeSubTaules      = teSubTaules ? 1 : 0,
            NomMostrat       = nomMostrat
        };
    }


    public async Task<int> GetCountTaulaAsync(int idtaula)
    {
        var comandes = await AppDbContext.Comandes.Where(c => c.IDTaula == idtaula && c.EstatComanda.Trim().ToUpper() == "PENDENT").ToListAsync();
        
        var count = comandes.Count;

        if (count == 0)
        {
            return 0;
        }
        if (count > 1)
        {
            return -1;
        }
        var comanda = comandes.First();
        return comanda.IdComanda; 
    }
    
    public async Task<List<Taula>> GetSubTaulesAsync(int idTaulaPare)
    {
        
        await using var tx = await AppDbContext.Database.BeginTransactionAsync();

        var taulaPare = await AppDbContext.Taula
            .FirstOrDefaultAsync(t => t.IdTaula == idTaulaPare);

        if (taulaPare == null)
            return new List<Taula>();
        
        var subTaules = await AppDbContext.Taula
            .Where(t => t.TaulaPare == idTaulaPare)
            .OrderBy(t => t.IdTaula)
            .ToListAsync();
        if (subTaules.Count == 0)
            return subTaules;
        
        taulaPare.ACTIU = 0;

        foreach (var st in subTaules)
            st.ACTIU = 1;
        
        await AppDbContext.SaveChangesAsync();
        await tx.CommitAsync();
        return subTaules;
        
    }

    public async Task<bool> JuntarTaulesAsync(int idTaulaPare)
    {
        await using var tx = await AppDbContext.Database.BeginTransactionAsync();

        var taulaPare = await AppDbContext.Taula
            .FirstOrDefaultAsync(t => t.IdTaula == idTaulaPare);

        if (taulaPare == null)
            return false;

        var subTaules = await AppDbContext.Taula
            .Where(t => t.TaulaPare == idTaulaPare)
            .ToListAsync();

        if (subTaules.Count == 0)
            return false;

        taulaPare.ACTIU = 1;
        foreach (var st in subTaules)
            st.ACTIU = 0;

        await AppDbContext.SaveChangesAsync();
        await tx.CommitAsync();

        return true;
    }
    private int getLastID()
    {
        var lastId = AppDbContext.Taula
            .Where(t => t.IdTaula < 99)
            .OrderByDescending(t => t.IdTaula)
            .Select(t => t.IdTaula)
            .FirstOrDefault();

        return lastId;
    }

    public async Task<Taula> CrearTaulaAsync(TaulaNewDTO dto)
    {
        if (dto.NumTaula <= 0 || dto.NumTaula > 99)
            throw new ArgumentException("El número de taula ha d'estar entre 1 i 99.");

        var interExt = dto.Ubicacio?.ToUpperInvariant() == "INTERIOR" ? 'I' : 'E';

        await FerEspaiPerNumTaulaAsync(dto.NumTaula, interExt);

        int id = getLastID() + 1;

        var taula = new Taula()
        {
            NumTaula = dto.NumTaula,
            IdTaula = id,
            OCUPAT = 0,
            INTERIOREXTERIOR = interExt,
            TaulaPare = null,
            IMATGE = "/uploads/img/taula4.png",
            ACTIU = 1,
        };

        AppDbContext.Taula.Add(taula);

        if (dto.TeSubTaules)
        {
            for (int i = 0; i < 2; i++)
            {
                id++;
                AfegirSubTaula(taula, id);
            }
        }

        await AppDbContext.SaveChangesAsync();
        return taula;
    }

    
    public async Task<TaulaDTO> GetByIDAsync(int id)
    {
        var dto = await AppDbContext.Taula
            .Where(t => t.IdTaula == id)
            .Select(t => new TaulaDTO()
            {
                IdTaula      = t.IdTaula,
                Actiu = t.ACTIU,
                Imatge = t.IMATGE,
                Interiorexterior = t.INTERIOREXTERIOR,
                Ocupat = t.OCUPAT,
                TaulaPare = t.TaulaPare,
            })
            .FirstOrDefaultAsync();

        if (dto == null)
            throw new KeyNotFoundException($"No s'ha trobat cap taula amb id {id}.");

        return dto;
    }

    public async Task<Taula> ActualitzarTaulaAsync(TaulaNewDTO dto)
{
    if (dto.idTaula is null)
        throw new ArgumentException("Falta l'idTaula per actualitzar");

    var taula = await AppDbContext.Taula
        .FirstOrDefaultAsync(t => t.IdTaula == dto.idTaula.Value);

    if (taula == null)
        throw new KeyNotFoundException($"No s'ha trobat la taula amb id {dto.idTaula.Value}");

    var novaInterExt = dto.Ubicacio?.ToUpperInvariant() == "INTERIOR" ? 'I' : 'E';
    var antigaInterExt = taula.INTERIOREXTERIOR;

    // Si és exterior, NO pot tenir subtaules
    if (novaInterExt == 'E')
        dto.TeSubTaules = false;

    var numAntic = taula.NumTaula;
    var numNou = dto.NumTaula;

    if (numNou <= 0 || numNou > 99)
        throw new ArgumentException("El número de taula ha d'estar entre 1 i 99.");

    //  REORDENACIÓ NUMERACIÓ (per ubicació) -
    if (novaInterExt != antigaInterExt)
    {
        // Tanquem el forat a la ubicació antiga (només taules pare)
        await ReordenarDespresEliminarAsync(numAntic, antigaInterExt);

        //  Fem espai a la ubicació nova (només taules pare)
        await FerEspaiPerNumTaulaAsync(numNou, novaInterExt);

        //  Apliquem ubicació i número nou
        taula.INTERIOREXTERIOR = novaInterExt;
        taula.NumTaula = numNou;
    }
    else
    {
        // mateixa ubicació -> reordenació normal
        if (numNou != numAntic)
        {
            await ReordenarNumTaulaEnModificarAsync(taula, numNou); // ja fa per ubicació
            taula.NumTaula = numNou;
        }
    }

    taula.ACTIU = dto.Actiu;

    var subTaules = await AppDbContext.Taula
        .Where(t => t.TaulaPare == taula.IdTaula)
        .ToListAsync();

    // Si hem canviat número o ubicació, sincronitzem subtaules
    foreach (var st in subTaules)
    {
        st.NumTaula = taula.NumTaula;
        st.INTERIOREXTERIOR = taula.INTERIOREXTERIOR;
    }

    if (taula.INTERIOREXTERIOR == 'E')
    {
        if (subTaules.Count > 0)
            AppDbContext.Taula.RemoveRange(subTaules);
    }
    else
    {
        if (dto.TeSubTaules)
        {
            if (subTaules.Count < 2)
            {
                int id = getLastID() + 1;
                int aCrear = 2 - subTaules.Count;

                for (int i = 0; i < aCrear; i++)
                {
                    AfegirSubTaula(taula, id);
                    id++;
                }
            }
        }
        else
        {
            if (subTaules.Count > 0)
                AppDbContext.Taula.RemoveRange(subTaules);
        }
    }

    await AppDbContext.SaveChangesAsync();
    return taula;
}


    
    public async Task<Taula> DeleteTaulaByIDAsync(int idTaula)
    {
        var taula = await AppDbContext.Taula
            .FirstOrDefaultAsync(t => t.IdTaula == idTaula);

        if (taula == null)
            throw new KeyNotFoundException($"No s'ha trobat la taula amb id {idTaula}");
        
        var interExt = taula.INTERIOREXTERIOR;
        var numTaulaEliminada = taula.NumTaula;
        
        var subTaules = await AppDbContext.Taula
            .Where(t => t.TaulaPare == idTaula)
            .ToListAsync();

        if (subTaules.Any())
            AppDbContext.Taula.RemoveRange(subTaules);

        AppDbContext.Taula.Remove(taula);

        await AppDbContext.SaveChangesAsync();
        
        await ReordenarDespresEliminarAsync(numTaulaEliminada, interExt);


        return taula;
    }


    private void AfegirSubTaula(Taula taulaPare, int id)
    {
        
        var subtaula = new Taula()
        {
            IdTaula = id,
            NumTaula = taulaPare.NumTaula,
            OCUPAT = 0,
            INTERIOREXTERIOR = taulaPare.INTERIOREXTERIOR,
            TaulaPare = taulaPare.IdTaula,
            IMATGE = "/uploads/img/taula2.png",
            ACTIU = 0,
        };

        AppDbContext.Taula.Add(subtaula);
    }
    
    private void eliminarSubTaules(Taula taulaPare)
    {
        var subTaules = AppDbContext.Taula
            .Where(t => t.TaulaPare == taulaPare.IdTaula)
            .ToList();

        if (!subTaules.Any())
            return;

        AppDbContext.Taula.RemoveRange(subTaules);
    }
    
    private async Task FerEspaiPerNumTaulaAsync(int numTaulaNou, char interExt)
    {
        var taulesAReordenar = await AppDbContext.Taula
            .Where(t =>
                t.TaulaPare == null &&
                t.INTERIOREXTERIOR == interExt &&
                t.NumTaula >= numTaulaNou &&
                t.NumTaula < 99)
            .ToListAsync();

        foreach (var t in taulesAReordenar)
            t.NumTaula += 1;

        await AppDbContext.SaveChangesAsync();
    }

    private async Task ReordenarDespresEliminarAsync(int numTaula, char interExt)
    {
        var taulesAReordenar = await AppDbContext.Taula
            .Where(t =>
                t.TaulaPare == null &&
                t.INTERIOREXTERIOR == interExt &&
                t.NumTaula > numTaula &&
                t.NumTaula <= 99)
            .ToListAsync();

        foreach (var t in taulesAReordenar)
            t.NumTaula -= 1;

        await AppDbContext.SaveChangesAsync();
    }

    
    private async Task ReordenarNumTaulaEnModificarAsync(Taula taula, int numNou)
    {
        var interExt = taula.INTERIOREXTERIOR;
        var numActual = taula.NumTaula;

        if (numNou == numActual) return;

        if (numNou < numActual)
        {
            var taulesAReordenar = await AppDbContext.Taula
                .Where(t =>
                    t.TaulaPare == null &&
                    t.INTERIOREXTERIOR == interExt &&
                    t.IdTaula != taula.IdTaula &&
                    t.NumTaula >= numNou &&
                    t.NumTaula < numActual)
                .ToListAsync();

            foreach (var t in taulesAReordenar) t.NumTaula += 1;
        }
        else
        {
            var taulesAReordenar = await AppDbContext.Taula
                .Where(t =>
                    t.TaulaPare == null &&
                    t.INTERIOREXTERIOR == interExt &&
                    t.IdTaula != taula.IdTaula &&
                    t.NumTaula <= numNou &&
                    t.NumTaula > numActual)
                .ToListAsync();

            foreach (var t in taulesAReordenar) t.NumTaula -= 1;
        }

        await AppDbContext.SaveChangesAsync();
    }
    

    
}
