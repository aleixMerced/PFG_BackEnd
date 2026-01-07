using Microsoft.AspNetCore.Mvc;
using PFG_BackEnd.Models;
using PFG_BackEnd.ModelsDTO;
using PFG_BackEnd.Service;


namespace PFG_BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TipusProducteController : Controller
    {
        private readonly TipusProducteService tipusProducteService;

        public TipusProducteController(TipusProducteService service)
        {
            tipusProducteService = service;
        }

        [HttpGet("GetNomTipus")]
        public async Task<IActionResult> GetTipusProducte()
        {
            var tipus = await tipusProducteService.GetAllAsync();
            return Ok(tipus);
        }
        
        [HttpPost("PostTipusProducte")]
        public async Task<IActionResult> PostTipusProducte([FromForm] TipusProducteNewDTO dto)
        {
            var producte = await tipusProducteService.CrearTipusProducteAsync(dto);

            return Ok(producte);
        }

        [HttpGet("GetTipusById")]
        public async Task<IActionResult> GetProducteById(int id)
        {
            var producte = await tipusProducteService.GetByIDAsync(id);

            return Ok(producte);
        }

        [HttpDelete("DeleteTipusProducte")]
        public async Task<IActionResult> BorrarTipusProducteById(int id)
        {
            var producte = await tipusProducteService.DeleteTipusByIDAsync(id);
            return Ok(producte);
        }
        
        [HttpPut("PutTipusProducte")]
        public async Task<IActionResult> PutTipusProducte([FromForm] TipusProducteNewDTO dto)
        {
            var producte = await tipusProducteService.ActualitzarTipusProducteAsync(dto);

            return Ok(producte);
        }
    }
}