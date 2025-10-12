# ?? Referência Técnica do Sistema - Documentação para Desenvolvedores

## ?? Índice
- [Visão Geral Técnica](#visão-geral-técnica)
- [Arquitetura](#arquitetura)
- [Tecnologias Utilizadas](#tecnologias-utilizadas)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Modelos de Dados](#modelos-de-dados)
- [APIs e Endpoints](#apis-e-endpoints)
- [Segurança](#segurança)
- [Configurações](#configurações)

---

## ?? Visão Geral Técnica

### ?? Sobre o Sistema

**Nome:** Sistema Integrado de Locação e Turismo  
**Cliente:** Litoral Sul Locadora e Turismo  
**Arquitetura:** ASP.NET Core 8.0 Razor Pages com MVC  
**Padrão:** Model-View-Controller + MVVM  

### ??? Stack Tecnológica

```
Frontend:
??? Razor Pages (Views)
??? Bootstrap 5.3
??? jQuery 3.x
??? JavaScript ES6+
??? CSS3 + SCSS

Backend:
??? ASP.NET Core 8.0
??? Entity Framework Core 8.0
??? ASP.NET Core Identity
??? C# 12

Database:
??? SQL Server 2019+
??? T-SQL

Logging:
??? Serilog
??? File Logger
??? Console Logger

Versionamento:
??? API Versioning 7.x
```

---

## ??? Arquitetura

### ?? Diagrama de Camadas

```
???????????????????????????????????????
?         PRESENTATION LAYER          ?
?  (Views, Controllers, ViewModels)   ?
???????????????????????????????????????
?         APPLICATION LAYER           ?
?    (Services, Business Logic)       ?
???????????????????????????????????????
?          DOMAIN LAYER               ?
?    (Models, Entities, DTOs)         ?
???????????????????????????????????????
?       INFRASTRUCTURE LAYER          ?
?  (Data Access, External Services)   ?
???????????????????????????????????????
?         DATABASE LAYER              ?
?         (SQL Server)                ?
???????????????????????????????????????
```

### ?? Fluxo de Dados

```
User Request
    ?
Controller/Page
    ?
Service Layer (Business Logic)
    ?
Repository/DbContext
    ?
Database
    ?
Response ? ViewModel ? Data
```

---

## ?? Tecnologias Utilizadas

### ?? Frontend

#### **Bootstrap 5.3**
```html
<!-- Layout responsivo -->
<div class="container">
    <div class="row">
        <div class="col-md-6">...</div>
    </div>
</div>
```

**Componentes principais:**
- Cards para exibição de dados
- Modals para ações rápidas
- Navbar responsivo
- Forms com validação
- Tables com ordenação
- Badges para status
- Alerts para feedback

#### **jQuery & JavaScript**

**site.js - Sistema consolidado:**
```javascript
// Namespace global
window.RentalTourismSystem = {
    // Máscaras
    Masks: { ... },
    
    // Validações
    Validation: { ... },
    
    // Utils
    Utils: { ... },
    
    // AJAX
    Ajax: { ... }
};
```

**Recursos:**
- Máscaras de CPF, CNPJ, telefone, placa
- Validações client-side
- AJAX para operações assíncronas
- Formatação de valores
- Feedback visual

### ??? Backend

#### **ASP.NET Core 8.0**

**Program.cs - Configuração:**
```csharp
var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddDbContext<RentalTourismContext>();
builder.Services.AddDefaultIdentity<ApplicationUser>();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ILocacaoService, LocacaoService>();
// ...

var app = builder.Build();

// Middleware Pipeline
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(...);
```

#### **Entity Framework Core 8.0**

**DbContext:**
```csharp
public class RentalTourismContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Veiculo> Veiculos { get; set; }
    public DbSet<Locacao> Locacoes { get; set; }
    public DbSet<ManutencaoVeiculo> Manutencoes { get; set; }
    public DbSet<ReservaViagem> ReservasViagens { get; set; }
    public DbSet<PacoteViagem> PacotesViagens { get; set; }
    // ...
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configurações de relacionamentos
        // Índices, restrições, etc.
    }
}
```

**Migrations:**
```bash
# Criar migration
dotnet ef migrations add NomeDaMigration

# Aplicar migration
dotnet ef database update

# Reverter migration
dotnet ef database update PreviousMigration
```

---

## ?? Estrutura do Projeto

### ??? Organização de Pastas

```
RentalTourismSystem/
?
??? Controllers/              # Controllers MVC
?   ??? HomeController.cs
?   ??? ClientesController.cs
?   ??? VeiculosController.cs
?   ??? LocacoesController.cs
?   ??? ManutencoesController.cs
?   ??? ReservasViagensController.cs
?   ??? AccountController.cs
?   ??? ApiController.cs
?
??? Models/                   # Modelos de domínio
?   ??? Cliente.cs
?   ??? Veiculo.cs
?   ??? Locacao.cs
?   ??? ManutencaoVeiculo.cs
?   ??? ReservaViagem.cs
?   ??? PacoteViagem.cs
?   ??? Funcionario.cs
?   ??? Agencia.cs
?   ??? ApplicationUser.cs
?   ??? Documento.cs
?   ??? ViewModels/           # ViewModels
?       ??? LoginViewModel.cs
?       ??? ContratoLocacaoViewModel.cs
?       ??? LaudoVistoriaViewModel.cs
?
??? Views/                    # Views Razor
?   ??? Shared/
?   ?   ??? _Layout.cshtml
?   ?   ??? _LayoutLogin.cshtml
?   ?   ??? Error.cshtml
?   ??? Clientes/
?   ??? Veiculos/
?   ??? Locacoes/
?   ??? Manutencoes/
?   ??? ReservasViagens/
?   ??? Account/
?
??? Services/                 # Serviços de aplicação
?   ??? ILocacaoService.cs
?   ??? LocacaoService.cs
?   ??? IVeiculoService.cs
?   ??? VeiculoService.cs
?   ??? IFileService.cs
?   ??? FileService.cs
?   ??? INotificationService.cs
?   ??? NotificationService.cs
?   ??? EmailService.cs
?   ??? RelatorioService.cs
?
??? Data/                     # Acesso a dados
?   ??? RentalTourismContext.cs
?   ??? SeedData.cs
?
??? Helpers/                  # Classes auxiliares
?   ??? ValidationHelpers.cs
?   ??? FormatHelpers.cs
?   ??? DateTimeHelper.cs
?   ??? ClienteHelper.cs
?
??? Extensions/               # Extension methods
?   ??? ModelExtensions.cs
?   ??? ClienteExtensions.cs
?   ??? ReservaViagemExtensions.cs
?
??? wwwroot/                  # Arquivos estáticos
?   ??? css/
?   ??? js/
?   ?   ??? site.js          # JavaScript consolidado
?   ??? lib/                 # Bibliotecas externas
?   ??? uploads/             # Upload de arquivos
?       ??? clientes/
?       ??? veiculos/
?
??? Migrations/               # Migrations EF
?   ??? Scripts/             # Scripts SQL manuais
?
??? Docs/                     # Documentação
?   ??? README.md
?   ??? CLIENTES_GUIA_COMPLETO.md
?   ??? VEICULOS_GUIA_COMPLETO.md
?   ??? LOCACOES_GUIA_COMPLETO.md
?   ??? MANUTENCAO_GUIA_ACESSO.md
?   ??? RESERVAS_VIAGEM_GUIA_COMPLETO.md
?   ??? AUTENTICACAO_GUIA_COMPLETO.md
?   ??? UPLOAD_DOCUMENTOS.md
?   ??? GUIA_INICIO_RAPIDO.md
?
??? appsettings.json          # Configurações
??? appsettings.Development.json
??? Program.cs                # Ponto de entrada
??? RentalTourismSystem.csproj
```

---

## ??? Modelos de Dados

### ?? Diagrama ER Simplificado

```
????????????????     ????????????????     ????????????????
?   Cliente    ???????   Locacao    ???????   Veiculo    ?
????????????????     ????????????????     ????????????????
       ?                    ?                     ?
       ?                    ?                     ?
       ?                    ?                     ?
????????????????     ????????????????     ????????????????
?  Documento   ?     ? Funcionario  ?     ? Manutencao   ?
????????????????     ????????????????     ????????????????
       ?                    ?                     ?
       ?                    ?                     ?
       ?             ????????????????     ????????????????
????????????????     ?   Agencia    ?     ?ItemManutencao?
?ReservaViagem ?     ????????????????     ????????????????
????????????????            
       ?                    
       ?                    
????????????????     
?PacoteViagem  ?     
????????????????     
```

### ?? Modelos Principais

#### **Cliente**
```csharp
public class Cliente
{
    [Key]
    public int Id { get; set; }
    
    [Required, StringLength(100)]
    public string Nome { get; set; }
    
    [Required, CpfValidation, StringLength(14)]
    public string CPF { get; set; }
    
    [Required, StringLength(20)]
    public string Telefone { get; set; }
    
    [Required, EmailAddress, StringLength(100)]
    public string Email { get; set; }
    
    [Required]
    public DateTime DataNascimento { get; set; }
    
    [StringLength(20)]
    public string? CNH { get; set; }
    
    public DateTime? ValidadeCNH { get; set; }
    
    // Propriedades calculadas
    public int Idade => /* cálculo */;
    public bool CNHValida => /* validação */;
    
    // Navegação
    public virtual ICollection<Locacao> Locacoes { get; set; }
    public virtual ICollection<ReservaViagem> ReservasViagens { get; set; }
    public virtual ICollection<Documento> Documentos { get; set; }
}
```

#### **Veículo**
```csharp
public class Veiculo
{
    [Key]
    public int Id { get; set; }
    
    [Required, StringLength(50)]
    public string Marca { get; set; }
    
    [Required, StringLength(50)]
    public string Modelo { get; set; }
    
    [Required, Range(1990, 2030)]
    public int Ano { get; set; }
    
    [Required, PlacaValidation, StringLength(10)]
    public string Placa { get; set; }
    
    [Required, Column(TypeName = "decimal(10,2)")]
    public decimal ValorDiaria { get; set; }
    
    [Required]
    public int StatusCarroId { get; set; }
    public virtual StatusCarro StatusCarro { get; set; }
    
    // Navegação
    public virtual ICollection<Locacao> Locacoes { get; set; }
    public virtual ICollection<ManutencaoVeiculo> Manutencoes { get; set; }
}
```

#### **Locação**
```csharp
public class Locacao
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public DateTime DataRetirada { get; set; }
    
    [Required]
    public DateTime DataDevolucao { get; set; }
    
    public DateTime? DataDevolucaoReal { get; set; }
    
    [Required, Column(TypeName = "decimal(10,2)")]
    public decimal ValorTotal { get; set; }
    
    public int QuilometragemRetirada { get; set; }
    public int QuilometragemDevolucao { get; set; }
    
    // Foreign Keys
    [Required]
    public int ClienteId { get; set; }
    public virtual Cliente Cliente { get; set; }
    
    [Required]
    public int VeiculoId { get; set; }
    public virtual Veiculo Veiculo { get; set; }
    
    [Required]
    public int FuncionarioId { get; set; }
    public virtual Funcionario Funcionario { get; set; }
}
```

#### **Manutenção**
```csharp
public class ManutencaoVeiculo
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int VeiculoId { get; set; }
    public virtual Veiculo Veiculo { get; set; }
    
    [Required]
    public DateTime DataManutencao { get; set; }
    
    [Required, StringLength(50)]
    public string TipoManutencao { get; set; }
    
    [Required, StringLength(50)]
    public string StatusManutencao { get; set; }
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal CustoTotal { get; set; }
    
    public int QuilometragemAtual { get; set; }
    
    public bool Preventiva { get; set; }
    public bool Urgente { get; set; }
    
    // Navegação
    public virtual ICollection<ItemManutencao> Itens { get; set; }
}
```

---

## ?? APIs e Endpoints

### ?? Controllers API

#### **ApiController - Endpoints Gerais**

```csharp
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class ApiController : ControllerBase
{
    // GET: api/v1/api/validarcpf/{cpf}
    [HttpGet("validarcpf/{cpf}")]
    [RequireRateLimiting("CpfValidationPolicy")]
    public IActionResult ValidarCPF(string cpf) { ... }
    
    // GET: api/v1/api/validarplaca/{placa}
    [HttpGet("validarplaca/{placa}")]
    public IActionResult ValidarPlaca(string placa) { ... }
    
    // GET: api/v1/api/veiculos/disponiveis
    [HttpGet("veiculos/disponiveis")]
    public IActionResult VeiculosDisponiveis([FromQuery] DateTime inicio, [FromQuery] DateTime fim) { ... }
    
    // GET: api/v1/api/dashboard/stats
    [HttpGet("dashboard/stats")]
    [Authorize(Roles = "Admin,Manager")]
    public IActionResult DashboardStats() { ... }
}
```

#### **Versionamento de API**

```csharp
// Configuração em Program.cs
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Version"),
        new QueryStringApiVersionReader("version")
    );
});
```

**Uso:**
```
GET /api/v1/api/validarcpf/12345678900
GET /api/v2/api/validarcpf/12345678900
GET /api/api/validarcpf/12345678900?version=1.0
```

#### **Rate Limiting**

```csharp
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("ApiPolicy", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
    });
    
    options.AddFixedWindowLimiter("CpfValidationPolicy", limiterOptions =>
    {
        limiterOptions.PermitLimit = 20;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
    });
});
```

### ?? Exemplo de Chamada AJAX

```javascript
// Validar CPF via API
async function validarCPF(cpf) {
    try {
        const response = await fetch(`/api/v1/api/validarcpf/${cpf}`);
        const data = await response.json();
        
        if (data.valido) {
            console.log('CPF válido');
        } else {
            console.log('CPF inválido:', data.mensagem);
        }
    } catch (error) {
        console.error('Erro:', error);
    }
}
```

---

## ?? Segurança

### ??? Autenticação - ASP.NET Core Identity

#### **Configuração**

```csharp
// Program.cs
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    // Senha
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    
    // Lockout
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    
    // Usuário
    options.User.RequireUniqueEmail = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<RentalTourismContext>();
```

#### **Autorização por Roles**

```csharp
// Controller
[Authorize(Roles = "Admin")]
public class AdminController : Controller { }

[Authorize(Roles = "Admin,Manager")]
public IActionResult Edit() { }

// View
@if (User.IsInRole("Admin"))
{
    <a asp-action="Delete">Excluir</a>
}
```

### ?? Headers de Segurança

```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    
    if (context.Request.IsHttps)
    {
        context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000");
    }
    
    await next();
});
```

### ?? Anti-Forgery Token

```html
<!-- Formulário -->
<form method="post">
    @Html.AntiForgeryToken()
    <!-- campos -->
</form>
```

```csharp
// Controller
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Create(Cliente cliente) { }
```

### ?? Logging com Serilog

```csharp
// Program.cs
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "RentalTourismSystem")
    .WriteTo.Console()
    .WriteTo.File("logs/rental-tourism-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30)
    .CreateLogger();

// Uso em Controllers
_logger.LogInformation("Cliente {ClienteId} criado por {Usuario}", cliente.Id, User.Identity.Name);
_logger.LogWarning("Tentativa de acesso negado: {Action} por {Usuario}", action, User.Identity.Name);
_logger.LogError(ex, "Erro ao processar locação {LocacaoId}", id);
```

---

## ?? Configurações

### ?? appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RentalTourism;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    }
  },
  "AppSettings": {
    "EmailFrom": "noreply@litoralsul.com.br",
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "UploadPath": "wwwroot/uploads",
    "MaxFileSize": 10485760
  },
  "AllowedHosts": "*"
}
```

### ?? Variáveis de Ambiente (Produção)

```bash
# Azure App Service
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=Server=...
AppSettings__SmtpPassword=...
```

---

## ?? Deploy

### ?? Publicação

```bash
# Publicar para pasta
dotnet publish -c Release -o ./publish

# Publicar para Azure
dotnet publish -c Release /p:PublishProfile=Azure

# Criar pacote ZIP
Compress-Archive -Path ./publish/* -DestinationPath RentalTourism.zip
```

### ??? Migrations em Produção

```bash
# Gerar script SQL
dotnet ef migrations script --output migration.sql

# Aplicar via Azure
# Upload do script no Azure SQL Database
```

### ?? Checklist de Deploy

- [ ] Atualizar connection string
- [ ] Aplicar migrations
- [ ] Configurar variáveis de ambiente
- [ ] Testar autenticação
- [ ] Verificar upload de arquivos
- [ ] Configurar HTTPS
- [ ] Ativar logging
- [ ] Backup do banco
- [ ] Monitoramento ativo

---

## ?? Testes

### ?? Testes Unitários (Futuro)

```csharp
[Fact]
public void Cliente_IdadeMinimaValidacao()
{
    var cliente = new Cliente
    {
        DataNascimento = DateTime.Now.AddYears(-20)
    };
    
    Assert.True(cliente.Idade < 21);
    // Deve falhar na validação
}
```

### ?? Testes de Integração (Futuro)

```csharp
[Fact]
public async Task CriarLocacao_ComSucesso()
{
    // Arrange
    var cliente = await CriarClienteTeste();
    var veiculo = await CriarVeiculoTeste();
    
    // Act
    var locacao = await _service.CriarLocacaoAsync(cliente.Id, veiculo.Id, ...);
    
    // Assert
    Assert.NotNull(locacao);
    Assert.Equal(StatusCarro.Locado, veiculo.StatusCarroId);
}
```

---

## ?? Recursos Adicionais

### ?? Documentação Externa

- [ASP.NET Core Docs](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [Bootstrap 5](https://getbootstrap.com/docs/5.3)
- [Serilog](https://serilog.net)

### ??? Ferramentas Recomendadas

- Visual Studio 2022
- SQL Server Management Studio
- Postman (API testing)
- Azure Data Studio
- Git / GitHub

---

**Desenvolvido para:** Litoral Sul Locadora e Turismo  
**Versão:** 1.0  
**Framework:** ASP.NET Core 8.0  
**Última Atualização:** Outubro/2025
