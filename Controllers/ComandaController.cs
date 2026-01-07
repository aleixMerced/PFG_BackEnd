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


        [HttpGet("GetComandaByTaulaPaginada")]
        public async Task<ActionResult<List<ComandaDTO>>> GetComandaByTaulaPaginada(int idTaula, int page, int pageSize, string? dataInici, string? dataFinal, string? filtreGlobal, string? estat, string? formaPagament, double? importMinim, double? importMaxim)
        {
            var llista = await comandaService.GetComandaByTaulaPaginadaAsync(idTaula, page, pageSize, dataInici, dataFinal, filtreGlobal, estat, formaPagament, importMinim, importMaxim);
            return Ok(llista);
        }
        
        [HttpGet("GetCountComandaByTaula")]
        public async Task<ActionResult<int>> GetCountComandaByTaula (int idTaula, int page, int pageSize, string? dataInici, string? dataFinal, string? filtreGlobal, string? estat, string? formaPagament, double? importMinim, double? importMaxim)
        {
            var total = await comandaService.GetCountComandaByTaulaAsync(idTaula, dataInici, dataFinal, filtreGlobal, estat, formaPagament, importMinim, importMaxim);
            return Ok(total);
        }

        
        [HttpGet("GetComandaByNom")]
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
            if (comanda.IdComanda == 0 || comanda.PreuComanda < 0)
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
            var lastId = await comandaService.GetLastIdAsync();
            return Ok(lastId + 1);
        }

        [HttpPost("PostProducteComanda")]
        public async Task<IActionResult> PostProducteComanda([FromBody] ComandaLiniaDTO comanda)
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
        
        [HttpGet("GetAllProducteComanda")]

        public async Task<IActionResult> GetAllProducteComanda(int idComanda)
        {
            if (idComanda == null)
            {
                return BadRequest("no existeix la comanda");
            }

            var comandaProducte = await comandaService.GetAllProducteComandaAsync(idComanda);

            return Ok(comandaProducte);
        }

        [HttpPut("PutComanda")]
        public async Task<IActionResult> PutComanda([FromBody] ComandaUpdateDto  comanda)
        {
            if (comanda == null)
            {
                return BadRequest("no existeix la comanda");
            }

            var comandaActualitzada = await comandaService.ActualitzarComanda(comanda);
            
            return Ok(comandaActualitzada);

        }

        [HttpDelete("EsborrarComanda")]
        public async Task<IActionResult> esborrarComanda(int idComanda)
        {
            if (idComanda <= 0)
            {
                return BadRequest("no existeix la comanda");
            }

            var resultat = await comandaService.esborrarComanda(idComanda);
            
            return Ok(resultat);
        }

        //LINIA COMANDA

        [HttpPut("ActualitzarLiniaComanda")]
        public async Task<IActionResult> ActualitzarLiniaComanda([FromQuery] int idComanda,  [FromQuery] int idProducte,  [FromBody] ComandaLiniaDTO dto)
        {
            if (dto == null) return BadRequest("Cos buit.");
            if (dto.quantitat < 0 || dto.preuMoment < 0)
                return BadRequest("Valors negatius no permesos.");
            
            var ok = await comandaService.ActualitzarLiniaAsync(idComanda, idProducte, dto);

            if (!ok) return NotFound("Línia no trobada.");
            return Ok(true);

        }

        [HttpGet("GetLiniaComanda")]
        public async Task<IActionResult> GetLiniaComanda([FromQuery] int idComanda, [FromQuery] bool pagades, CancellationToken ct = default)
        {
            if (idComanda <= 0) return BadRequest("idComanda invàlid.");

            var llistat = await comandaService.GetLiniesAsync(idComanda, pagades, ct);
            return Ok(llistat);
        }
        
        [HttpDelete("DeleteLiniaComanda")]
        public async Task<IActionResult> DeleteLiniaComanda([FromQuery] int idComanda, [FromQuery] int idProducte, [FromQuery] int quantitat, CancellationToken ct = default)
        {
            if (idComanda <= 0 || idProducte <= 0 || quantitat < 0)
                return BadRequest("Paràmetres invàlids.");

            var eliminat = await comandaService.DeleteLiniaComanda(idComanda, idProducte, quantitat, ct);
            return eliminat ? NoContent() : NotFound();
        }
        
        
        //LINIA COMANDA PAGADA

        [HttpPost("AfegirLiniaComandaPagada")]
        public async Task<IActionResult> PostLiniaComandaPagada([FromBody] ComandaLiniaDTO body,
            CancellationToken ct = default)
        {
            if (body == null) return BadRequest("no existeix la comanda");

            var linia = await comandaService.PostLiniaPagadaComandaAsync(body, ct);
            
            return Ok(linia);
        }

        [HttpPut("ActualitzarLiniaComandaPagada")]
        public async Task<IActionResult> PutLiniaComandaPgada([FromBody] ComandaLiniaDTO body,
            CancellationToken ct = default)
        {
            if (body == null) return BadRequest("no existeix la comanda");
            
            var linia = await comandaService.PutLiniaPagadaComandaAsync(body, ct);
            
            return Ok(linia);
            
        }
        
        [HttpPost("AfegirPagament")]
        public async Task<IActionResult> AfegirPagament([FromBody] ComandaPagadaNewDTO dto)
        {
            await comandaService.AfegirPagamentAsync(dto);
            return Ok();
        }
        
        [HttpPut("FinalitzarPagament")]
        public async Task<IActionResult> FinalitzarPagament([FromBody] ComandaPagadaNewDTO  body)
        {
            int idComanda = body.IdComanda;
            string tipusPagament = body.TipusPagament;
            double total = body.Total;

            var ok = await comandaService.FinalitzarPagamentAsync(idComanda, tipusPagament, total);
            return Ok(ok);
        }
        
        [HttpPost("CancelarComanda")]
        public async Task<IActionResult> CancelarComanda([FromQuery] int idComanda)
        {
            try
            {
                var ok = await comandaService.CancelarComandaAsync(idComanda);

                return Ok(ok);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        

    }
    
    

    
    
    
}
