using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RentalTourismSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddCNHPatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Agencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CNPJ = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Endereco = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Cidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Telefone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agencias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CPF = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Endereco = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CEP = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DataNascimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstadoCivil = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Profissao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CNH = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ValidadeCNH = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CategoriaCNH = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    CNHPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PacotesViagens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Destino = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Duracao = table.Column<int>(type: "int", nullable: false),
                    UnidadeTempo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Preco = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PacotesViagens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatusCarros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusCarros", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatusManutencoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusManutencoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatusReservaViagens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusReservaViagens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposManutencao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposManutencao", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Funcionarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Cpf = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Cargo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Salario = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    DataAdmissao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    AgenciaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funcionarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Funcionarios_Agencias_AgenciaId",
                        column: x => x.AgenciaId,
                        principalTable: "Agencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Veiculos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Marca = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Modelo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ano = table.Column<int>(type: "int", nullable: false),
                    Placa = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Cor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Combustivel = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Cambio = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ValorDiaria = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    ValorMercado = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    Quilometragem = table.Column<int>(type: "int", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatusCarroId = table.Column<int>(type: "int", nullable: false),
                    AgenciaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veiculos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Veiculos_Agencias_AgenciaId",
                        column: x => x.AgenciaId,
                        principalTable: "Agencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Veiculos_StatusCarros_StatusCarroId",
                        column: x => x.StatusCarroId,
                        principalTable: "StatusCarros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReservasViagens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataReserva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataViagem = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Observacoes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    PacoteViagemId = table.Column<int>(type: "int", nullable: false),
                    StatusReservaViagemId = table.Column<int>(type: "int", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UltimaAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservasViagens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservasViagens_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservasViagens_PacotesViagens_PacoteViagemId",
                        column: x => x.PacoteViagemId,
                        principalTable: "PacotesViagens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservasViagens_StatusReservaViagens_StatusReservaViagemId",
                        column: x => x.StatusReservaViagemId,
                        principalTable: "StatusReservaViagens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NomeCompleto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Cpf = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    Cargo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AgenciaId = table.Column<int>(type: "int", nullable: true),
                    FuncionarioId = table.Column<int>(type: "int", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Agencias_AgenciaId",
                        column: x => x.AgenciaId,
                        principalTable: "Agencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Funcionarios_FuncionarioId",
                        column: x => x.FuncionarioId,
                        principalTable: "Funcionarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Locacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataRetirada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataDevolucao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataDevolucaoReal = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValorTotal = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Observacoes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    QuilometragemDevolucao = table.Column<int>(type: "int", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    VeiculoId = table.Column<int>(type: "int", nullable: false),
                    FuncionarioId = table.Column<int>(type: "int", nullable: false),
                    AgenciaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locacoes_Agencias_AgenciaId",
                        column: x => x.AgenciaId,
                        principalTable: "Agencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Locacoes_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Locacoes_Funcionarios_FuncionarioId",
                        column: x => x.FuncionarioId,
                        principalTable: "Funcionarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Locacoes_Veiculos_VeiculoId",
                        column: x => x.VeiculoId,
                        principalTable: "Veiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ManutencoesVeiculos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VeiculoId = table.Column<int>(type: "int", nullable: false),
                    TipoManutencaoId = table.Column<int>(type: "int", nullable: false),
                    StatusManutencaoId = table.Column<int>(type: "int", nullable: false),
                    DataAgendada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataConclusao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QuilometragemAtual = table.Column<int>(type: "int", nullable: false),
                    ProximaQuilometragem = table.Column<int>(type: "int", nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Observacoes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Oficina = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Custo = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    CustoPecas = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    CustoMaoObra = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    GarantiaDias = table.Column<int>(type: "int", nullable: true),
                    Preventiva = table.Column<bool>(type: "bit", nullable: false),
                    Urgente = table.Column<bool>(type: "bit", nullable: false),
                    FuncionarioId = table.Column<int>(type: "int", nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NotaFiscal = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManutencoesVeiculos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManutencoesVeiculos_Funcionarios_FuncionarioId",
                        column: x => x.FuncionarioId,
                        principalTable: "Funcionarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ManutencoesVeiculos_StatusManutencoes_StatusManutencaoId",
                        column: x => x.StatusManutencaoId,
                        principalTable: "StatusManutencoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ManutencoesVeiculos_TiposManutencao_TipoManutencaoId",
                        column: x => x.TipoManutencaoId,
                        principalTable: "TiposManutencao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ManutencoesVeiculos_Veiculos_VeiculoId",
                        column: x => x.VeiculoId,
                        principalTable: "Veiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServicosAdicionais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Preco = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    ReservaViagemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicosAdicionais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServicosAdicionais_ReservasViagens_ReservaViagemId",
                        column: x => x.ReservaViagemId,
                        principalTable: "ReservasViagens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Documentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeArquivo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CaminhoArquivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TipoDocumento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TamanhoBytes = table.Column<long>(type: "bigint", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DataUpload = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioUpload = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ClienteId = table.Column<int>(type: "int", nullable: true),
                    VeiculoId = table.Column<int>(type: "int", nullable: true),
                    FuncionarioId = table.Column<int>(type: "int", nullable: true),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documentos_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Documentos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Documentos_Funcionarios_FuncionarioId",
                        column: x => x.FuncionarioId,
                        principalTable: "Funcionarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Documentos_Veiculos_VeiculoId",
                        column: x => x.VeiculoId,
                        principalTable: "Veiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notificacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Mensagem = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LinkAcao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TextoLinkAcao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClienteId = table.Column<int>(type: "int", nullable: true),
                    VeiculoId = table.Column<int>(type: "int", nullable: true),
                    LocacaoId = table.Column<int>(type: "int", nullable: true),
                    ReservaId = table.Column<int>(type: "int", nullable: true),
                    Lida = table.Column<bool>(type: "bit", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataLeitura = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Prioridade = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notificacoes_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notificacoes_Locacoes_LocacaoId",
                        column: x => x.LocacaoId,
                        principalTable: "Locacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notificacoes_ReservasViagens_ReservaId",
                        column: x => x.ReservaId,
                        principalTable: "ReservasViagens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notificacoes_Veiculos_VeiculoId",
                        column: x => x.VeiculoId,
                        principalTable: "Veiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItensManutencao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ManutencaoVeiculoId = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Fornecedor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CodigoPeca = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Observacoes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensManutencao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensManutencao_ManutencoesVeiculos_ManutencaoVeiculoId",
                        column: x => x.ManutencaoVeiculoId,
                        principalTable: "ManutencoesVeiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Agencias",
                columns: new[] { "Id", "CNPJ", "Cidade", "Email", "Endereco", "Nome", "Telefone" },
                values: new object[] { 1, "12.345.678/0001-90", "João Pessoa - PB", "central@rentaltourism.com.br", "Rua Principal, 123 - Centro", "Agência Central", "(83) 3221-1234" });

            migrationBuilder.InsertData(
                table: "PacotesViagens",
                columns: new[] { "Id", "Ativo", "DataCriacao", "Descricao", "Destino", "Duracao", "Nome", "Preco", "UnidadeTempo" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2024, 1, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), "3 dias e 2 noites na Praia do Forte com café da manhã incluso", "Praia do Forte - BA", 3, "Pacote Praia do Forte", 850.00m, "dias" },
                    { 2, true, new DateTime(2024, 1, 20, 14, 30, 0, 0, DateTimeKind.Unspecified), "5 dias na Serra da Mantiqueira com todas as refeições", "Serra da Mantiqueira - MG", 5, "Pacote Serra da Mantiqueira", 1200.00m, "dias" },
                    { 3, true, new DateTime(2024, 2, 1, 9, 15, 0, 0, DateTimeKind.Unspecified), "Emocionante passeio de buggy pelas dunas de Natal com parada para banho de lagoa", "Genipabu - RN", 4, "Passeio de Buggy nas Dunas", 120.00m, "horas" },
                    { 4, true, new DateTime(2024, 2, 10, 11, 45, 0, 0, DateTimeKind.Unspecified), "Mergulho com cilindro nos recifes de coral de Porto de Galinhas, equipamentos inclusos", "Porto de Galinhas - PE", 3, "Mergulho em Recife de Coral", 180.00m, "horas" }
                });

            migrationBuilder.InsertData(
                table: "StatusCarros",
                columns: new[] { "Id", "Status" },
                values: new object[,]
                {
                    { 1, "Disponível" },
                    { 2, "Alugado" },
                    { 3, "Manutenção" },
                    { 4, "Indisponível" }
                });

            migrationBuilder.InsertData(
                table: "StatusManutencoes",
                columns: new[] { "Id", "Descricao", "Status" },
                values: new object[,]
                {
                    { 1, "Manutenção agendada, aguardando início", "Agendada" },
                    { 2, "Manutenção em execução", "Em Andamento" },
                    { 3, "Manutenção concluída com sucesso", "Concluída" },
                    { 4, "Manutenção cancelada", "Cancelada" },
                    { 5, "Aguardando aprovação de orçamento", "Pendente Aprovação" }
                });

            migrationBuilder.InsertData(
                table: "StatusReservaViagens",
                columns: new[] { "Id", "Status" },
                values: new object[,]
                {
                    { 1, "Pendente" },
                    { 2, "Confirmada" },
                    { 3, "Cancelada" },
                    { 4, "Realizada" }
                });

            migrationBuilder.InsertData(
                table: "TiposManutencao",
                columns: new[] { "Id", "Ativo", "Descricao", "Nome" },
                values: new object[,]
                {
                    { 1, true, "Troca de óleo do motor e filtro", "Troca de Óleo" },
                    { 2, true, "Revisão programada conforme manual", "Revisão Periódica" },
                    { 3, true, "Substituição de pneus", "Troca de Pneus" },
                    { 4, true, "Serviço de alinhamento e balanceamento", "Alinhamento e Balanceamento" },
                    { 5, true, "Manutenção do sistema de freios", "Freios" },
                    { 6, true, "Manutenção do sistema de suspensão", "Suspensão" },
                    { 7, true, "Manutenção do sistema de climatização", "Ar Condicionado" },
                    { 8, true, "Troca ou manutenção da bateria", "Bateria" },
                    { 9, true, "Reparos no sistema elétrico", "Sistema Elétrico" },
                    { 10, true, "Reparos no motor", "Motor" },
                    { 11, true, "Manutenção do sistema de transmissão", "Câmbio" },
                    { 12, true, "Reparos de lataria e funilaria", "Funilaria" },
                    { 13, true, "Serviços de pintura", "Pintura" },
                    { 14, true, "Troca ou reparo de vidros", "Vidros" },
                    { 15, true, "Limpeza interna e externa detalhada", "Limpeza Completa" },
                    { 16, true, "Inspeção veicular obrigatória", "Inspeção Veicular" },
                    { 17, true, "Outras manutenções não especificadas", "Outros" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_AgenciaId",
                table: "AspNetUsers",
                column: "AgenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Cpf",
                table: "AspNetUsers",
                column: "Cpf",
                unique: true,
                filter: "[Cpf] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_FuncionarioId",
                table: "AspNetUsers",
                column: "FuncionarioId",
                unique: true,
                filter: "[FuncionarioId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_CPF",
                table: "Clientes",
                column: "CPF",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Email",
                table: "Clientes",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_ApplicationUserId",
                table: "Documentos",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_ClienteId",
                table: "Documentos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_DataUpload",
                table: "Documentos",
                column: "DataUpload");

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_FuncionarioId",
                table: "Documentos",
                column: "FuncionarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_TipoDocumento",
                table: "Documentos",
                column: "TipoDocumento");

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_VeiculoId",
                table: "Documentos",
                column: "VeiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_Funcionarios_AgenciaId",
                table: "Funcionarios",
                column: "AgenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Funcionarios_Cpf",
                table: "Funcionarios",
                column: "Cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Funcionarios_Email",
                table: "Funcionarios",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ItensManutencao_ManutencaoVeiculoId",
                table: "ItensManutencao",
                column: "ManutencaoVeiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_Locacoes_AgenciaId",
                table: "Locacoes",
                column: "AgenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Locacoes_ClienteId",
                table: "Locacoes",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Locacoes_FuncionarioId",
                table: "Locacoes",
                column: "FuncionarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Locacoes_VeiculoId",
                table: "Locacoes",
                column: "VeiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_ManutencoesVeiculos_DataAgendada",
                table: "ManutencoesVeiculos",
                column: "DataAgendada");

            migrationBuilder.CreateIndex(
                name: "IX_ManutencoesVeiculos_DataConclusao",
                table: "ManutencoesVeiculos",
                column: "DataConclusao");

            migrationBuilder.CreateIndex(
                name: "IX_ManutencoesVeiculos_FuncionarioId",
                table: "ManutencoesVeiculos",
                column: "FuncionarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ManutencoesVeiculos_StatusManutencaoId",
                table: "ManutencoesVeiculos",
                column: "StatusManutencaoId");

            migrationBuilder.CreateIndex(
                name: "IX_ManutencoesVeiculos_TipoManutencaoId",
                table: "ManutencoesVeiculos",
                column: "TipoManutencaoId");

            migrationBuilder.CreateIndex(
                name: "IX_ManutencoesVeiculos_VeiculoId",
                table: "ManutencoesVeiculos",
                column: "VeiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificacoes_Categoria",
                table: "Notificacoes",
                column: "Categoria");

            migrationBuilder.CreateIndex(
                name: "IX_Notificacoes_ClienteId",
                table: "Notificacoes",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificacoes_DataCriacao",
                table: "Notificacoes",
                column: "DataCriacao");

            migrationBuilder.CreateIndex(
                name: "IX_Notificacoes_Lida",
                table: "Notificacoes",
                column: "Lida");

            migrationBuilder.CreateIndex(
                name: "IX_Notificacoes_LocacaoId",
                table: "Notificacoes",
                column: "LocacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificacoes_ReservaId",
                table: "Notificacoes",
                column: "ReservaId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificacoes_Tipo",
                table: "Notificacoes",
                column: "Tipo");

            migrationBuilder.CreateIndex(
                name: "IX_Notificacoes_VeiculoId",
                table: "Notificacoes",
                column: "VeiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservasViagens_ClienteId",
                table: "ReservasViagens",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservasViagens_PacoteViagemId",
                table: "ReservasViagens",
                column: "PacoteViagemId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservasViagens_StatusReservaViagemId",
                table: "ReservasViagens",
                column: "StatusReservaViagemId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicosAdicionais_ReservaViagemId",
                table: "ServicosAdicionais",
                column: "ReservaViagemId");

            migrationBuilder.CreateIndex(
                name: "IX_Veiculos_AgenciaId",
                table: "Veiculos",
                column: "AgenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Veiculos_Placa",
                table: "Veiculos",
                column: "Placa",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Veiculos_StatusCarroId",
                table: "Veiculos",
                column: "StatusCarroId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Documentos");

            migrationBuilder.DropTable(
                name: "ItensManutencao");

            migrationBuilder.DropTable(
                name: "Notificacoes");

            migrationBuilder.DropTable(
                name: "ServicosAdicionais");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ManutencoesVeiculos");

            migrationBuilder.DropTable(
                name: "Locacoes");

            migrationBuilder.DropTable(
                name: "ReservasViagens");

            migrationBuilder.DropTable(
                name: "StatusManutencoes");

            migrationBuilder.DropTable(
                name: "TiposManutencao");

            migrationBuilder.DropTable(
                name: "Funcionarios");

            migrationBuilder.DropTable(
                name: "Veiculos");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "PacotesViagens");

            migrationBuilder.DropTable(
                name: "StatusReservaViagens");

            migrationBuilder.DropTable(
                name: "Agencias");

            migrationBuilder.DropTable(
                name: "StatusCarros");
        }
    }
}
