using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mottu.FrotaApi.Data; 
using Mottu.FrotaApi.Models; 
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Mottu.FrotaApi.Controllers
{
    [ApiController]
    [Route("api/iot")]
    public class IotController : ControllerBase
    {
        private readonly AppDbContext _context;

        public IotController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("reportar-status")]
        public async Task<IActionResult> ReportarStatus([FromBody] List<IotReportDto> reports) 
        {
            if (reports == null || !reports.Any())
            {
                return BadRequest("Dados de IoT inválidos (lista vazia).");
            }

            var report = reports[0]; 
            
            // --- CORREÇÃO: Usando a propriedade 'MotoId' (Maiúsculo) ---
            if (string.IsNullOrEmpty(report.MotoId)) 
            {
                return BadRequest("MotoId inválido (o mapeamento falhou)."); 
            }

            // --- CORREÇÃO: Usando a propriedade 'MotoId' (Maiúsculo) ---
            var moto = await _context.Motos
                         .FirstOrDefaultAsync(m => m.VisaoId == report.MotoId); 

            if (moto == null)
            {
                // Este erro 404 agora só vai acontecer se o YOLO
                // realmente ainda não tiver criado a moto.
                Console.WriteLine($"[IoT] Moto com VisaoId {report.MotoId} não encontrada.");
                return NotFound($"Moto com VisaoId {report.MotoId} não encontrada.");
            }

            moto.Status = report.Status; // <-- CORREÇÃO
            await _context.SaveChangesAsync();

            Console.WriteLine($"--- DADOS DE IoT RECEBIDOS ---");
            Console.WriteLine($"Status da Moto {moto.VisaoId} atualizado para: {moto.Status}");
            Console.WriteLine($"--- DADOS SALVOS NO BANCO DE DADOS ---");

            return Ok(new { message = $"Status de {moto.VisaoId} atualizado." });
        }
    }
}