using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TrocaBateriaWebApp.Models;

namespace TrocaBateriaWebApp.Data
{
    /// <summary>
    /// Contexto do banco de dados que herda de IdentityDbContext
    /// Gerencia todas as entidades e relacionamentos do sistema
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// DbSet de Baterias
        /// </summary>
        public DbSet<Bateria> Baterias { get; set; }

        /// <summary>
        /// DbSet de Agendamentos
        /// </summary>
        public DbSet<Agendamento> Agendamentos { get; set; }

        /// <summary>
        /// Configuração dos relacionamentos e regras do banco
        /// </summary>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configuração do relacionamento Bateria -> Empresa
            builder.Entity<Bateria>()
                .HasOne(b => b.Empresa)
                .WithMany(u => u.Baterias)
                .HasForeignKey(b => b.EmpresaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuração do relacionamento Agendamento -> Usuario
            builder.Entity<Agendamento>()
                .HasOne(a => a.Usuario)
                .WithMany(u => u.AgendamentosFeitos)
                .HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict); // Evita exclusão em cascata

            // Configuração do relacionamento Agendamento -> Empresa
            builder.Entity<Agendamento>()
                .HasOne(a => a.Empresa)
                .WithMany(u => u.AgendamentosRecebidos)
                .HasForeignKey(a => a.EmpresaId)
                .OnDelete(DeleteBehavior.Restrict); // Evita exclusão em cascata

            // Índices para melhorar performance
            builder.Entity<ApplicationUser>()
                .HasIndex(u => u.TipoConta);

            builder.Entity<ApplicationUser>()
                .HasIndex(u => u.Cnpj)
                .IsUnique()
                .HasFilter("[Cnpj] IS NOT NULL");

            builder.Entity<Bateria>()
                .HasIndex(b => b.Tipo);

            builder.Entity<Agendamento>()
                .HasIndex(a => a.Status);
        }
    }
}
