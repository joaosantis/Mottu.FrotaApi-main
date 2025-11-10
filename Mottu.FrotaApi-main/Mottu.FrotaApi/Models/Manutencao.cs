using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Mottu.FrotaApi.Models
{
    public class Manutencao
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Descricao { get; set; } = string.Empty;

        [Required]
        public DateTime Data { get; set; }

        [Required]
        public int MotoId { get; set; }

        [JsonIgnore] 
        public Moto? Moto { get; set; }
    }
}
