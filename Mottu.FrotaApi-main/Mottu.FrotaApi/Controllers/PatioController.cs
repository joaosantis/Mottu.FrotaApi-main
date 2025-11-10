using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mottu.FrotaApi.Data;
using Mottu.FrotaApi.Models;
using System.Threading.Tasks;
using System.Linq;

namespace Mottu.FrotaApi.Controllers
{
    [ApiController]
    [Route("api/patio")]
    public class PatioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PatioController(AppDbContext context)
        {
            _context = context;
        }

        // Este é o endpoint que o nosso dashboard vai chamar
        // GET /api/patio/visualizar
        [HttpGet("visualizar")]
        public async Task<IActionResult> VisualizarPatio()
        {
            // Pega TODAS as motos do banco de dados
            // Filtramos para pegar apenas as que foram detectadas pela visão
            var motosNoPatio = await _context.Motos
                                    .Where(m => m.VisaoId != null)
                                    .ToListAsync();
            
            Console.WriteLine($"--- DASHBOARD SOLICITOU DADOS ---");
            Console.WriteLine($"Enviando {motosNoPatio.Count} motos para o frontend.");

            // Retorna a lista de motos como JSON
            return Ok(motosNoPatio);
        }
    }
}