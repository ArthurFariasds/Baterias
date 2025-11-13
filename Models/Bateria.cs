using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrocaBateriaWebApp.Models
{

    public class Bateria
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da bateria é obrigatório")]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo da bateria é obrigatório")]
        [StringLength(50)]
        public string Tipo { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "A quantidade é obrigatória")]
        [Range(0, 10000, ErrorMessage = "A quantidade deve ser entre 0 e 10000")]
        public int Quantidade { get; set; }

        public DateTime DataCadastro { get; set; } = DateTime.Now;

        [Required]
        public string EmpresaId { get; set; } = string.Empty;

        [ForeignKey("EmpresaId")]
        public virtual ApplicationUser? Empresa { get; set; }
    }
}
