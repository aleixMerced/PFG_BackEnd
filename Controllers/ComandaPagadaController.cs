using Microsoft.AspNetCore.Mvc;
using PFG_BackEnd.Models;
using PFG_BackEnd.ModelsDTO;
using PFG_BackEnd.Service;

namespace PFG_BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComandaPagadaController : ControllerBase
{
    
    private readonly ComandaPagadaService comandaPagadaService;
    
    public ComandaPagadaController(ComandaPagadaService service)
    {
        comandaPagadaService = service;
    }
    
    [HttpGet("GetProducteComandaPagada")]
    public async Task<IActionResult> GetProducteComandaPagada(int idComanda)
    {
        if (idComanda == null)
        {
            return BadRequest("no existeix la comanda");
        }

        var comandaProducte = await comandaPagadaService.GetProducteComanda(idComanda);

        return Ok(comandaProducte);
    }

    [HttpGet("GetProductComandaPagada")]
    public async Task<IActionResult> GetProductComandaPagada(int idComanda)
    {
        var comadna = await comandaPagadaService.GetLiniesTicketAsync(idComanda);
        return Ok(comadna);
    }
    
}