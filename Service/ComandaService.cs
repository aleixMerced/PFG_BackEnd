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
                Pagat = c.Pagat,
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
                Pagat = c.Pagat,
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
                IDTaula = c.IDTaula,
                Pagat = c.Pagat

            })
            .FirstOrDefaultAsync(); 
    }
    
    public async Task<Comanda> CreateComanda(Comanda comanda)
    {
        AppDbContext.Comandes.Add(comanda);
        await AppDbContext.SaveChangesAsync();

        return comanda;
    }
    
    public async Task<int> GetLastIdAsync()
    {
        var lastId = await AppDbContext.Comandes
            .MaxAsync(c => (int?)c.IdComanda);

        return lastId ?? 0;
    }
    
    public async Task<Comanda_Linia> AfegirProducteComanda(int idProducte, int quantitat, int idComanda, decimal preuMoment)
    {
        var existeixComanda = await AppDbContext.Comandes.AnyAsync(c => c.IdComanda == idComanda);
        var existeixProducte = await AppDbContext.Productes.AnyAsync(p => p.IdProducte == idProducte);

        if (!existeixComanda || !existeixProducte)
        {
            throw new InvalidOperationException($"No existeix la comanda o el producte");
        }

        var comandaProducte = await AppDbContext.ComandaLinia
            .FirstOrDefaultAsync(c => c.IdComanda == idComanda && c.IdProducte == idProducte);

        if (comandaProducte != null)
        {
            //Si ja existeix el producte en aquella comanda 
            comandaProducte.PreuMoment = preuMoment;
            comandaProducte.Quantitat = quantitat;
            await AppDbContext.SaveChangesAsync();
            return comandaProducte;
        } 
        
        var comprod = new Comanda_Linia()
        {
            IdComanda = idComanda,
            IdProducte = idProducte,
            Quantitat = quantitat,
            PreuMoment = preuMoment
        };
        AppDbContext.ComandaLinia.Add(comprod);
        await AppDbContext.SaveChangesAsync(); 

        return comprod;

    }

    public async Task<List<ProducteComandaDTO>> GetProducteComanda(int idComanda)
    {
        var productes = await AppDbContext.ComandaLinia
            .Where(cl =>
                cl.IdComanda == idComanda &&                        
                !AppDbContext.ComandaLiniaPagada.Any(clp =>                    
                    clp.IdComanda  == cl.IdComanda &&
                    clp.IdProducte == cl.IdProducte))
            .Include(cl => cl.Producte)  
            .ThenInclude(p => p.Tipus)    
            .Select(cl => new ProducteComandaDTO
            {
                IdProducte     = cl.Producte.IdProducte,
                NomProducte    = cl.Producte.NomProducte.Trim(),
                Estoc          = cl.Producte.Estoc,
                ImatgeProducte = cl.Producte.ImatgeProducte.Trim(),
                PreuVenta      = cl.Producte.PreuVenta,
                PreuCompra     = cl.Producte.PreuCompra,
                MinimEstoc     = cl.Producte.MinimEstoc,
                NomTipus       = cl.Producte.Tipus.NomTipus.Trim(),
                Quantitat      = cl.Quantitat,
                PreuMoment     = cl.PreuMoment
            })
            .ToListAsync();

        return productes;
    }

    public async Task<List<ProducteComandaDTO>> GetAllProducteComandaAsync(int idComanda)
    {
        var liniesNoPagades = AppDbContext.ComandaLinia
            .Where(cl => cl.IdComanda == idComanda)
            .Select(cl => new
            {
                cl.IdProducte,
                cl.Quantitat,
                cl.PreuMoment
            });

        var liniesPagades = AppDbContext.ComandaLiniaPagada
            .Where(clp => clp.IdComanda == idComanda)
            .Select(clp => new
            {
                clp.IdProducte,
                clp.Quantitat,
                clp.PreuMoment
            });

        var agrupat = liniesNoPagades
            .Concat(liniesPagades)                 // UNION ALL
            .GroupBy(x => x.IdProducte)
            .Select(g => new
            {
                IdProducte      = g.Key,
                QuantitatTotal  = g.Sum(x => x.Quantitat),
                PreuMomentTotal = g.Sum(x => x.PreuMoment)
            });

        var query = agrupat
            .Join(
                AppDbContext.Productes.Include(p => p.Tipus),
                g => g.IdProducte,
                p => p.IdProducte,
                (g, p) => new ProducteComandaDTO
                {
                    IdProducte     = p.IdProducte,
                    NomProducte    = p.NomProducte.Trim(),
                    Estoc          = p.Estoc,
                    ImatgeProducte = p.ImatgeProducte.Trim(),
                    PreuVenta      = p.PreuVenta,
                    PreuCompra     = p.PreuCompra,
                    MinimEstoc     = p.MinimEstoc,
                    NomTipus       = p.Tipus.NomTipus.Trim(),

                    Quantitat      = g.QuantitatTotal,
                    PreuMoment     = g.PreuMomentTotal
                });

        return await query.ToListAsync();
    }

    public async Task<Comanda> ActualitzarComanda(ComandaUpdateDto novaComanda)
    {
        var antiguaComanda = await AppDbContext.Comandes.FindAsync(novaComanda.IdComanda);
        if (antiguaComanda == null) return null;

        if (!string.IsNullOrEmpty(novaComanda.NomClient))
            antiguaComanda.NomClient = novaComanda.NomClient;

        if (!string.IsNullOrEmpty(novaComanda.EstatComanda))
            antiguaComanda.EstatComanda = novaComanda.EstatComanda;

        if (!string.IsNullOrEmpty(novaComanda.TipusPagament))
            antiguaComanda.TipusPagament = novaComanda.TipusPagament;

        if (novaComanda.DataComanda.HasValue)
        {
            if (novaComanda.DataComanda.Value < antiguaComanda.DataComanda)
            {
                antiguaComanda.DataComanda = novaComanda.DataComanda.Value;
            }
        }

        if (novaComanda.DataPagament.HasValue)
            antiguaComanda.DataPagament = novaComanda.DataPagament.Value;

        if (novaComanda.PreuComanda.HasValue)
            antiguaComanda.PreuComanda = novaComanda.PreuComanda.Value;

        if (novaComanda.IDTaula.HasValue)
            antiguaComanda.IDTaula = novaComanda.IDTaula.Value;

        await AppDbContext.SaveChangesAsync();
        return antiguaComanda;
    }
    public async Task<bool> esborrarComanda(int idComanda, CancellationToken ct = default)
    {
        //ESBORRAR COMANDA PRIEMR HAIG D'ESBORRAR TOTES LES LINIES PAGADES I NO PAGADES
        
        //1. esborrar linies pagades

        await using var tx= await AppDbContext.Database.BeginTransactionAsync(ct);
        try
        {
            await AppDbContext.ComandaLiniaPagada
                .Where(l => l.IdComanda == idComanda)
                .ExecuteDeleteAsync(ct);

            // 2) Línies normals
            await AppDbContext.ComandaLinia
                .Where(l => l.IdComanda == idComanda)
                .ExecuteDeleteAsync(ct);

            // 4) Comanda
            var deleted = await AppDbContext.Comandes
                .Where(c => c.IdComanda == idComanda)
                .ExecuteDeleteAsync(ct);

            await tx.CommitAsync(ct);
            return deleted > 0;
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<bool> ActualitzarLiniaAsync(int idComanda, int idProducte, ComandaLiniaDTO dto, CancellationToken ct = default)
    {
        var linia = await AppDbContext.ComandaLinia
            .SingleOrDefaultAsync(l => l.IdComanda == idComanda && l.IdProducte == idProducte, ct);

        if (linia is null)
        {
            if (dto.quantitat <= 0) return false;

            linia = new Comanda_Linia
            {
                IdComanda  = idComanda,
                IdProducte = idProducte,
                Quantitat  = dto.quantitat,
                PreuMoment = dto.preuMoment
            };

            await AppDbContext.ComandaLinia.AddAsync(linia, ct);
        }
        else
        {
            if (dto.quantitat <= 0)
            {
                AppDbContext.ComandaLinia.Remove(linia);
            }
            else
            {
                // Actualitzar línia existent
                linia.Quantitat  = dto.quantitat;
                linia.PreuMoment = dto.preuMoment;
            }
        }

        await AppDbContext.SaveChangesAsync(ct);
        return true;
    }



    public async Task<List<LiniaComandaGeneral>> GetLiniesAsync(int idComanda, bool pagades, CancellationToken ct = default)
    {
        if (!pagades)
        {
            var q =
                from l in AppDbContext.ComandaLinia.AsNoTracking()
                join p in AppDbContext.Productes.AsNoTracking()
                    on l.IdProducte equals p.IdProducte
                where l.IdComanda == idComanda
                select new LiniaComandaGeneral
                {
                    idProducte  = l.IdProducte,
                    nomProducte = p.NomProducte.Trim(),   
                    unitats     = l.Quantitat,
                    preuUnitari = p.PreuVenta,
                    total       = l.PreuMoment, 
                    pagat       = false,
                    preuPagat   = 0m,
                    stockDisponible = p.Estoc,
                    
                };

            return await q.OrderBy(x => x.nomProducte).ToListAsync(ct);
        }
        else
        {
            var q =
                from l in AppDbContext.ComandaLiniaPagada.AsNoTracking()
                join p in AppDbContext.Productes.AsNoTracking()
                    on l.IdProducte equals p.IdProducte
                where l.IdComanda == idComanda
                let encaraPendent = AppDbContext.ComandaLinia
                    .Any(x => x.IdComanda == idComanda && x.IdProducte == l.IdProducte)
                select new LiniaComandaGeneral
                {
                    idProducte  = l.IdProducte,
                    nomProducte = p.NomProducte.Trim(),                 
                    unitats     = l.Quantitat,
                    preuUnitari = p.PreuVenta,
                    total       = l.PreuMoment,      
                    pagat       = !encaraPendent,
                    preuPagat   = l.PreuMoment
                };

            return await q.OrderBy(x => x.nomProducte).ToListAsync(ct);
        }
    }
    
    //LINIES PAGADES

    public async Task<bool> DeleteLiniaComanda(int idComanda, int idProducte, int quantitat, CancellationToken ct = default)
    {
        if (quantitat <= 0) return false;

        var linia = await AppDbContext.ComandaLinia
            .SingleOrDefaultAsync(x => x.IdComanda == idComanda && x.IdProducte == idProducte, ct);

        if (linia is null) return false;

        var novaQuantitat = linia.Quantitat - quantitat;

        if (novaQuantitat <= 0)
        {
            AppDbContext.ComandaLinia.Remove(linia);
        }
        else
        {
            var preuUnitariActual = linia.Quantitat > 0 
                ? linia.PreuMoment / linia.Quantitat 
                : 0m;

            linia.Quantitat  = novaQuantitat;
            linia.PreuMoment = preuUnitariActual * novaQuantitat; 
        }

        await AppDbContext.SaveChangesAsync(ct);
        return true;
    }
    
   
    public async Task<ComandaLiniaDTO> PostLiniaPagadaComandaAsync(
        ComandaLiniaDTO dto,
        CancellationToken ct = default)
    {
        if (dto is null) throw new ArgumentNullException(nameof(dto));

        var existent = await AppDbContext.ComandaLiniaPagada
            .SingleOrDefaultAsync(x =>
                x.IdComanda == dto.idComanda &&
                x.IdProducte == dto.idProducte, ct);

        if (existent != null)
        {
            throw new InvalidOperationException("La línia ja existeix. Utilitza PUT per actualitzar-la.");
        }

        var linia = new Comanda_Linia_Pagada
        {
            IdComanda  = dto.idComanda,
            IdProducte = dto.idProducte,
            Quantitat  = dto.quantitat,
            PreuMoment = dto.preuMoment
        };

        await AppDbContext.ComandaLiniaPagada.AddAsync(linia, ct);
        await AppDbContext.SaveChangesAsync(ct);

        return new ComandaLiniaDTO
        {
            idComanda  = linia.IdComanda,
            idProducte = linia.IdProducte,
            quantitat  = linia.Quantitat,
            preuMoment = linia.PreuMoment
        };
    }
    
    public async Task<ComandaLiniaDTO> PutLiniaPagadaComandaAsync(ComandaLiniaDTO dto, CancellationToken ct = default)
    {
        if (dto is null) throw new ArgumentNullException(nameof(dto));

        var linia = await AppDbContext.ComandaLiniaPagada
            .SingleOrDefaultAsync(x =>
                x.IdComanda == dto.idComanda &&
                x.IdProducte == dto.idProducte, ct);

        if (linia is null)
            throw new KeyNotFoundException("La línia no existeix. Utilitza POST per crear-la.");

        linia.Quantitat += dto.quantitat;    
        linia.PreuMoment += dto.preuMoment;   

        await AppDbContext.SaveChangesAsync(ct);

        return new ComandaLiniaDTO
        {
            idComanda  = linia.IdComanda,
            idProducte = linia.IdProducte,
            quantitat  = linia.Quantitat,
            preuMoment = linia.PreuMoment
        };
    }
    
    public async Task<bool> FinalitzarPagamentAsync(int idComanda, string tipusPagament, double total)
    {
        var comanda = await AppDbContext.Comandes.FindAsync(idComanda);
        if (comanda == null) return false;

        comanda.EstatComanda = "PAGADA";
        comanda.TipusPagament = tipusPagament;
        comanda.Pagat = '1';
        comanda.DataPagament = DateTime.Now;
        comanda.PreuComanda = total;

        await AppDbContext.SaveChangesAsync();
        return true;
    }
    
    public async Task<ComandaPagadaNewDTO> AfegirPagamentAsync(ComandaPagadaNewDTO dto, CancellationToken ct = default)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        var comandaExisteix = await AppDbContext.Comandes
            .AnyAsync(c => c.IdComanda == dto.IdComanda, ct);

        if (!comandaExisteix)
        {
            throw new InvalidOperationException($"No existeix la comanda amb ID {dto.IdComanda}");
        }

        var pagament = new Comanda_Pagament
        {
            IdComanda     = dto.IdComanda,
            TipusPagament = dto.TipusPagament,
            Import        = (decimal)dto.Total,
            DataPagament  = DateTime.Now
        };

        await AppDbContext.ComandaPagaments.AddAsync(pagament, ct);
        await AppDbContext.SaveChangesAsync(ct);

        return new ComandaPagadaNewDTO()
        {
            IdComanda     = pagament.IdComanda,
            TipusPagament = pagament.TipusPagament,
            Total        = (double)pagament.Import,
        };
    }

    
    // HISTÒRIC COMANDES

    public async Task<List<ComandaDTO>> GetComandaByTaulaPaginadaAsync(int idTaula, int page, int pageSize, string? dataInici, string? dataFinal, string? filtreGlobal, string? estat, string? formaPagament, double? importMinim, double? importMaxim)
    {
    if (page <= 0) page = 1;
    if (pageSize <= 0) pageSize = 10;

    IQueryable<Comanda> query = AppDbContext.Comandes.AsNoTracking();

    // idTaula / taules pare-fill
    var idsTaules = await GetIdsTaulesAsync(idTaula);
    if (idsTaules != null)
    {
        if (!idsTaules.Any())
            return new List<ComandaDTO>();

        query = query.Where(c => idsTaules.Contains(c.IDTaula));
    }

    // Rang de dates
    if (!string.IsNullOrWhiteSpace(dataInici) &&
        DateTime.TryParse(dataInici, out var dInici))
    {
        query = query.Where(c => c.DataComanda >= dInici);
    }

    if (!string.IsNullOrWhiteSpace(dataFinal) &&
        DateTime.TryParse(dataFinal, out var dFinal))
    {
        var dFinalExclusive = dFinal.Date.AddDays(1);
        query = query.Where(c => c.DataComanda < dFinalExclusive);
    }

    // Estat (PENDENT, PAGADA, CONVIDAT...)
    if (!string.IsNullOrWhiteSpace(estat))
    {
        var e = estat.Trim().ToUpper();
        query = query.Where(c =>
            c.EstatComanda != null &&
            c.EstatComanda.ToUpper() == e);
    }

    // Forma pagament
    if (!string.IsNullOrWhiteSpace(formaPagament))
    {
        var fp = formaPagament.Trim().ToUpper();

        if (fp == "EFECTIU")
        {
            query = query.Where(c =>
                c.TipusPagament != null &&
                (c.TipusPagament.ToUpper() == "EFECTIU" ||
                 c.TipusPagament.ToUpper() == "EFECTIUTARGETA"));
        }
        else if (fp == "TARGETA")
        {
            query = query.Where(c =>
                c.TipusPagament != null &&
                (c.TipusPagament.ToUpper() == "TARGETA" ||
                 c.TipusPagament.ToUpper() == "EFECTIUTARGETA"));
        }
    }

    // Import mínim / màxim
    if (importMinim.HasValue)
        query = query.Where(c => c.PreuComanda >= importMinim.Value);

    if (importMaxim.HasValue)
        query = query.Where(c => c.PreuComanda <= importMaxim.Value);

    // Filtre global
    if (!string.IsNullOrWhiteSpace(filtreGlobal))
    {
        var f = filtreGlobal.Trim().ToLower();

        query = query.Where(c =>
            c.IdComanda.ToString().Contains(f) ||
            c.IDTaula.ToString().Contains(f) ||
            (c.EstatComanda != null && c.EstatComanda.ToLower().Contains(f)) ||
            (c.TipusPagament != null && c.TipusPagament.ToLower().Contains(f)) ||
            c.PreuComanda.ToString().Contains(f) ||
            (c.NomClient != null && c.NomClient.ToLower().Contains(f))
        );
    }

    var comandesPage = await query
        .OrderByDescending(c => c.DataComanda)
        .ThenByDescending(c => c.IdComanda)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    if (!comandesPage.Any())
        return new List<ComandaDTO>();

    var idsTaulesUsades = comandesPage
        .Select(c => c.IDTaula)
        .Distinct()
        .ToList();

    var taulesUsades = await AppDbContext.Taula
        .Where(t => idsTaulesUsades.Contains(t.IdTaula))
        .ToListAsync();

    var parentIds = taulesUsades
        .Where(t => t.TaulaPare != null)
        .Select(t => t.TaulaPare!.Value)
        .Distinct()
        .ToList();

    var subTaulesPerPare = new Dictionary<int, List<Taula>>();

    if (parentIds.Any())
    {
        var totesLesSubTaules = await AppDbContext.Taula
            .Where(t => t.TaulaPare != null && parentIds.Contains(t.TaulaPare.Value))
            .OrderBy(t => t.TaulaPare)
            .ThenBy(t => t.IdTaula)
            .ToListAsync();

        subTaulesPerPare = totesLesSubTaules
            .GroupBy(t => t.TaulaPare!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    var resultat = new List<ComandaDTO>();

    foreach (var c in comandesPage)
    {
        var taula = taulesUsades.FirstOrDefault(t => t.IdTaula == c.IDTaula);
        string? nomTaula = null;

        if (taula != null)
        {
            if (taula.TaulaPare == null)
            {
                // Taula pare → "2"
                nomTaula = taula.NumTaula.ToString();
            }
            else
            {
                // Taula filla → "2.1", "2.2", ...
                if (subTaulesPerPare.TryGetValue(taula.TaulaPare.Value, out var germanes))
                {
                    var index = germanes.FindIndex(st => st.IdTaula == taula.IdTaula);
                    var posicio = index >= 0 ? index + 1 : 1;
                    nomTaula = $"{taula.NumTaula}.{posicio}";
                }
                else
                {
                    // Per si de cas, fallback
                    nomTaula = taula.NumTaula.ToString();
                }
            }
        }

        resultat.Add(new ComandaDTO
        {
            IdComanda     = c.IdComanda,
            NomClient     = c.NomClient == null     ? string.Empty : c.NomClient.Trim(),
            EstatComanda  = c.EstatComanda == null  ? string.Empty : c.EstatComanda.Trim(),
            TipusPagament = c.TipusPagament == null ? null         : c.TipusPagament.Trim(),
            DataComanda   = c.DataComanda,
            DataPagament  = c.DataPagament,
            PreuComanda   = c.PreuComanda,
            Pagat         = c.Pagat,
            IDTaula       = c.IDTaula,
            nomTaula      = nomTaula
        });
    }

    return resultat;
}

    public async Task<int> GetCountComandaByTaulaAsync(int idTaula, string? dataInici, string? dataFinal, string? filtreGlobal, string? estat, string? formaPagament, double? importMinim, double? importMaxim)
    {
        IQueryable<Comanda> query = AppDbContext.Comandes.AsNoTracking();

        // idTaula / taules pare-fill
        var idsTaules = await GetIdsTaulesAsync(idTaula);
        if (idsTaules != null)
        {
            if (!idsTaules.Any())
                return 0;

            query = query.Where(c => idsTaules.Contains(c.IDTaula));
        }

        // Rang de dates
        if (!string.IsNullOrWhiteSpace(dataInici) &&
            DateTime.TryParse(dataInici, out var dInici))
        {
            query = query.Where(c => c.DataComanda >= dInici);
        }

        if (!string.IsNullOrWhiteSpace(dataFinal) &&
            DateTime.TryParse(dataFinal, out var dFinal))
        {
            var dFinalExclusive = dFinal.Date.AddDays(1);
            query = query.Where(c => c.DataComanda < dFinalExclusive);
        }

        //  Estat
        if (!string.IsNullOrWhiteSpace(estat))
        {
            var e = estat.Trim().ToUpper();
            query = query.Where(c =>
                c.EstatComanda != null &&
                c.EstatComanda.ToUpper() == e);
        }

        // Forma pagament
        if (!string.IsNullOrWhiteSpace(formaPagament))
        {
            var fp = formaPagament.Trim().ToUpper();

            if (fp == "EFECTIU")
            {
                query = query.Where(c =>
                    c.TipusPagament != null &&
                    (c.TipusPagament.ToUpper() == "EFECTIU" ||
                     c.TipusPagament.ToUpper() == "EFECTIUTARGETA"));
            }
            else if (fp == "TARGETA")
            {
                query = query.Where(c =>
                    c.TipusPagament != null &&
                    (c.TipusPagament.ToUpper() == "TARGETA" ||
                     c.TipusPagament.ToUpper() == "EFECTIUTARGETA"));
            }
        }

        // Import mínim
        if (importMinim.HasValue)
        {
            query = query.Where(c => c.PreuComanda >= importMinim.Value);
        }

        // Import màxim
        if (importMaxim.HasValue)
        {
            query = query.Where(c => c.PreuComanda <= importMaxim.Value);
        }

        // Filtre global
        if (!string.IsNullOrWhiteSpace(filtreGlobal))
        {
            var f = filtreGlobal.Trim().ToLower();

            query = query.Where(c =>
                c.IdComanda.ToString().Contains(f) ||
                c.IDTaula.ToString().Contains(f) ||
                (c.EstatComanda != null && c.EstatComanda.ToLower().Contains(f)) ||
                (c.TipusPagament != null && c.TipusPagament.ToLower().Contains(f)) ||
                c.PreuComanda.ToString().Contains(f) ||
                (c.NomClient != null && c.NomClient.ToLower().Contains(f))
            );
        }

        return await query.CountAsync();
    }

    
    private async Task<List<int>?> GetIdsTaulesAsync(int idTaula)
    {
        if (idTaula == 0)
            return null;

        var taula = await AppDbContext.Taula
            .AsNoTracking()
            .Where(t => t.IdTaula == idTaula)
            .Select(t => new { t.IdTaula, t.TaulaPare })
            .FirstOrDefaultAsync();

        if (taula == null)
            return new List<int>();  

        if (taula.TaulaPare == null)
        {
            return await AppDbContext.Taula
                .AsNoTracking()
                .Where(t => t.IdTaula == idTaula || t.TaulaPare == idTaula)
                .Select(t => t.IdTaula)
                .ToListAsync();
        }

        return new List<int> { idTaula };
    }

    public async Task<bool> CancelarComandaAsync(int idComanda)
    {
        var comanda = await AppDbContext.Comandes
            .FirstOrDefaultAsync(c => c.IdComanda == idComanda);

        if (comanda == null)
            return false;
        
        var linies = await AppDbContext.ComandaLinia
            .Where(l => l.IdComanda == idComanda)
            .ToListAsync();
        
        foreach (var linia in linies)
        {
            var prod = await AppDbContext.Productes
                .FirstOrDefaultAsync(p => p.IdProducte == linia.IdProducte);

            if (prod != null)
            {
                prod.Estoc = (prod.Estoc ?? 0) + linia.Quantitat;
            }
        }
        
        AppDbContext.ComandaLinia.RemoveRange(linies);
        AppDbContext.Comandes.Remove(comanda);
        
        await AppDbContext.SaveChangesAsync();

        return true;
        

    }




}