using Microsoft.AspNetCore.Identity;

namespace TrocaBateriaWebApp.Models
{

    public class ApplicationUser : IdentityUser
    {

        public string TipoConta { get; set; } = string.Empty;

        public string? Cnpj { get; set; }

        public string NomeCompleto { get; set; } = string.Empty;

        public string? Endereco { get; set; }

        public string? Telefone { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public virtual ICollection<Bateria>? Baterias { get; set; }

        public virtual ICollection<Agendamento>? AgendamentosFeitos { get; set; }

        public virtual ICollection<Agendamento>? AgendamentosRecebidos { get; set; }
    }
}
