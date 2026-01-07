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

    [HttpGet("getResumDiari")] 
    public async Task<IActionResult> GetResumDiari([FromQuery] DateOnly diaResum)
    {
        var estadistica = await estadistiquesService.GetResumDiariAsync(diaResum);
        return Ok(estadistica);
    }
    
    [HttpGet("getResumSetmanal")]
    public async Task<ActionResult<EstadistiquesDTO>> GetResumSetmanal([FromQuery] int any, [FromQuery] int setmana)
    {
        var estadistica = await estadistiquesService.GetResumSetmanalAsync(any, setmana);
        return Ok(estadistica);
    }
    
    [HttpGet("getResumMensual")]
    public async Task<ActionResult<EstadistiquesDTO>> GetResumMensual([FromQuery] int any, [FromQuery] int mes)
    {
        var res = await estadistiquesService.GetResumMensualAsync(any, mes);
        return Ok(res);
    }

    [HttpGet("getResumAnual")]
    public async Task<ActionResult<EstadistiquesDTO>> GetResumAnual([FromQuery] int any)
    {
        var res = await estadistiquesService.GetResumAnualAsync(any);
        return Ok(res);
    }

}