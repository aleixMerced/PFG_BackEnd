using Microsoft.AspNetCore.Mvc;
using PFG_BackEnd.Models;
using PFG_BackEnd.ModelsDTO;
using PFG_BackEnd.Service;

namespace PFG_BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComandaController : ControllerBase
    {
        
        private readonly ComandaService comandaService;
        
        
        public ComandaController(ComandaService service)
        {
            comandaService = service;
        }
        
        [HttpGet("GetComandes")]
        public async Task<IActionResult> GetComandes()
        {
            var comandes = await comandaService.GetAllAsync();
            return Ok(comandes);
        }

        [HttpGet("GetComandaByNom/{NomComanda}")]
        public async Task<IActionResult> GetComandaByNomComanda(string NomComanda)
        {
            var comanda = await comandaService.GetComandaByName(NomComanda);
            
            return Ok(comanda);
        }

        [HttpGet("GetComandaByID")]
        public async Task<IActionResult> GetComandaByID(int idComanda)
        {
            var comanda = await comandaService.GetComandaByID(idComanda);
            
            return Ok(comanda);
        }

        [HttpPost("PostComanda")]
        public async Task<IActionResult> PostComanda([FromBody] Comanda comanda)
        {
            if (string.IsNullOrEmpty(comanda.NomClient) || comanda.IdComanda == 0 || comanda.PreuComanda < 0)
            {
                return BadRequest("Faltan dades de la comanda");
            }
            
            
            try
            {
                
                var comandaCreada = await comandaService.CreateComanda(comanda);

                return CreatedAtAction(nameof(PostComanda), new { id = comandaCreada.IdComanda }, comandaCreada);

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al procesar la comanda: " + ex.Message);
            }
        }

        [HttpGet("GetLastID")]
        public async Task<IActionResult> GetLastId()
        {
            var lastComanda = await comandaService.GetLastID();

            if (lastComanda == null)
            {
                return Ok(1);
            }

            return Ok(lastComanda.IdComanda + 1);
        }

        [HttpPost("PostProducteComanda")]
        public async Task<IActionResult> PostProducteComanda([FromBody] ComandaProducteDTO comanda)
        { 
            if (comanda.idProducte < 0 || comanda.idComanda < 0 || comanda.quantitat < 0)
            {
                return BadRequest("Comanda mal ");
            }
            
            
            
            var crearProducteComanda = await comandaService.AfegirProducteComanda(comanda.idProducte, comanda.quantitat, comanda.idComanda, comanda.preuMoment);


            return CreatedAtAction(nameof(PostProducteComanda),
                new { idComanda = crearProducteComanda.IdComanda, idProducte = crearProducteComanda.IdProducte },
                crearProducteComanda);
        }
        
        [HttpGet("GetProducteComanda")]

        public async Task<IActionResult> GetProducteComanda(int idComanda)
        {
            if (idComanda == null)
            {
                return BadRequest("no existeix la comanda");
            }

            var comandaProducte = await comandaService.GetProducteComanda(idComanda);

            return Ok(comandaProducte);
        }

        [HttpPut("PutComanda")]
        public async Task<IActionResult> PutComanda([FromBody] Comanda comanda)
        {
            if (comanda == null)
            {
                return BadRequest("no existeix la comanda");
            }

            var comandaActualitzada = await comandaService.ActualitzarComanda(comanda);
            
            return Ok(comandaActualitzada);

        }
    }
}
