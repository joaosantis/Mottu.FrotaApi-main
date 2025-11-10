using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mottu.FrotaApi.ML;
using Mottu.FrotaApi.Services;

namespace Mottu.FrotaApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/ml")]
    [Authorize] 
    public class MlController : ControllerBase
    {
        private readonly DisponibilidadeTrainer _trainer;

        public MlController(DisponibilidadeTrainer trainer)
        {
            _trainer = trainer;

            // Garante que exista um modelo treinado (executa no primeiro uso)
            if (!_trainer.ModelExists())
            {
                _trainer.TreinarESalvarModelo();
            }
        }

        // POST api/v1/ml/prever-disponibilidade
        [HttpPost("prever-disponibilidade")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        public IActionResult PreverDisponibilidade([FromBody] DisponibilidadeInput input)
        {
            if (input == null) return BadRequest("Entrada inválida.");

            var pred = _trainer.Prever(input);

            // Probabilidade pode vir como 0 em alguns trainers; tratar segurança
            var prob = float.IsNaN(pred.Probability) ? 0f : pred.Probability;
            if (prob < 0) prob = 0f;
            if (prob > 1) prob = 1f;

            return Ok(new
            {
                disponivel = pred.PredictedLabel,
                probabilidade = Math.Round(prob, 4)
            });
        }

        // Opcional: endpoint para re-treinar manualmente
        [HttpPost("retrain")]
        [ProducesResponseType(204)]
        public IActionResult Retrain()
        {
            _trainer.TreinarESalvarModelo(overwrite: true);
            return NoContent();
        }
    }
}
