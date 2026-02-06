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
        public IActionResult Index()
        {
            _logger.LogInformation("Página de pré-cadastro acessada");
            return View();
        }

        // POST: PreCadastro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(PreCadastroCliente model)
        {
            try
            {
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
                    Mensagem = $"Cliente {model.Nome} (CPF: {model.CPF}) realizou pré-cadastro e solicitou reserva de {model.DataInicioLocacao:dd/MM/yyyy} a {model.DataFinalLocacao:dd/MM/yyyy} ({quantidadeDias} dias). Telefone: {model.Telefone}",
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

                _logger.LogInformation("Pré-cadastro realizado com sucesso. CPF: {CPF}, Período: {DataInicio} a {DataFim}",
                    cpfLimpo, model.DataInicioLocacao, model.DataFinalLocacao);

                // Armazenar informações na TempData para exibir na página de sucesso
                TempData["NomeCliente"] = model.Nome;
                TempData["PeriodoInicio"] = model.DataInicioLocacao.ToString("dd/MM/yyyy");
                TempData["PeriodoFim"] = model.DataFinalLocacao.ToString("dd/MM/yyyy");
                TempData["QuantidadeDias"] = quantidadeDias;

                return RedirectToAction(nameof(Sucesso));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao realizar pré-cadastro");
                ModelState.AddModelError("", "Ocorreu um erro ao processar seu cadastro. Tente novamente.");
                return View(model);
            }
        }

        // GET: PreCadastro/Sucesso
        public IActionResult Sucesso()
        {
            return View();
        }

        #region Métodos Auxiliares

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
