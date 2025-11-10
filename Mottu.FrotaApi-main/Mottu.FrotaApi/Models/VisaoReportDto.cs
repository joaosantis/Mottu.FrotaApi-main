using Newtonsoft.Json; // <-- MUDANÇA AQUI

namespace Mottu.FrotaApi.Models
{
    public class VisaoReportDto
    {
        [JsonProperty("moto_id")] // <-- MUDANÇA AQUI
        public string? MotoId { get; set; }

        [JsonProperty("filial_id")] // <-- MUDANÇA AQUI
        public string? FilialId { get; set; }

        [JsonProperty("posicao_x")] // <-- MUDANÇA AQUI
        public double PosicaoX { get; set; }
        
        [JsonProperty("teste")] // <-- MUDANÇA AQUI
        public string? Teste { get; set; }
    }
}