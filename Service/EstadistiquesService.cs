using Microsoft.EntityFrameworkCore;
using PFG_BackEnd.Models;
using PFG_BackEnd.ModelsDTO;
using System.Globalization;

namespace PFG_BackEnd.Service;

public class EstadistiquesService : ServiceCollection
{
    private readonly AppDbContext AppDbContext;

    public EstadistiquesService(AppDbContext appDbContext)
    {
        AppDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));

    }
    
    private async Task<EstadistiquesDTO> BuildResumAsync(DateTime inici, DateTime fi)
    {
        var total = await AppDbContext.CaixesDiaria
            .Where(x => x.DataCaixa >= inici && x.DataCaixa < fi)
            .SumAsync(x => (decimal?)x.TotalDia) ?? 0m;

        var linies = AppDbContext.ComandaLiniaPagada
            .Where(cl => cl.Comanda.DataComanda >= inici && cl.Comanda.DataComanda < fi);

        var perProducte = linies
            .GroupBy(cl => new { cl.IdProducte, cl.Producte.NomProducte, cl.Producte.IdTipus })
            .Select(g => new
            {
                ProducteId = g.Key.IdProducte,
                Nom = g.Key.NomProducte, 
                IdTipus = g.Key.IdTipus,
                TotalQuantitat = g.Sum(x => x.Quantitat)
            });

        var productesTotals = await perProducte
            .SumAsync(x => (int?)x.TotalQuantitat) ?? 0;

        var menusFets = await perProducte
            .Where(x => x.IdTipus == 15)
            .SumAsync(x => (int?)x.TotalQuantitat) ?? 0;

        var mesVenut = await perProducte
            .OrderByDescending(x => x.TotalQuantitat)
            .ThenBy(x => x.Nom)
            .FirstOrDefaultAsync();

        return new EstadistiquesDTO
        {
            Total = total,
            ProductesTotals = productesTotals,
            MenusFets = menusFets,
            ProducteMesVenutId = mesVenut?.ProducteId,
            NomMesVenut = mesVenut?.Nom?.Trim(),
            UnitatsMesVenut = mesVenut?.TotalQuantitat
        };
    }

    public async Task<EstadistiquesDTO> GetResumDiariAsync(DateOnly diaResum)
    {
        var dia = diaResum.ToDateTime(TimeOnly.MinValue);
        var inici = DateTime.SpecifyKind(dia, DateTimeKind.Unspecified);
        var fi = inici.AddDays(1);
        return await BuildResumAsync(inici, fi);
    }

    public async Task<EstadistiquesDTO> GetResumSetmanalAsync(int isoAny, int isoSetmana)
    {
        var inici = ISOWeek.ToDateTime(isoAny, isoSetmana, DayOfWeek.Monday);
        inici = DateTime.SpecifyKind(inici, DateTimeKind.Unspecified);
        var fi = inici.AddDays(7);

        return await BuildResumAsync(inici, fi);
    }
    
    public async Task<EstadistiquesDTO> GetResumMensualAsync(int any, int mes)
    {
        if (mes < 1 || mes > 12)
            throw new ArgumentOutOfRangeException(nameof(mes), "El mes ha d'estar entre 1 i 12.");

        var inici = new DateTime(any, mes, 1, 0, 0, 0);
        inici = DateTime.SpecifyKind(inici, DateTimeKind.Unspecified);

        var fi = inici.AddMonths(1);

        return await BuildResumAsync(inici, fi);
    }

    public async Task<EstadistiquesDTO> GetResumAnualAsync(int any)
    {
        var inici = new DateTime(any, 1, 1, 0, 0, 0);
        inici = DateTime.SpecifyKind(inici, DateTimeKind.Unspecified);

        var fi = inici.AddYears(1);

        return await BuildResumAsync(inici, fi);
    }


}