using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Mottu.FrotaApi.Models
{
    public class Moto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // --- MUDANÇA IMPORTANTE ---
        // Tornei Placa, Modelo e Status anuláveis (string?)
        // e removi o [Required]. Isso é NECESSÁRIO para
        // que o YOLO possa criar um registro de moto novo
        // apenas com a posição, sem saber a placa ainda.

        [MaxLength(20)]
        public string? Placa { get; set; } // Removido [Required]

        [MaxLength(100)]
        public string? Modelo { get; set; } // Removido [Required]

        [MaxLength(50)]
        public string? Status { get; set; } // Removido [Required]

        [Required] // A FilialId ainda é obrigatória, o que é ótimo.
        public int FilialId { get; set; }

        [JsonIgnore] 
        public Filial? Filial { get; set; }

        public ICollection<Manutencao> Manutencoes { get; set; } = new List<Manutencao>();
        
        // --- NOVOS CAMPOS ADICIONADOS ---
        
        // Para salvar a localização do pátio vinda do YOLO
        public double? PosicaoX { get; set; }
        public double? PosicaoY { get; set; }

        // ID de rastreamento do YOLO (para encontrar e atualizar)
        [MaxLength(50)] // É bom definir um tamanho
        public string? VisaoId { get; set; } 
        
        // --- FIM DOS NOVOS CAMPOS ---
    }
}