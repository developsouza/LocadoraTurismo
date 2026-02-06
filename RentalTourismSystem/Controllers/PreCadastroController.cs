using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentalTourismSystem.Data;
using RentalTourismSystem.Models;

namespace RentalTourismSystem.Controllers
{
    /// <summary>
    /// Controller público para pré-cadastro de clientes (sem autenticação)
    /// </summary>
    public class PreCadastroController : Controller
    {
        private readonly RentalTourismContext _context;
        private readonly ILogger<PreCadastroController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PreCadastroController(
            RentalTourismContext context,
            ILogger<PreCadastroController> logger,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: PreCadastro
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Página de pré-cadastro acessada");

            // Carregar lista de veículos disponíveis
            var veiculosDisponiveis = await _context.Veiculos
                .Include(v => v.StatusCarro)
                .Where(v => v.StatusCarroId == 1) // Apenas veículos disponíveis
                .OrderBy(v => v.Marca)
                .ThenBy(v => v.Modelo)
                .Select(v => new
                {
                    v.Id,
                    Descricao = $"{v.Marca} {v.Modelo} ({v.Ano}) - Placa: {v.Placa} - R$ {v.ValorDiaria:F2}/dia"
                })
                .ToListAsync();

            ViewBag.Veiculos = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                veiculosDisponiveis,
                "Id",
                "Descricao"
            );

            return View();
        }

