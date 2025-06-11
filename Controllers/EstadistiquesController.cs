using Microsoft.AspNetCore.Mvc;
using PFG_BackEnd.Models;
using PFG_BackEnd.ModelsDTO;
using PFG_BackEnd.Service;


namespace PFG_BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstadistiquesController : ControllerBase
{
    
    private readonly EstadistiquesService estadistiquesService;

    public EstadistiquesController(EstadistiquesService service)
    {
        estadistiquesService = service;
    }

    [HttpGet("GetEstadisticaByTipusDia")]
    public async Task<IActionResult> GetEstadisticaByTipusDia([FromQuery] string tipus, [FromQuery] DateOnly date)
    {
        var estadistica = await estadistiquesService.GetEstadisticaByTipusDia(tipus, date);

        return Ok(estadistica);
    }

    [HttpPost("AugmentarCaixa")]
    public async Task<IActionResult> AugmentarCaixa(decimal id, decimal augment)
    {
        try
        {
            await estadistiquesService.IncrementarPreuAsync(id, augment);
            return Ok();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        
    }
    [HttpGet("GetAllTipusDates")]
    public async Task<ActionResult<IEnumerable<EstadistiquesDTO>>> GetAllTipusDates()
    {
        var result = await estadistiquesService.GetAllTipusDatesAsync();
        return Ok(result);
    }
}