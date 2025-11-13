using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrocaBateriaWebApp.Models
{

    public class Agendamento
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UsuarioId { get; set; } = string.Empty;

        [Required]
        public string EmpresaId { get; set; } = string.Empty;
  
        [Required(ErrorMessage = "O tipo de bateria é obrigatório")]
        [StringLength(50)]
        public string TipoBateria { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Observacoes { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pendente";

        public DateTime DataAgendamento { get; set; } = DateTime.Now;

        public DateTime DataAtualizacao { get; set; } = DateTime.Now;

        [ForeignKey("UsuarioId")]
        public virtual ApplicationUser? Usuario { get; set; }

        [ForeignKey("EmpresaId")]
        public virtual ApplicationUser? Empresa { get; set; }
    }
}
