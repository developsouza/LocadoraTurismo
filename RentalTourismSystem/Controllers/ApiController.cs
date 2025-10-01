using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using RentalTourismSystem.Data;
using System.ComponentModel.DataAnnotations;

namespace RentalTourismSystem.Controllers
{
    /// <summary>
    /// API Controller para operações do Sistema de Locação e Turismo
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Authorize] // Toda a API requer autenticação
    public class ApiController : ControllerBase
    {
        private readonly RentalTourismContext _context;
        private readonly ILogger<ApiController> _logger;
        private readonly IMemoryCache _cache;

        public ApiController(
            RentalTourismContext context,
            ILogger<ApiController> logger,
            IMemoryCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        /// <summary>
        /// Obtém os dados de um cliente específico
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <returns>Dados do cliente</returns>
        [HttpGet("cliente/{id}")]
        [ProducesResponseType(typeof(ClienteResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorResponseDto), 404)]
        [ProducesResponseType(typeof(ErrorResponseDto), 400)]
        public async Task<ActionResult<ClienteResponseDto>> GetCliente([FromRoute] int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Tentativa de buscar cliente com ID inválido: {ClienteId} por {User}", id, User.Identity?.Name);
                    return BadRequest(new ErrorResponseDto { Message = "ID do cliente deve ser um número positivo" });
                }

                _logger.LogInformation("Buscando dados do cliente {ClienteId} via API por {User}", id, User.Identity?.Name);

                var cliente = await _context.Clientes
                    .AsNoTracking()
                    .Select(c => new ClienteResponseDto
                    {
                        Id = c.Id,
                        Nome = c.Nome,
                        Email = c.Email,
                        Telefone = c.Telefone,
                        Cpf = c.Cpf,
                        DataNascimento = c.DataNascimento,
                        NumeroHabilitacao = c.NumeroHabilitacao,
                        ValidadeCNH = c.ValidadeCNH,
                        HabilitacaoVencida = c.ValidadeCNH.HasValue && c.ValidadeCNH.Value.Date < DateTime.Now.Date,
                        DiasParaVencimentoCNH = c.ValidadeCNH.HasValue ?
                            (int)(c.ValidadeCNH.Value.Date - DateTime.Now.Date).TotalDays : (int?)null,
                        TotalLocacoes = c.Locacoes.Count(),
                        TotalReservas = c.ReservasViagens.Count(),
                        ValorTotalGasto = c.Locacoes.Sum(l => l.ValorTotal) + c.ReservasViagens.Sum(r => r.ValorTotal)
                    })
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (cliente == null)
                {
                    _logger.LogWarning("Cliente {ClienteId} não encontrado via API por {User}", id, User.Identity?.Name);
                    return NotFound(new ErrorResponseDto { Message = "Cliente não encontrado" });
                }

                return Ok(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar cliente {ClienteId} via API por {User}", id, User.Identity?.Name);
                return StatusCode(500, new ErrorResponseDto { Message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Obtém os dados de um veículo específico
        /// </summary>
        /// <param name="id">ID do veículo</param>
        /// <returns>Dados do veículo</returns>
        [HttpGet("veiculo/{id}")]
        [ProducesResponseType(typeof(VeiculoResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorResponseDto), 404)]
        [ProducesResponseType(typeof(ErrorResponseDto), 400)]
        public async Task<ActionResult<VeiculoResponseDto>> GetVeiculo([FromRoute] int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Tentativa de buscar veículo com ID inválido: {VeiculoId} por {User}", id, User.Identity?.Name);
                    return BadRequest(new ErrorResponseDto { Message = "ID do veículo deve ser um número positivo" });
                }

                _logger.LogInformation("Buscando dados do veículo {VeiculoId} via API por {User}", id, User.Identity?.Name);

                var veiculo = await _context.Veiculos
                    .Include(v => v.StatusCarro)
                    .Include(v => v.Agencia)
                    .AsNoTracking()
                    .Select(v => new VeiculoResponseDto
                    {
                        Id = v.Id,
                        Marca = v.Marca,
                        Modelo = v.Modelo,
                        Ano = v.Ano,
                        Placa = v.Placa,
                        Cor = v.Cor,
                        ValorDiaria = v.ValorDiaria,
                        Quilometragem = v.Quilometragem,
                        Status = v.StatusCarro.Status,
                        StatusId = v.StatusCarroId,
                        Agencia = v.Agencia.Nome,
                        AgenciaId = v.AgenciaId,
                        Disponivel = v.StatusCarro.Status == "Disponível",
                        TotalLocacoes = v.Locacoes.Count(),
                        ReceitaTotal = v.Locacoes.Sum(l => l.ValorTotal),
                        UltimaLocacao = v.Locacoes.Any() ? v.Locacoes.Max(l => l.DataRetirada) : (DateTime?)null
                    })
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (veiculo == null)
                {
                    _logger.LogWarning("Veículo {VeiculoId} não encontrado via API por {User}", id, User.Identity?.Name);
                    return NotFound(new ErrorResponseDto { Message = "Veículo não encontrado" });
                }

                return Ok(veiculo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar veículo {VeiculoId} via API por {User}", id, User.Identity?.Name);
                return StatusCode(500, new ErrorResponseDto { Message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Obtém os dados de um pacote de viagem específico
        /// </summary>
        /// <param name="id">ID do pacote</param>
        /// <returns>Dados do pacote</returns>
        [HttpGet("pacote/{id}")]
        [ProducesResponseType(typeof(PacoteResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorResponseDto), 404)]
        [ProducesResponseType(typeof(ErrorResponseDto), 400)]
        public async Task<ActionResult<PacoteResponseDto>> GetPacote([FromRoute] int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Tentativa de buscar pacote com ID inválido: {PacoteId} por {User}", id, User.Identity?.Name);
                    return BadRequest(new ErrorResponseDto { Message = "ID do pacote deve ser um número positivo" });
                }

                _logger.LogInformation("Buscando dados do pacote {PacoteId} via API por {User}", id, User.Identity?.Name);

                var pacote = await _context.PacotesViagens
                    .AsNoTracking()
                    .Select(p => new PacoteResponseDto
                    {
                        Id = p.Id,
                        Nome = p.Nome,
                        Descricao = p.Descricao,
                        Destino = p.Destino,
                        Duracao = p.Duracao,
                        Preco = p.Preco,
                        TotalReservas = p.ReservasViagens.Count(),
                        ReceitaTotal = p.ReservasViagens.Sum(r => r.ValorTotal),
                        TotalPessoas = p.ReservasViagens.Sum(r => r.Quantidade)
                    })
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (pacote == null)
                {
                    _logger.LogWarning("Pacote {PacoteId} não encontrado via API por {User}", id, User.Identity?.Name);
                    return NotFound(new ErrorResponseDto { Message = "Pacote não encontrado" });
                }

                return Ok(pacote);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pacote {PacoteId} via API por {User}", id, User.Identity?.Name);
                return StatusCode(500, new ErrorResponseDto { Message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Valida um CPF
        /// </summary>
        /// <param name="cpf">CPF para validação</param>
        /// <returns>Resultado da validação</returns>
        [HttpGet("validar/cpf/{cpf}")]
        [ProducesResponseType(typeof(ValidacaoResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorResponseDto), 400)]
        public ActionResult<ValidacaoResponseDto> ValidarCPF([FromRoute] string cpf)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cpf))
                {
                    _logger.LogWarning("Tentativa de validar CPF vazio via API por {User}", User.Identity?.Name);
                    return BadRequest(new ErrorResponseDto { Message = "CPF não pode estar vazio" });
                }

                _logger.LogInformation("Validando CPF via API por {User}", User.Identity?.Name);

                // Remover formatação
                cpf = new string(cpf.Where(char.IsDigit).ToArray());

                var resultado = new ValidacaoResponseDto
                {
                    Valido = ValidarCPFInterno(cpf),
                    Valor = cpf,
                    Mensagem = ValidarCPFInterno(cpf) ? "CPF válido" : "CPF inválido"
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar CPF via API por {User}", User.Identity?.Name);
                return StatusCode(500, new ErrorResponseDto { Message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Verifica disponibilidade de veículo
        /// </summary>
        /// <param name="veiculoId">ID do veículo</param>
        /// <param name="dataInicio">Data de início</param>
        /// <param name="dataFim">Data de fim</param>
        /// <returns>Status de disponibilidade</returns>
        [HttpGet("verificar/disponibilidade")]
        [ProducesResponseType(typeof(DisponibilidadeResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorResponseDto), 400)]
        public async Task<ActionResult<DisponibilidadeResponseDto>> VerificarDisponibilidade(
            [FromQuery] int veiculoId,
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim)
        {
            try
            {
                if (veiculoId <= 0)
                {
                    return BadRequest(new ErrorResponseDto { Message = "ID do veículo inválido" });
                }

                if (dataInicio >= dataFim)
                {
                    return BadRequest(new ErrorResponseDto { Message = "Data de início deve ser anterior à data de fim" });
                }

                if (dataInicio.Date < DateTime.Now.Date)
                {
                    return BadRequest(new ErrorResponseDto { Message = "Data de início não pode ser no passado" });
                }

                _logger.LogInformation("Verificando disponibilidade do veículo {VeiculoId} para período {DataInicio} a {DataFim} via API por {User}",
                    veiculoId, dataInicio.ToString("dd/MM/yyyy"), dataFim.ToString("dd/MM/yyyy"), User.Identity?.Name);

                // Verificar se o veículo existe e está ativo
                var veiculo = await _context.Veiculos
                    .Include(v => v.StatusCarro)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(v => v.Id == veiculoId);

                if (veiculo == null)
                {
                    return NotFound(new ErrorResponseDto { Message = "Veículo não encontrado" });
                }

                var disponivel = true;
                var motivo = "Veículo disponível para o período";

                // Verificar status do veículo
                if (veiculo.StatusCarro.Status != "Disponível")
                {
                    disponivel = false;
                    motivo = $"Veículo está com status '{veiculo.StatusCarro.Status}'";
                }
                else
                {
                    // Verificar conflitos de datas
                    var conflito = await _context.Locacoes
                        .Where(l => l.VeiculoId == veiculoId && l.DataDevolucaoReal == null)
                        .AnyAsync(l => (dataInicio < l.DataDevolucao && dataFim > l.DataRetirada));

                    if (conflito)
                    {
                        disponivel = false;
                        motivo = "Veículo já está reservado para o período solicitado";
                    }
                }

                var resultado = new DisponibilidadeResponseDto
                {
                    VeiculoId = veiculoId,
                    DataInicio = dataInicio,
                    DataFim = dataFim,
                    Disponivel = disponivel,
                    Motivo = motivo,
                    StatusVeiculo = veiculo.StatusCarro.Status
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar disponibilidade via API por {User}", User.Identity?.Name);
                return StatusCode(500, new ErrorResponseDto { Message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Calcula o preço de um pacote para quantidade específica
        /// </summary>
        /// <param name="id">ID do pacote</param>
        /// <param name="quantidade">Quantidade de pessoas</param>
        /// <returns>Cálculo do preço</returns>
        [HttpGet("pacote/{id}/preco")]
        [ProducesResponseType(typeof(PrecoResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorResponseDto), 400)]
        public async Task<ActionResult<PrecoResponseDto>> CalcularPrecoPacote([FromRoute] int id, [FromQuery] int quantidade = 1)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new ErrorResponseDto { Message = "ID do pacote inválido" });
                }

                if (quantidade <= 0)
                {
                    return BadRequest(new ErrorResponseDto { Message = "Quantidade deve ser maior que zero" });
                }

                _logger.LogInformation("Calculando preço do pacote {PacoteId} para {Quantidade} pessoas via API por {User}",
                    id, quantidade, User.Identity?.Name);

                var pacote = await _context.PacotesViagens
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (pacote == null)
                {
                    return NotFound(new ErrorResponseDto { Message = "Pacote não encontrado" });
                }

                var resultado = new PrecoResponseDto
                {
                    PacoteId = id,
                    NomePacote = pacote.Nome,
                    PrecoUnitario = pacote.Preco,
                    Quantidade = quantidade,
                    PrecoTotal = pacote.Preco * quantidade
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular preço do pacote via API por {User}", User.Identity?.Name);
                return StatusCode(500, new ErrorResponseDto { Message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Obtém dados do dashboard principal
        /// </summary>
        /// <returns>Dados consolidados do dashboard</returns>
        [HttpGet("dashboard/dados")]
        [ProducesResponseType(typeof(DashboardResponseDto), 200)]
        [Authorize(Roles = "Admin,Manager")] // Dados sensíveis apenas para gestores
        public async Task<ActionResult<DashboardResponseDto>> GetDadosDashboard()
        {
            try
            {
                _logger.LogInformation("Dados do dashboard solicitados via API por {User}", User.Identity?.Name);

                var cacheKey = "dashboard_dados";
                if (_cache.TryGetValue(cacheKey, out DashboardResponseDto cachedData))
                {
                    return Ok(cachedData);
                }

                var hoje = DateTime.Now;
                var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);

                var dados = new DashboardResponseDto
                {
                    // Dados gerais
                    TotalClientes = await _context.Clientes.CountAsync(),
                    TotalVeiculos = await _context.Veiculos.CountAsync(),
                    TotalPacotes = await _context.PacotesViagens.CountAsync(),
                    TotalFuncionarios = await _context.Funcionarios.CountAsync(),

                    // Veículos por status
                    VeiculosDisponiveis = await _context.Veiculos
                        .Include(v => v.StatusCarro)
                        .CountAsync(v => v.StatusCarro.Status == "Disponível"),
                    VeiculosAlugados = await _context.Veiculos
                        .Include(v => v.StatusCarro)
                        .CountAsync(v => v.StatusCarro.Status == "Alugado"),
                    VeiculosManutencao = await _context.Veiculos
                        .Include(v => v.StatusCarro)
                        .CountAsync(v => v.StatusCarro.Status == "Manutenção"),

                    // Locações
                    LocacoesAtivas = await _context.Locacoes
                        .CountAsync(l => l.DataDevolucaoReal == null),
                    LocacoesMes = await _context.Locacoes
                        .CountAsync(l => l.DataRetirada >= inicioMes),
                    ReceitaLocacoesMes = await _context.Locacoes
                        .Where(l => l.DataRetirada >= inicioMes)
                        .SumAsync(l => l.ValorTotal),

                    // Reservas
                    ReservasAtivas = await _context.ReservasViagens
                        .Include(r => r.StatusReservaViagem)
                        .CountAsync(r => r.StatusReservaViagem.Status == "Confirmada" || r.StatusReservaViagem.Status == "Pendente"),
                    ReservasMes = await _context.ReservasViagens
                        .CountAsync(r => r.DataReserva >= inicioMes),
                    ReceitaReservasMes = await _context.ReservasViagens
                        .Include(r => r.StatusReservaViagem)
                        .Where(r => r.DataReserva >= inicioMes && r.StatusReservaViagem.Status == "Confirmada")
                        .SumAsync(r => r.ValorTotal),

                    // CNHs
                    CNHsVencendo = await _context.Clientes
                        .CountAsync(c => c.ValidadeCNH.HasValue &&
                                       c.ValidadeCNH.Value.Date <= hoje.AddDays(30).Date &&
                                       c.ValidadeCNH.Value.Date >= hoje.Date),

                    DataAtualizacao = DateTime.Now
                };

                dados.ReceitaTotalMes = dados.ReceitaLocacoesMes + dados.ReceitaReservasMes;

                // Cache por 5 minutos
                _cache.Set(cacheKey, dados, TimeSpan.FromMinutes(5));

                return Ok(dados);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter dados do dashboard via API por {User}", User.Identity?.Name);
                return StatusCode(500, new ErrorResponseDto { Message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Obtém receita mensal por ano
        /// </summary>
        /// <param name="ano">Ano para consulta</param>
        /// <returns>Receita mensal do ano</returns>
        [HttpGet("relatorio/receita-mensal/{ano}")]
        [ProducesResponseType(typeof(ReceitaMensalResponseDto), 200)]
        [Authorize(Roles = "Admin,Manager")] // Relatórios apenas para gestores
        public async Task<ActionResult<ReceitaMensalResponseDto>> GetReceitaMensal([FromRoute] int ano)
        {
            try
            {
                if (ano < 2020 || ano > DateTime.Now.Year + 1)
                {
                    return BadRequest(new ErrorResponseDto { Message = "Ano inválido" });
                }

                _logger.LogInformation("Relatório de receita mensal {Ano} solicitado via API por {User}", ano, User.Identity?.Name);

                var cacheKey = $"receita_mensal_{ano}";
                if (_cache.TryGetValue(cacheKey, out ReceitaMensalResponseDto cachedData))
                {
                    return Ok(cachedData);
                }

                var inicioAno = new DateTime(ano, 1, 1);
                var fimAno = inicioAno.AddYears(1).AddDays(-1);

                var receitaLocacoes = await _context.Locacoes
                    .Where(l => l.DataRetirada >= inicioAno && l.DataRetirada <= fimAno)
                    .GroupBy(l => l.DataRetirada.Month)
                    .Select(g => new { Mes = g.Key, Receita = g.Sum(l => l.ValorTotal) })
                    .ToListAsync();

                var receitaReservas = await _context.ReservasViagens
                    .Include(r => r.StatusReservaViagem)
                    .Where(r => r.DataReserva >= inicioAno && r.DataReserva <= fimAno &&
                               r.StatusReservaViagem.Status == "Confirmada")
                    .GroupBy(r => r.DataReserva.Month)
                    .Select(g => new { Mes = g.Key, Receita = g.Sum(r => r.ValorTotal) })
                    .ToListAsync();

                var dadosMensais = new List<ReceitaMensalItemDto>();
                var mesesNome = new[] { "", "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho",
                    "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro" };

                for (int mes = 1; mes <= 12; mes++)
                {
                    var receitaL = receitaLocacoes.FirstOrDefault(r => r.Mes == mes)?.Receita ?? 0;
                    var receitaR = receitaReservas.FirstOrDefault(r => r.Mes == mes)?.Receita ?? 0;

                    dadosMensais.Add(new ReceitaMensalItemDto
                    {
                        Mes = mes,
                        NomeMes = mesesNome[mes],
                        ReceitaLocacoes = receitaL,
                        ReceitaReservas = receitaR,
                        ReceitaTotal = receitaL + receitaR
                    });
                }

                var resultado = new ReceitaMensalResponseDto
                {
                    Ano = ano,
                    DadosMensais = dadosMensais,
                    ReceitaTotalAno = dadosMensais.Sum(d => d.ReceitaTotal)
                };

                // Cache por 1 hora
                _cache.Set(cacheKey, resultado, TimeSpan.FromHours(1));

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter receita mensal via API por {User}", User.Identity?.Name);
                return StatusCode(500, new ErrorResponseDto { Message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Alterar status de veículo
        /// </summary>
        /// <param name="id">ID do veículo</param>
        /// <param name="request">Dados da alteração</param>
        /// <returns>Resultado da operação</returns>
        [HttpPost("veiculo/{id}/status")]
        [ProducesResponseType(typeof(OperacaoResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorResponseDto), 400)]
        [Authorize(Roles = "Admin,Manager")] // Apenas gestores podem alterar status
        public async Task<ActionResult<OperacaoResponseDto>> AlterarStatusVeiculo([FromRoute] int id, [FromBody] AlterarStatusRequest request)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new ErrorResponseDto { Message = "ID do veículo inválido" });
                }

                if (request.NovoStatusId <= 0)
                {
                    return BadRequest(new ErrorResponseDto { Message = "ID do status inválido" });
                }

                _logger.LogInformation("Alteração de status do veículo {VeiculoId} solicitada via API por {User}", id, User.Identity?.Name);

                var veiculo = await _context.Veiculos
                    .Include(v => v.StatusCarro)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (veiculo == null)
                {
                    return NotFound(new ErrorResponseDto { Message = "Veículo não encontrado" });
                }

                var novoStatus = await _context.StatusCarros.FindAsync(request.NovoStatusId);
                if (novoStatus == null)
                {
                    return BadRequest(new ErrorResponseDto { Message = "Status não encontrado" });
                }

                // Verificar regras de negócio
                if (novoStatus.Status == "Disponível")
                {
                    var temLocacaoAtiva = await _context.Locacoes
                        .AnyAsync(l => l.VeiculoId == id && l.DataDevolucaoReal == null);

                    if (temLocacaoAtiva)
                    {
                        return BadRequest(new ErrorResponseDto { Message = "Não é possível alterar para 'Disponível'. O veículo possui locação ativa." });
                    }
                }

                var statusAnterior = veiculo.StatusCarro.Status;
                veiculo.StatusCarroId = request.NovoStatusId;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Status do veículo {VeiculoId} alterado de '{StatusAnterior}' para '{NovoStatus}' por {User}. Motivo: {Motivo}",
                    id, statusAnterior, novoStatus.Status, User.Identity?.Name, request.Motivo ?? "Não informado");

                // Invalidar cache
                _cache.Remove("dashboard_dados");

                return Ok(new OperacaoResponseDto
                {
                    Sucesso = true,
                    Mensagem = $"Status alterado de '{statusAnterior}' para '{novoStatus.Status}' com sucesso!",
                    Dados = new { StatusAnterior = statusAnterior, NovoStatus = novoStatus.Status }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar status do veículo via API por {User}", User.Identity?.Name);
                return StatusCode(500, new ErrorResponseDto { Message = "Erro interno do servidor" });
            }
        }

        // ========== MÉTODOS AUXILIARES ==========

        private bool ValidarCPFInterno(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf) || cpf.Length != 11)
                return false;

            // Verificar se todos os dígitos são iguais
            if (cpf.All(c => c == cpf[0]))
                return false;

            // Calcular primeiro dígito verificador
            int soma = 0;
            for (int i = 0; i < 9; i++)
            {
                soma += int.Parse(cpf[i].ToString()) * (10 - i);
            }

            int primeiroDigito = (soma * 10) % 11;
            if (primeiroDigito >= 10) primeiroDigito = 0;

            if (int.Parse(cpf[9].ToString()) != primeiroDigito)
                return false;

            // Calcular segundo dígito verificador
            soma = 0;
            for (int i = 0; i < 10; i++)
            {
                soma += int.Parse(cpf[i].ToString()) * (11 - i);
            }

            int segundoDigito = (soma * 10) % 11;
            if (segundoDigito >= 10) segundoDigito = 0;

            return int.Parse(cpf[10].ToString()) == segundoDigito;
        }
    }

    // ========== DTOs PARA RESPOSTAS DA API ==========

    public class ClienteResponseDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public string Cpf { get; set; } = string.Empty;
        public DateTime? DataNascimento { get; set; }
        public string? NumeroHabilitacao { get; set; }
        public DateTime? ValidadeCNH { get; set; }
        public bool HabilitacaoVencida { get; set; }
        public int? DiasParaVencimentoCNH { get; set; }
        public int TotalLocacoes { get; set; }
        public int TotalReservas { get; set; }
        public decimal ValorTotalGasto { get; set; }
    }

    public class VeiculoResponseDto
    {
        public int Id { get; set; }
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int Ano { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string? Cor { get; set; }
        public decimal ValorDiaria { get; set; }
        public int? Quilometragem { get; set; }
        public string Status { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public string Agencia { get; set; } = string.Empty;
        public int AgenciaId { get; set; }
        public bool Disponivel { get; set; }
        public int TotalLocacoes { get; set; }
        public decimal ReceitaTotal { get; set; }
        public DateTime? UltimaLocacao { get; set; }
    }

    public class PacoteResponseDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public string Destino { get; set; } = string.Empty;
        public int Duracao { get; set; }
        public decimal Preco { get; set; }
        public int TotalReservas { get; set; }
        public decimal ReceitaTotal { get; set; }
        public int TotalPessoas { get; set; }
    }

    public class ValidacaoResponseDto
    {
        public bool Valido { get; set; }
        public string Valor { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
    }

    public class DisponibilidadeResponseDto
    {
        public int VeiculoId { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public bool Disponivel { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public string StatusVeiculo { get; set; } = string.Empty;
    }

    public class PrecoResponseDto
    {
        public int PacoteId { get; set; }
        public string NomePacote { get; set; } = string.Empty;
        public decimal PrecoUnitario { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoTotal { get; set; }
    }

    public class DashboardResponseDto
    {
        public int TotalClientes { get; set; }
        public int TotalVeiculos { get; set; }
        public int TotalPacotes { get; set; }
        public int TotalFuncionarios { get; set; }
        public int VeiculosDisponiveis { get; set; }
        public int VeiculosAlugados { get; set; }
        public int VeiculosManutencao { get; set; }
        public int LocacoesAtivas { get; set; }
        public int LocacoesMes { get; set; }
        public decimal ReceitaLocacoesMes { get; set; }
        public int ReservasAtivas { get; set; }
        public int ReservasMes { get; set; }
        public decimal ReceitaReservasMes { get; set; }
        public decimal ReceitaTotalMes { get; set; }
        public int CNHsVencendo { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }

    public class ReceitaMensalResponseDto
    {
        public int Ano { get; set; }
        public List<ReceitaMensalItemDto> DadosMensais { get; set; } = new();
        public decimal ReceitaTotalAno { get; set; }
    }

    public class ReceitaMensalItemDto
    {
        public int Mes { get; set; }
        public string NomeMes { get; set; } = string.Empty;
        public decimal ReceitaLocacoes { get; set; }
        public decimal ReceitaReservas { get; set; }
        public decimal ReceitaTotal { get; set; }
    }

    public class OperacaoResponseDto
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public object? Dados { get; set; }
    }

    public class ErrorResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    // ========== DTOs PARA REQUESTS ==========

    public class AlterarStatusRequest
    {
        [Required(ErrorMessage = "ID do status é obrigatório")]
        public int NovoStatusId { get; set; }

        public string? Motivo { get; set; }
    }
}