using Newtonsoft.Json; // <-- MUDANÇA AQUI (de System.Text.Json para Newtonsoft)

namespace Mottu.FrotaApi.Models
{
    public class IotReportDto
    {
        // --- CORREÇÃO: Usando o atributo [JsonProperty] do Newtonsoft ---
        [JsonProperty("moto_id")] // <-- MUDANÇA AQUI (removido "Name")
        public string? MotoId { get; set; } 

        [JsonProperty("status")] // <-- MUDANÇA AQUI
        public string? Status { get; set; }
    }
}