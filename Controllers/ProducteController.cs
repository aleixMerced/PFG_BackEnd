using Microsoft.AspNetCore.Mvc;
using PFG_BackEnd.Models;
using PFG_BackEnd.ModelsDTO;
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
        
        
        [HttpGet("GetPlats")]
        public async Task<ActionResult> GetPlats()
        {
            var productes = await producteService.GetPlatsAsync();
            return Ok(productes);
        }

        [HttpGet("GetProductes")]
        public async Task<IActionResult> GetProductes()
        {
            var productes = await producteService.GetAllAsync();
            return Ok(productes);
        }

        [HttpGet("GetProducteByTipus")]
        public async Task<IActionResult> GetProducteByTipus(string tipus)
        {
            var productes = await producteService.GetProductesByTipus(tipus);
            return Ok(productes);
        }

        [HttpPost("PostProducte")]
        public async Task<IActionResult> PostProducte([FromForm] ProducteNewDTO dto)
        {
            var producte = await producteService.CrearProducteAsync(dto);

            return Ok(producte);
        }

        [HttpGet("GetProducteById")]
        public async Task<IActionResult> GetProducteById(int id)
        {
            var producte = await producteService.GetByIDAsync(id);

            return Ok(producte);
        }

        [HttpDelete("DeleteProducte")]
        public async Task<IActionResult> BorrarProducteById(int id)
        {
            var producte = await producteService.DeleteProductByIDAsync(id);
            return Ok(producte);
        }
        
        [HttpPut("PutProducte")]
        public async Task<IActionResult> PutProducte([FromForm] ProducteNewDTO dto)
        {
            var producte = await producteService.ActualitzarProducteAsync(dto);

            return Ok(producte);
        }

        [HttpPut("UpdateStock")]
        public async Task<ActionResult> UpdateStock([FromBody] UpdateStockDTO dto)
        {
            if (dto == null) return BadRequest(new { message = "Body buit" });

            
            if (!int.TryParse(dto.IdProducte, out int idProducte) || idProducte <= 0)
                return BadRequest(new { message = "IdProducte invàlid" });

            if (!int.TryParse(dto.Quantitat, out int nouStock))
                return BadRequest(new { message = "Quantitat invàlida" });

            var result = await producteService.UpdateStockAsync(idProducte, nouStock);

            if (result == null)
                return NotFound(new { message = "Prodcute no ok" });

            if (!result.Suficient) return Conflict(new { message = result.Message });
            return Ok(new
            {
                newStock = result.NewStock,
                warning = result.Warning,
                message = result.Message
            });            
        }

        
    }

}
