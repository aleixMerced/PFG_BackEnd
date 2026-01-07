using Microsoft.AspNetCore.Mvc;
using PFG_BackEnd.ModelsDTO;
using PFG_BackEnd.Service;

namespace PFG_BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CaixaDiariaController : ControllerBase
{
    private readonly CaixaDiariaService caixaDiariaService;

    public CaixaDiariaController(CaixaDiariaService diariaService)
    {
        caixaDiariaService = diariaService;
    }

    [HttpGet("ResumMati")]
    public async Task<ActionResult<CaixaDTO>> GetResumMati()
    {
        var resum = await caixaDiariaService.GetResumMatiAsync();
        return Ok(resum);
    }
    
    [HttpGet("ResumTarda")]
    public async Task<ActionResult<CaixaDTO>> GetResuTarda()
    {
        var resum = await caixaDiariaService.GetResuTardaAsync();
        return Ok(resum);
    }
    
    [HttpPost("PostCaixaMati")]
    public async Task<ActionResult<CaixaDTO>> PostCaixaMati([FromBody] CaixaDTO caixa)
    {
        var resum = await caixaDiariaService.PostCaixaMatiAsync(caixa);
        return Ok(resum);
    }
    
    [HttpPut("PostCaixaTotal")]
    public async Task<ActionResult<CaixaDTO>> PostCaixaTotal([FromBody] CaixaDTO caixa)
    {
        var resum = await caixaDiariaService.PostCaixaTotalAsync(caixa);
        return Ok(resum);
    }
    
    [HttpGet("EstatCaixaDiaria")]
    public async Task<ActionResult<bool>> GetEstatCaixaDiaria()
    {
        var haFetCaixaMati = await caixaDiariaService.HaFetCaixaMatiAvuiAsync();
        return Ok(haFetCaixaMati);
    }
    
    [HttpGet("EstatCaixaFinal")]
    public async Task<ActionResult<bool>> EstatCaixaFinal()
    {
        var haFetCaixaMati = await caixaDiariaService.HaFetCaixaFinalAvuiAsync();
        return Ok(haFetCaixaMati);
    }
}