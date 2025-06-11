using Microsoft.AspNetCore.Mvc;
using PFG_BackEnd.Service;

namespace PFG_BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProducteController : Controller
    {
        private readonly ProducteService producteService;

        public ProducteController(ProducteService service)
        {
            producteService = service;
        }

        [HttpGet("GetProductes")]
        public async Task<IActionResult> GetProductes()
        {
            var productes = await producteService.GetAllAsync();
            return Ok(productes);
        }
    }

}
