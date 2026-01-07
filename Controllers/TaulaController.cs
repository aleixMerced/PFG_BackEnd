using Microsoft.AspNetCore.Mvc;
using PFG_BackEnd.Models;
using PFG_BackEnd.ModelsDTO;
using PFG_BackEnd.Service;

namespace PFG_BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class TaulaController : ControllerBase
    {
        private readonly TaulaService taulaService;
        
        
        public TaulaController(TaulaService service)
        {
            taulaService = service;
        }
    
        [HttpGet("GetTaulesInterior")]
        public async Task<IActionResult> GetTaulesInt()
        {
            var comandes = await taulaService.GetAllInteriorAsync();
            return Ok(comandes);
        }
        
        [HttpGet("GetTaulesExterior")]
        public async Task<IActionResult> GetTaulesExt()
        {
            var comandes = await taulaService.GetAllExteriorAsync();
            return Ok(comandes);
        }
        
        [HttpGet("GetTaulesPare")]
        public async Task<IActionResult> GetTaulesPare()
        {
            var comandes = await taulaService.GetTaulesPareAsync();
            return Ok(comandes);
        }
        
        [HttpPut("CanviarEstatTaula")]
        public async Task<ActionResult> CanviarEstatTaula(int id)
        {
            var ocupat = await taulaService.CanviarEstatAsync(id);
            return Ok(ocupat);
            
        }

        [HttpGet("GetTaulaByID")]
        public async Task<IActionResult> GetTaulaByID(int idTaula)
        {
            var taula = await taulaService.GetTaulaByIDAsync(idTaula);
            return Ok(taula);
        }

        [HttpGet("GetCountTaules")]
        public async Task<IActionResult> GetCountTaules(int idTaula)
        {
            var taula = await taulaService.GetCountTaulaAsync(idTaula);
            return Ok(taula);
        }
        
        [HttpGet("GetSubTaules")]
        public async Task<IActionResult> GetSubTaules(int idTaulaPare)
        {
            var subtaules = await taulaService.GetSubTaulesAsync(idTaulaPare);
            return Ok(subtaules);
        }
        
        [HttpGet("JuntarTaules")]
        public async Task<IActionResult> JuntarTaules([FromQuery] int idTaulaPare)
        {
            var ok = await taulaService.JuntarTaulesAsync(idTaulaPare);
            return Ok(ok);
        }
        
        [HttpPost("PostTaula")]
        public async Task<IActionResult> PostTaula([FromForm] TaulaNewDTO dto)
        {
            var taula = await taulaService.CrearTaulaAsync(dto);

            return Ok(taula);
        }

        [HttpDelete("DeleteTaula")]
        public async Task<IActionResult> DeleteTaulaById(int id)
        {
            var taula = await taulaService.DeleteTaulaByIDAsync(id);
            return Ok(taula);
        }
        
        [HttpPut("PutTaula")]
        public async Task<IActionResult> PutTaula([FromForm] TaulaNewDTO dto)
        {
            var taula = await taulaService.ActualitzarTaulaAsync(dto);

            return Ok(taula);
        }
        
        [HttpGet("GetAllTaules")]
        public async Task<ActionResult> GetTaulesPerFiltre()
        {
            var taules = await taulaService.GetAllAsync();
            return Ok(taules);
        } 
        
        [HttpGet("GetTaulesActives")]
        public async Task<ActionResult> GetTaulesActives()
        {
            var taules = await taulaService.GetTaulesActivesAsync();
            return Ok(taules);
        }
    }
}

