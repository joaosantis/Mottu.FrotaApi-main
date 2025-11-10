using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mottu.FrotaApi.Data;
using Mottu.FrotaApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace Mottu.FrotaApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class ManutencoesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ManutencoesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> GetManutencoes([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var totalItems = await _context.Manutencoes.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var manutencoes = await _context.Manutencoes
                .Include(m => m.Moto)
                .ThenInclude(m => m.Filial)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new
            {
                totalItems,
                page,
                pageSize,
                totalPages,
                data = manutencoes.Select(m => new
                {
                    m.Id,
                    m.Data,
                    m.Descricao,
                    Moto = new { m.Moto.Id, m.Moto.Placa, Filial = new { m.Moto.Filial.Id, m.Moto.Filial.Nome } }
                })
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetManutencao(int id)
        {
            var manutencao = await _context.Manutencoes
                .Include(m => m.Moto)
                .ThenInclude(m => m.Filial)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (manutencao == null) return NotFound();

            return Ok(manutencao);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Manutencao), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateManutencao([FromBody] Manutencao manutencao)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Manutencoes.Add(manutencao);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetManutencao), new { id = manutencao.Id }, manutencao);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateManutencao(int id, [FromBody] Manutencao manutencao)
        {
            if (id != manutencao.Id) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Entry(manutencao).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Manutencoes.Any(m => m.Id == id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteManutencao(int id)
        {
            var manutencao = await _context.Manutencoes.FindAsync(id);
            if (manutencao == null) return NotFound();

            _context.Manutencoes.Remove(manutencao);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
