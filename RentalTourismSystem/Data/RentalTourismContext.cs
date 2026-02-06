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

        // ========== MANUTENÇÃO VEICULAR ==========
        public DbSet<TipoManutencao> TiposManutencao { get; set; }
        public DbSet<StatusManutencao> StatusManutencoes { get; set; }
        public DbSet<ManutencaoVeiculo> ManutencoesVeiculos { get; set; }
        public DbSet<ItemManutencao> ItensManutencao { get; set; }

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

            // ========== CONFIGURAÇÕES DE MANUTENÇÃO ==========
            modelBuilder.Entity<ManutencaoVeiculo>()
                .HasOne(m => m.Veiculo)
                .WithMany()
                .HasForeignKey(m => m.VeiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ManutencaoVeiculo>()
                .HasOne(m => m.TipoManutencao)
                .WithMany(t => t.Manutencoes)
                .HasForeignKey(m => m.TipoManutencaoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ManutencaoVeiculo>()
                .HasOne(m => m.StatusManutencao)
                .WithMany(s => s.Manutencoes)
                .HasForeignKey(m => m.StatusManutencaoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ManutencaoVeiculo>()
                .HasOne(m => m.Funcionario)
                .WithMany()
                .HasForeignKey(m => m.FuncionarioId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ItemManutencao>()
                .HasOne(i => i.ManutencaoVeiculo)
                .WithMany(m => m.Itens)
                .HasForeignKey(i => i.ManutencaoVeiculoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices para Manutenção
            modelBuilder.Entity<ManutencaoVeiculo>()
                .HasIndex(m => m.DataAgendada);

            modelBuilder.Entity<ManutencaoVeiculo>()
                .HasIndex(m => m.DataConclusao);

            modelBuilder.Entity<ManutencaoVeiculo>()
                .HasIndex(m => m.StatusManutencaoId);

            modelBuilder.Entity<ManutencaoVeiculo>()
                .HasIndex(m => m.VeiculoId);

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

            modelBuilder.Entity<ManutencaoVeiculo>()
                .Property(m => m.Custo)
                .HasColumnType("decimal(10,2)")
                .HasPrecision(10, 2);

            modelBuilder.Entity<ManutencaoVeiculo>()
                .Property(m => m.CustoPecas)
                .HasColumnType("decimal(10,2)")
                .HasPrecision(10, 2);

            modelBuilder.Entity<ManutencaoVeiculo>()
                .Property(m => m.CustoMaoObra)
                .HasColumnType("decimal(10,2)")
                .HasPrecision(10, 2);

            modelBuilder.Entity<ItemManutencao>()
                .Property(i => i.ValorUnitario)
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
                new StatusCarro { Id = 4, Status = "Indisponível" },
                new StatusCarro { Id = 5, Status = "Reservado" }
            );

            // Status de Reservas de Viagem
            modelBuilder.Entity<StatusReservaViagem>().HasData(
                new StatusReservaViagem { Id = 1, Status = "Pendente" },
                new StatusReservaViagem { Id = 2, Status = "Confirmada" },
                new StatusReservaViagem { Id = 3, Status = "Cancelada" },
                new StatusReservaViagem { Id = 4, Status = "Realizada" }
            );

            // ========== SEED DATA - MANUTENÇÃO ==========
            modelBuilder.Entity<StatusManutencao>().HasData(
                new StatusManutencao { Id = 1, Status = "Agendada", Descricao = "Manutenção agendada, aguardando início" },
                new StatusManutencao { Id = 2, Status = "Em Andamento", Descricao = "Manutenção em execução" },
                new StatusManutencao { Id = 3, Status = "Concluída", Descricao = "Manutenção concluída com sucesso" },
                new StatusManutencao { Id = 4, Status = "Cancelada", Descricao = "Manutenção cancelada" },
                new StatusManutencao { Id = 5, Status = "Pendente Aprovação", Descricao = "Aguardando aprovação de orçamento" }
            );

            modelBuilder.Entity<TipoManutencao>().HasData(
                new TipoManutencao { Id = 1, Nome = "Troca de Óleo", Descricao = "Troca de óleo do motor e filtro", Ativo = true },
                new TipoManutencao { Id = 2, Nome = "Revisão Periódica", Descricao = "Revisão programada conforme manual", Ativo = true },
                new TipoManutencao { Id = 3, Nome = "Troca de Pneus", Descricao = "Substituição de pneus", Ativo = true },
                new TipoManutencao { Id = 4, Nome = "Alinhamento e Balanceamento", Descricao = "Serviço de alinhamento e balanceamento", Ativo = true },
                new TipoManutencao { Id = 5, Nome = "Freios", Descricao = "Manutenção do sistema de freios", Ativo = true },
                new TipoManutencao { Id = 6, Nome = "Suspensão", Descricao = "Manutenção do sistema de suspensão", Ativo = true },
                new TipoManutencao { Id = 7, Nome = "Ar Condicionado", Descricao = "Manutenção do sistema de climatização", Ativo = true },
                new TipoManutencao { Id = 8, Nome = "Bateria", Descricao = "Troca ou manutenção da bateria", Ativo = true },
                new TipoManutencao { Id = 9, Nome = "Sistema Elétrico", Descricao = "Reparos no sistema elétrico", Ativo = true },
                new TipoManutencao { Id = 10, Nome = "Motor", Descricao = "Reparos no motor", Ativo = true },
                new TipoManutencao { Id = 11, Nome = "Câmbio", Descricao = "Manutenção do sistema de transmissão", Ativo = true },
                new TipoManutencao { Id = 12, Nome = "Funilaria", Descricao = "Reparos de lataria e funilaria", Ativo = true },
                new TipoManutencao { Id = 13, Nome = "Pintura", Descricao = "Serviços de pintura", Ativo = true },
                new TipoManutencao { Id = 14, Nome = "Vidros", Descricao = "Troca ou reparo de vidros", Ativo = true },
                new TipoManutencao { Id = 15, Nome = "Limpeza Completa", Descricao = "Limpeza interna e externa detalhada", Ativo = true },
                new TipoManutencao { Id = 16, Nome = "Inspeção Veicular", Descricao = "Inspeção veicular obrigatória", Ativo = true },
                new TipoManutencao { Id = 17, Nome = "Outros", Descricao = "Outras manutenções não especificadas", Ativo = true }
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