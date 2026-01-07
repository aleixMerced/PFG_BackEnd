using Microsoft.AspNetCore.Mvc;
using PFG_BackEnd.ModelsDTO;
using PFG_BackEnd.Service;

namespace PFG_BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly MenuService menuservice;

    public MenuController(MenuService menuService)
    {
        menuservice = menuService;
    }

    [HttpGet("GetPlatsMenu")]
    public async Task<ActionResult<IEnumerable<PlatsMenuDTO>>> GetPlatsMenu(int? idMenu, DateOnly? dia)
    {
        var diaEf = dia ?? DateOnly.FromDateTime(DateTime.Today);
        
        var plats = await menuservice.GetPlatsMenuAsync(idMenu, diaEf);
        return Ok(plats);
    }

    [HttpGet("GetMenuDia")]
    public async Task<ActionResult<IEnumerable<PlatsMenuDTO>>> GetMenuDia(DateOnly diaMenu)
    {
        var plats = await menuservice.GetMenuDiaAsync(diaMenu);
        return Ok(plats);
    }
    
    [HttpPost("SaveMenuDia")]
    public async Task<ActionResult> PostMenuDia([FromBody] PlatsMenuNewDTO dto)
    {
        if (dto == null)
            return BadRequest("Body buit.");

        dto.Primers = dto.Primers.Distinct().ToList();
        dto.Segons  = dto.Segons.Distinct().ToList();

        await menuservice.SaveMenuDiaAsync(dto);

        return Ok();
    }

    [HttpDelete("DeleteMenuDia")]

    public async Task<ActionResult> DeleteMenuDia(DateOnly diaMenu, int idMenu)
    {
        var plat = await menuservice.DeleteMenuDiaAsync(diaMenu, idMenu);
        return Ok(plat);
    }

}