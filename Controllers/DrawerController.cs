using Microsoft.AspNetCore.Mvc;
using PFG_BackEnd.ModelsDTO;
using PFG_BackEnd.Service;

namespace PFG_BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DrawerController : ControllerBase
{
    private readonly CashDrawerService drawerService;
    private readonly ComandaPagadaService comandaPagadaService;


    public DrawerController(CashDrawerService drawer, ComandaPagadaService service)
    {
        drawerService = drawer;
        comandaPagadaService = service;
    }

    [HttpPost("open")]
    public IActionResult Open()
    {
        drawerService.OpenDrawer();
        return Ok(new { ok = true });
    }
    
    [HttpPost("sample-ticket")]
    public IActionResult PrintSampleTicket()
    {
        drawerService.PrintSampleTicket();
        return Ok(new { ok = true });
    }

    [HttpPost("ticketFinal")]
    public async Task<IActionResult> PrintTicketFinal([FromBody] int idComanda)
    {
        var linies = await comandaPagadaService.GetLiniesTicketAsync(idComanda);
        drawerService.PrintTicketFinalAsync(linies);
        return Ok(new { ok = true });
    }
    
    [HttpPost("ticketCuina")]
    public IActionResult PrintTicketCuina([FromBody] EnviarCuinaDTO dto)
    {
        drawerService.PrintTicketCuina(dto);
        return Ok(new { ok = true });
    }
}