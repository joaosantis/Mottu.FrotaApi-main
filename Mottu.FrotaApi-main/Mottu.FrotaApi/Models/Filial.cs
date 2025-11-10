using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mottu.FrotaApi.Models
{
    public class Filial
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [MaxLength(300)]
        public string Endereco { get; set; } = string.Empty;

        public ICollection<Moto> Motos { get; set; } = new List<Moto>();
    }
}