        // POST: PreCadastro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(PreCadastroCliente model)
        {
            try
            {
                // Recarregar lista de veículos para ViewBag em caso de erro
                await CarregarListaVeiculos();

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Validar se CPF já existe
                var cpfLimpo = LimparCPF(model.CPF);
                var cpfExiste = await _context.Clientes
                    .AnyAsync(c => c.CPF.Replace(".", "").Replace("-", "") == cpfLimpo);

                if (cpfExiste)
                {
                    ModelState.AddModelError("CPF", "Este CPF já está cadastrado em nosso sistema.");
                    return View(model);
                }

                // Validar se o veículo existe e está disponível
                var veiculo = await _context.Veiculos
                    .Include(v => v.StatusCarro)
                    .FirstOrDefaultAsync(v => v.Id == model.VeiculoId);

                if (veiculo == null)
                {
                    ModelState.AddModelError("VeiculoId", "Veículo não encontrado.");
                    return View(model);
                }

                if (veiculo.StatusCarroId != 1) // Não está disponível
                {
                    ModelState.AddModelError("VeiculoId", $"Veículo não está disponível. Status atual: {veiculo.StatusCarro?.Status ?? "Desconhecido"}");
                    return View(model);
                }

                // Verificar disponibilidade do veículo no período selecionado
                var veiculoDisponivel = await VerificarDisponibilidadeVeiculoAsync(
                    model.VeiculoId,
                    model.DataInicioLocacao,
                    model.DataFinalLocacao
                );

                if (!veiculoDisponivel)
                {
                    // Calcular próximo período disponível
                    var proximoPeriodoDisponivel = await CalcularProximoPeriodoDisponivelAsync(
                        model.VeiculoId,
                        model.DataInicioLocacao,
                        model.DataFinalLocacao
                    );

                    if (proximoPeriodoDisponivel.HasValue)
                    {
                        ModelState.AddModelError("VeiculoId",
                            $"O veículo {veiculo.Marca} {veiculo.Modelo} não está disponível no período selecionado. " +
                            $"Próximo período disponível: a partir de {proximoPeriodoDisponivel.Value:dd/MM/yyyy}");
                    }
                    else
                    {
                        ModelState.AddModelError("VeiculoId",
                            $"O veículo {veiculo.Marca} {veiculo.Modelo} não está disponível no período selecionado. " +
                            "Por favor, escolha outro veículo ou entre em contato conosco.");
                    }
                    return View(model);
                }

                // Validar arquivo de CNH
                if (model.CNHUpload == null || model.CNHUpload.Length == 0)
                {
                    ModelState.AddModelError("CNHUpload", "O upload da CNH é obrigatório.");
                    return View(model);
                }

                // Validar tipo e tamanho do arquivo
                var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
                var extensao = Path.GetExtension(model.CNHUpload.FileName).ToLowerInvariant();

                if (!extensoesPermitidas.Contains(extensao))
                {
                    ModelState.AddModelError("CNHUpload", "Apenas arquivos JPG, JPEG, PNG ou PDF são permitidos.");
                    return View(model);
                }

                // Validar tamanho (máximo 5MB)
                if (model.CNHUpload.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("CNHUpload", "O arquivo deve ter no máximo 5MB.");
                    return View(model);
                }

                // Salvar arquivo da CNH
                var cnhPath = await SalvarArquivoCNH(model.CNHUpload, cpfLimpo);

                // Criar cliente com pré-cadastro
                var cliente = new Cliente
                {
                    Nome = model.Nome.Trim(),
                    CPF = model.CPF,
                    Telefone = model.Telefone,
                    CNHPath = cnhPath,
                    // Campos obrigatórios com valores temporários (serão completados posteriormente)
                    Email = $"precadastro_{cpfLimpo}@temp.com", // Email temporário
                    Endereco = "A completar",
                    DataNascimento = DateTime.Now.AddYears(-21), // Data temporária (21 anos)
                    DataCadastro = DateTime.Now
                };

                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();

                // Calcular quantidade de dias
                var quantidadeDias = (model.DataFinalLocacao - model.DataInicioLocacao).Days;

                // Criar notificação para o sistema
                var notificacao = new Notificacao
                {
                    Titulo = "Novo Pré-Cadastro e Reserva",
                    Mensagem = $"Cliente {model.Nome} (CPF: {model.CPF}) realizou pré-cadastro e solicitou reserva do veículo {veiculo.Marca} {veiculo.Modelo} (Placa: {veiculo.Placa}) de {model.DataInicioLocacao:dd/MM/yyyy} a {model.DataFinalLocacao:dd/MM/yyyy} ({quantidadeDias} dias). Telefone: {model.Telefone}",
                    Tipo = "info",
                    Categoria = "PreCadastro",
                    ClienteId = cliente.Id,
                    Prioridade = 2, // Alta prioridade
                    LinkAcao = $"/Clientes/Details/{cliente.Id}",
                    TextoLinkAcao = "Ver Detalhes do Cliente",
                    Lida = false,
                    DataCriacao = DateTime.Now
                };

                _context.Notificacoes.Add(notificacao);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Pré-cadastro realizado com sucesso. CPF: {CPF}, Veículo: {VeiculoId}, Período: {DataInicio} a {DataFim}",
                    cpfLimpo, model.VeiculoId, model.DataInicioLocacao, model.DataFinalLocacao);

                // Armazenar informações na TempData para exibir na página de sucesso
                TempData["NomeCliente"] = model.Nome;
                TempData["VeiculoDescricao"] = $"{veiculo.Marca} {veiculo.Modelo} ({veiculo.Ano})";
                TempData["PeriodoInicio"] = model.DataInicioLocacao.ToString("dd/MM/yyyy");
                TempData["PeriodoFim"] = model.DataFinalLocacao.ToString("dd/MM/yyyy");
                TempData["QuantidadeDias"] = quantidadeDias;

                return RedirectToAction(nameof(Sucesso));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao realizar pré-cadastro");
                ModelState.AddModelError("", "Ocorreu um erro ao processar seu cadastro. Tente novamente.");
                await CarregarListaVeiculos();
                return View(model);
            }
        }

        // GET: PreCadastro/Sucesso
        public IActionResult Sucesso()
        {
            return View();
        }

        #region Métodos Auxiliares

        private async Task CarregarListaVeiculos()
        {
            var veiculosDisponiveis = await _context.Veiculos
                .Include(v => v.StatusCarro)
                .Where(v => v.StatusCarroId == 1) // Apenas veículos disponíveis
                .OrderBy(v => v.Marca)
                .ThenBy(v => v.Modelo)
                .Select(v => new
                {
                    v.Id,
                    Descricao = $"{v.Marca} {v.Modelo} ({v.Ano}) - Placa: {v.Placa} - R$ {v.ValorDiaria:F2}/dia"
                })
                .ToListAsync();

            ViewBag.Veiculos = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                veiculosDisponiveis,
                "Id",
                "Descricao"
            );
        }

        private async Task<bool> VerificarDisponibilidadeVeiculoAsync(int veiculoId, DateTime dataInicio, DateTime dataFim)
        {
            try
            {
                // Verificar conflitos com locações existentes no período
                var conflitos = await _context.Locacoes
                    .Where(l => l.VeiculoId == veiculoId &&
                               l.DataDevolucaoReal == null && // Apenas locações ativas (não finalizadas)
                               ((l.DataRetirada < dataFim && l.DataDevolucao > dataInicio))) // Sobreposição de período
                    .AnyAsync();

                return !conflitos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar disponibilidade do veículo {VeiculoId}", veiculoId);
                return false;
            }
        }

        private async Task<DateTime?> CalcularProximoPeriodoDisponivelAsync(int veiculoId, DateTime dataInicioDesejada, DateTime dataFimDesejada)
        {
            try
            {
                // Buscar todas as locações futuras e ativas do veículo
                var locacoesFuturas = await _context.Locacoes
                    .Where(l => l.VeiculoId == veiculoId &&
                               l.DataDevolucaoReal == null && // Apenas locações ativas
                               l.DataRetirada >= dataInicioDesejada.Date) // A partir da data desejada
                    .OrderBy(l => l.DataRetirada)
                    .Select(l => new { l.DataRetirada, l.DataDevolucao })
                    .ToListAsync();

                if (!locacoesFuturas.Any())
                {
                    // Não há locações futuras, disponível imediatamente
                    return dataInicioDesejada;
                }

                // Verificar se há gap entre a data desejada e a primeira locação
                var primeiraLocacao = locacoesFuturas.First();
                var diasNecessarios = (dataFimDesejada - dataInicioDesejada).Days;

                if (dataInicioDesejada < primeiraLocacao.DataRetirada)
                {
                    // Há um gap antes da primeira locação
                    var diasDisponiveis = (primeiraLocacao.DataRetirada - dataInicioDesejada).Days;
                    if (diasDisponiveis >= diasNecessarios)
                    {
                        // O período cabe antes da primeira locação
                        return dataInicioDesejada;
                    }
                }

                // Procurar gap entre locações consecutivas ou após a última
                for (int i = 0; i < locacoesFuturas.Count; i++)
                {
                    var locacaoAtual = locacoesFuturas[i];
                    DateTime dataInicioDisponivel = locacaoAtual.DataDevolucao.AddDays(1);

                    if (i < locacoesFuturas.Count - 1)
                    {
                        // Há uma próxima locação
                        var proximaLocacao = locacoesFuturas[i + 1];
                        var diasDisponiveis = (proximaLocacao.DataRetirada - dataInicioDisponivel).Days;

                        if (diasDisponiveis >= diasNecessarios)
                        {
                            // Período cabe entre as duas locações
                            return dataInicioDisponivel;
                        }
                    }
                    else
                    {
                        // É a última locação, veículo fica disponível após ela
                        return dataInicioDisponivel;
                    }
                }

                // Caso não encontre período disponível (não deveria chegar aqui)
                return locacoesFuturas.Last().DataDevolucao.AddDays(1);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular próximo período disponível para veículo {VeiculoId}", veiculoId);
                return null;
            }
        }

        private string LimparCPF(string cpf)
        {
            return cpf.Replace(".", "").Replace("-", "").Trim();
        }

        private async Task<string> SalvarArquivoCNH(IFormFile arquivo, string cpf)
        {
            try
            {
                // Criar pasta se não existir
                var uploadsPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "cnh");
                Directory.CreateDirectory(uploadsPath);

                // Gerar nome único para o arquivo
                var extensao = Path.GetExtension(arquivo.FileName);
                var nomeArquivo = $"CNH_{cpf}_{DateTime.Now:yyyyMMddHHmmss}{extensao}";
                var caminhoCompleto = Path.Combine(uploadsPath, nomeArquivo);

                // Salvar arquivo
                using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                {
                    await arquivo.CopyToAsync(stream);
                }

                // Retornar caminho relativo
                return $"/uploads/cnh/{nomeArquivo}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar arquivo CNH");
                throw new Exception("Erro ao salvar arquivo da CNH", ex);
            }
        }

        #endregion
    }
}
