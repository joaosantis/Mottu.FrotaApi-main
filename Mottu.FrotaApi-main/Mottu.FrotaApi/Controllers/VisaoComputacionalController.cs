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
    [Route("api/visao")]
    public class VisaoComputacionalController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VisaoComputacionalController(AppDbContext context)
        {
            _context = context;
        }
        
        [HttpPost("reportar-localizacao")]
        // O DTO VisaoReportDto (que corrigimos para Newtonsoft)
        // já espera uma Lista, então [FromBody] List<> está correto.
        public async Task<IActionResult> ReportarLocalizacao([FromBody] List<VisaoReportDto> reports)
        {
            if (reports == null || !reports.Any())
            {
                return BadRequest("Nenhum dado de visão recebido.");
            }

            Console.WriteLine($"--- DADOS DA VISÃO RECEBIDOS (YOLO) ---");
            
            foreach (var report in reports)
            {
                // Validação de segurança
                if (string.IsNullOrEmpty(report.MotoId) || string.IsNullOrEmpty(report.FilialId))
                {
                    Console.WriteLine("[YOLO] Ignorando report com MotoId ou FilialId nulo.");
                    continue; 
                }

                // --- CORREÇÃO (Etapa 6.9): Extrair o FilialId ---
                // Converte "FILIAL-SP-01" para 1
                int.TryParse(report.FilialId.Replace("FILIAL-SP-", ""), out int filialId);
                if (filialId == 0) filialId = 1; // ID padrão caso a conversão falhe

                var motoExistente = await _context.Motos
                                      .FirstOrDefaultAsync(m => m.VisaoId == report.MotoId);

                if (motoExistente != null)
                {
                    // ATUALIZA MOTO
                    motoExistente.PosicaoX = report.PosicaoX;
                    motoExistente.FilialId = filialId; 
                    Console.WriteLine($"Moto {motoExistente.VisaoId} ATUALIZADA. Posição X: {report.PosicaoX}");
                }
                else
                {
                    // CRIA MOTO (com o FilialId obrigatório)
                    var novaMoto = new Moto
                    {
                        VisaoId = report.MotoId,
                        PosicaoX = report.PosicaoX,
                        FilialId = filialId, // <-- ESTA LINHA CORRIGE O BUG 500
                        Status = "Detectada" 
                    };
                    
                    await _context.Motos.AddAsync(novaMoto);
                    Console.WriteLine($"Moto {novaMoto.VisaoId} CRIADA. Posição X: {report.PosicaoX}");
                }
            }

            await _context.SaveChangesAsync();
            Console.WriteLine("--- DADOS DA VISÃO SALVOS NO BANCO DE DADOS ---");

            return Ok(new { message = $"Dados de visão processados para {reports.Count} objetos." });
        }
    }
}