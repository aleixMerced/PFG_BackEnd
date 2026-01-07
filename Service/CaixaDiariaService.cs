using Microsoft.EntityFrameworkCore;
using PFG_BackEnd.Models;
using PFG_BackEnd.ModelsDTO;

namespace PFG_BackEnd.Service;

public class CaixaDiariaService : ServiceCollection
{
    private readonly AppDbContext AppDbContext;
    private readonly ILogger<CaixaDiariaService> logger;

    public CaixaDiariaService(AppDbContext context, ILogger<CaixaDiariaService> log)
    {
        AppDbContext = context;
        logger = log;
    }
    
    public async Task<CaixaDTO> GetResumMatiAsync()
    {
        var avui = DateTime.Today;
        var inici = avui.AddHours(5);
        var final = avui.AddHours(17);
        
        logger.LogInformation("Calculant resum matí. Rang: {Inici} - {Final}", inici, final);

        var ssql = AppDbContext.ComandaPagaments
            .Where(c => c.DataPagament >= inici && c.DataPagament < final);
        
        var totalEfectiu = await ssql
            .Where(p => p.TipusPagament == "E")
            .SumAsync(p => (decimal?)p.Import) ?? 0m;

        var totalTargeta = await ssql
            .Where(p => p.TipusPagament == "T")
            .SumAsync(p => (decimal?)p.Import) ?? 0m;
        
        logger.LogInformation("Resum matí calculat. Efectiu={Efectiu} Targeta={Targeta}", totalEfectiu, totalTargeta);

        return new CaixaDTO
        {
            TotalEfectiu = totalEfectiu,
            TotalTargeta = totalTargeta,
            TotalDia = totalEfectiu + totalTargeta,
            Observacions = ""
        };
        
    }
    
    public async Task<CaixaDTO> GetResuTardaAsync()
    {
        var avui = DateTime.Today;             
        var inici = avui.AddHours(17);         
        var demà = avui.AddDays(1);            
        var final = demà.AddHours(5);   

        var ssql = AppDbContext.ComandaPagaments
            .Where(c => c.DataPagament >= inici && c.DataPagament < final);
        
        var totalEfectiu = await ssql
            .Where(p => p.TipusPagament == "E")
            .SumAsync(p => (decimal?)p.Import) ?? 0m;

        var totalTargeta = await ssql
            .Where(p => p.TipusPagament == "T")
            .SumAsync(p => (decimal?)p.Import) ?? 0m;
        
        return new CaixaDTO
        {
            TotalEfectiu = totalEfectiu,
            TotalTargeta = totalTargeta,
            TotalDia = totalEfectiu + totalTargeta,
            Observacions = ""
        };
        
    }
    
    public async Task<CaixaDTO> PostCaixaMatiAsync(CaixaDTO caixa)
    {
        var avui = DateTime.Today;

        string? obsMati = string.IsNullOrWhiteSpace(caixa.Observacions)
            ? null
            : $"Mati observacions: {caixa.Observacions}";

        var novaCaixa = new CaixaDiaria()
        {
            DataCaixa    = avui,

            MatiTargeta  = caixa.TotalTargeta,
            MatiEfectiu  = caixa.TotalEfectiu,
            MatiTotal    = caixa.TotalDia,

            TotalDia     = caixa.TotalDia,

            Observacions = obsMati
        };

        AppDbContext.CaixesDiaria.Add(novaCaixa);
        await AppDbContext.SaveChangesAsync();

        return new CaixaDTO
        {
            TotalEfectiu  = caixa.TotalEfectiu,
            TotalTargeta  = caixa.TotalTargeta,
            TotalDia      = caixa.TotalDia,
            Observacions  = caixa.Observacions
        };
    }
    
    public async Task<CaixaDTO> PostCaixaTotalAsync(CaixaDTO caixa)
    {
        var avui = DateTime.Today;

        var caixaDb = await AppDbContext.CaixesDiaria
            .FirstOrDefaultAsync(c => c.DataCaixa == avui);

        if (caixaDb == null)
        {
            caixaDb = new CaixaDiaria
            {
                DataCaixa = avui
            };
            AppDbContext.CaixesDiaria.Add(caixaDb);
        }

        caixaDb.TardaTargeta = caixa.TotalTargeta;
        caixaDb.TardaEfectiu = caixa.TotalEfectiu;
        caixaDb.TardaTotal   = caixa.TotalDia;

        var matiTotal  = caixaDb.MatiTotal  ?? 0m;
        var tardaTotal = caixaDb.TardaTotal ?? 0m;
        caixaDb.TotalDia = matiTotal + tardaTotal;

        if (!string.IsNullOrWhiteSpace(caixa.Observacions))
        {
            var obsTarda = $"Tarda observacions: {caixa.Observacions}";

            if (string.IsNullOrWhiteSpace(caixaDb.Observacions))
            {
                caixaDb.Observacions = obsTarda;
            }
            else
            {
                caixaDb.Observacions += Environment.NewLine + obsTarda;
            }
        }

        await AppDbContext.SaveChangesAsync();

        return new CaixaDTO
        {
            TotalEfectiu = caixaDb.TardaEfectiu ?? 0m,    
            TotalTargeta = caixaDb.TardaTargeta ?? 0m,
            TotalDia     = caixaDb.TotalDia ?? 0m,
            Observacions = caixaDb.Observacions
        };
    }
    public async Task<bool> HaFetCaixaMatiAvuiAsync()
    {
        var avui = DateTime.Today;

        return await AppDbContext.CaixesDiaria
            .AnyAsync(c =>
                c.DataCaixa.Date == avui &&
                c.MatiTotal != null);
    }
    
    public async Task<bool> HaFetCaixaFinalAvuiAsync()
    {
        var avui = DateTime.Today;

        return await AppDbContext.CaixesDiaria
            .AnyAsync(c =>
                c.DataCaixa.Date == avui &&
                c.TardaTotal != null);
    }
}