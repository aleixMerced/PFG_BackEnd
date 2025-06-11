using Microsoft.AspNetCore.Mvc;
using PFG_BackEnd.Models;
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
    }
}