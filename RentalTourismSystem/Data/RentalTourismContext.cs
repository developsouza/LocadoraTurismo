using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RentalTourismSystem.Models;

namespace RentalTourismSystem.Data
{
    public class RentalTourismContext : IdentityDbContext<ApplicationUser>
    {
        public RentalTourismContext(DbContextOptions<RentalTourismContext> options) : base(options) { }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<StatusCarro> StatusCarros { get; set; }
        public DbSet<Agencia> Agencias { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<Locacao> Locacoes { get; set; }
        public DbSet<StatusReservaViagem> StatusReservaViagens { get; set; }
        public DbSet<PacoteViagem> PacotesViagens { get; set; }
        public DbSet<ReservaViagem> ReservasViagens { get; set; }
        public DbSet<ServicoAdicional> ServicosAdicionais { get; set; }
        public DbSet<Notificacao> Notificacoes { get; set; }
        public DbSet<Documento> Documentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========== CONFIGURAÇÕES DE RELACIONAMENTO ==========
            modelBuilder.Entity<Funcionario>()
                .HasOne(f => f.Agencia)
                .WithMany(a => a.Funcionarios)
                .HasForeignKey(f => f.AgenciaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Veiculo>()
                .HasOne(v => v.StatusCarro)
                .WithMany(s => s.Veiculos)
                .HasForeignKey(v => v.StatusCarroId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Veiculo>()
                .HasOne(v => v.Agencia)
                .WithMany(a => a.Veiculos)
                .HasForeignKey(v => v.AgenciaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Locacao>()
                .HasOne(l => l.Cliente)
                .WithMany(c => c.Locacoes)
                .HasForeignKey(l => l.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Locacao>()
                .HasOne(l => l.Veiculo)
                .WithMany(v => v.Locacoes)
                .HasForeignKey(l => l.VeiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Locacao>()
                .HasOne(l => l.Funcionario)
                .WithMany(f => f.Locacoes)
                .HasForeignKey(l => l.FuncionarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Locacao>()
                .HasOne(l => l.Agencia)
                .WithMany(a => a.Locacoes)
                .HasForeignKey(l => l.AgenciaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReservaViagem>()
                .HasOne(r => r.Cliente)
                .WithMany(c => c.ReservasViagens)
                .HasForeignKey(r => r.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReservaViagem>()
                .HasOne(r => r.PacoteViagem)
                .WithMany(p => p.ReservasViagens)
                .HasForeignKey(r => r.PacoteViagemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReservaViagem>()
                .HasOne(r => r.StatusReservaViagem)
                .WithMany(s => s.ReservasViagens)
                .HasForeignKey(r => r.StatusReservaViagemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ServicoAdicional>()
                .HasOne(s => s.ReservaViagem)
                .WithMany(r => r.ServicosAdicionais)
                .HasForeignKey(s => s.ReservaViagemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Documento>(entity =>
            {
                entity.HasIndex(e => e.DataUpload);
                entity.HasIndex(e => e.TipoDocumento);
                entity.HasIndex(e => e.ClienteId);
                entity.HasIndex(e => e.VeiculoId);
                entity.HasIndex(e => e.FuncionarioId);

                entity.HasOne(e => e.Cliente)
                    .WithMany(c => c.Documentos)
                    .HasForeignKey(e => e.ClienteId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Veiculo)
                    .WithMany(v => v.Documentos)
                    .HasForeignKey(e => e.VeiculoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Funcionario)
                    .WithMany()
                    .HasForeignKey(e => e.FuncionarioId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ApplicationUser)
                    .WithMany()
                    .HasForeignKey(e => e.ApplicationUserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ========== CONFIGURAÇÕES DE ÍNDICES ÚNICOS ==========

            // Cliente - CPF (atualizado para maiúsculo)
            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.CPF)
                .IsUnique();

            // Cliente - Email
            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.Email)
                .IsUnique()
                .HasFilter("[Email] IS NOT NULL");

            // Funcionario - Cpf (mantém como está no seu modelo Funcionario)
            modelBuilder.Entity<Funcionario>()
                .HasIndex(f => f.Cpf)
                .IsUnique();

            // Funcionario - Email
            modelBuilder.Entity<Funcionario>()
                .HasIndex(f => f.Email)
                .IsUnique()
                .HasFilter("[Email] IS NOT NULL");

            // Veiculo - Placa
            modelBuilder.Entity<Veiculo>()
                .HasIndex(v => v.Placa)
                .IsUnique();

            // ========== CONFIGURAÇÕES PARA APPLICATION USER ==========
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasOne(u => u.Agencia)
                      .WithMany()
                      .HasForeignKey(u => u.AgenciaId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(u => u.Funcionario)
                      .WithOne()
                      .HasForeignKey<ApplicationUser>(u => u.FuncionarioId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(u => u.Cpf)
                      .IsUnique()
                      .HasFilter("[Cpf] IS NOT NULL");
            });

            // ========== CONFIGURAÇÕES DE PRECISÃO DECIMAL ==========
            modelBuilder.Entity<Locacao>()
                .Property(l => l.ValorTotal)
                .HasColumnType("decimal(10,2)")
                .HasPrecision(10, 2);

            modelBuilder.Entity<ReservaViagem>()
                .Property(r => r.ValorTotal)
                .HasColumnType("decimal(10,2)")
                .HasPrecision(10, 2);

            modelBuilder.Entity<PacoteViagem>()
                .Property(p => p.Preco)
                .HasColumnType("decimal(10,2)")
                .HasPrecision(10, 2);

            modelBuilder.Entity<ServicoAdicional>()
                .Property(s => s.Preco)
                .HasColumnType("decimal(10,2)")
                .HasPrecision(10, 2);

            modelBuilder.Entity<Veiculo>()
                .Property(v => v.ValorDiaria)
                .HasColumnType("decimal(10,2)")
                .HasPrecision(10, 2);

            modelBuilder.Entity<Veiculo>()
                .Property(v => v.ValorMercado)
                .HasColumnType("decimal(10,2)")
                .HasPrecision(10, 2);

            modelBuilder.Entity<Funcionario>()
                .Property(f => f.Salario)
                .HasColumnType("decimal(10,2)")
                .HasPrecision(10, 2);

            // ========== CONFIGURAÇÃO DE NOTIFICAÇÕES ==========
            modelBuilder.Entity<Notificacao>(entity =>
            {
                entity.HasIndex(e => e.DataCriacao);
                entity.HasIndex(e => e.Lida);
                entity.HasIndex(e => e.Tipo);
                entity.HasIndex(e => e.Categoria);

                entity.HasOne(e => e.Cliente)
                    .WithMany()
                    .HasForeignKey(e => e.ClienteId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Veiculo)
                    .WithMany()
                    .HasForeignKey(e => e.VeiculoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Locacao)
                    .WithMany()
                    .HasForeignKey(e => e.LocacaoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Reserva)
                    .WithMany()
                    .HasForeignKey(e => e.ReservaId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ========== DADOS INICIAIS (SEED DATA) ==========

            // Status de Carros
            modelBuilder.Entity<StatusCarro>().HasData(
                new StatusCarro { Id = 1, Status = "Disponível" },
                new StatusCarro { Id = 2, Status = "Alugado" },
                new StatusCarro { Id = 3, Status = "Manutenção" },
                new StatusCarro { Id = 4, Status = "Indisponível" }
            );

            // Status de Reservas de Viagem
            modelBuilder.Entity<StatusReservaViagem>().HasData(
                new StatusReservaViagem { Id = 1, Status = "Pendente" },
                new StatusReservaViagem { Id = 2, Status = "Confirmada" },
                new StatusReservaViagem { Id = 3, Status = "Cancelada" },
                new StatusReservaViagem { Id = 4, Status = "Realizada" }
            );

            // Agência Central (atualizada com novos campos)
            modelBuilder.Entity<Agencia>().HasData(
                new Agencia
                {
                    Id = 1,
                    Nome = "Agência Central",
                    CNPJ = "12.345.678/0001-90",
                    Endereco = "Rua Principal, 123 - Centro",
                    Cidade = "João Pessoa - PB",
                    Telefone = "(83) 3221-1234",
                    Email = "central@rentaltourism.com.br"
                }
            );

            // Pacotes de Viagem (exemplos)
            modelBuilder.Entity<PacoteViagem>().HasData(
                new PacoteViagem
                {
                    Id = 1,
                    Nome = "Pacote Praia do Forte",
                    Descricao = "3 dias e 2 noites na Praia do Forte com café da manhã incluso",
                    Destino = "Praia do Forte - BA",
                    Duracao = 3,
                    UnidadeTempo = "dias",
                    Preco = 850.00m,
                    Ativo = true,
                    DataCriacao = new DateTime(2024, 1, 15, 10, 0, 0)
                },
                new PacoteViagem
                {
                    Id = 2,
                    Nome = "Pacote Serra da Mantiqueira",
                    Descricao = "5 dias na Serra da Mantiqueira com todas as refeições",
                    Destino = "Serra da Mantiqueira - MG",
                    Duracao = 5,
                    UnidadeTempo = "dias",
                    Preco = 1200.00m,
                    Ativo = true,
                    DataCriacao = new DateTime(2024, 1, 20, 14, 30, 0)
                },
                new PacoteViagem
                {
                    Id = 3,
                    Nome = "Passeio de Buggy nas Dunas",
                    Descricao = "Emocionante passeio de buggy pelas dunas de Natal com parada para banho de lagoa",
                    Destino = "Genipabu - RN",
                    Duracao = 4,
                    UnidadeTempo = "horas",
                    Preco = 120.00m,
                    Ativo = true,
                    DataCriacao = new DateTime(2024, 2, 1, 9, 15, 0)
                },
                new PacoteViagem
                {
                    Id = 4,
                    Nome = "Mergulho em Recife de Coral",
                    Descricao = "Mergulho com cilindro nos recifes de coral de Porto de Galinhas, equipamentos inclusos",
                    Destino = "Porto de Galinhas - PE",
                    Duracao = 3,
                    UnidadeTempo = "horas",
                    Preco = 180.00m,
                    Ativo = true,
                    DataCriacao = new DateTime(2024, 2, 10, 11, 45, 0)
                }
            );
        }
    }
}