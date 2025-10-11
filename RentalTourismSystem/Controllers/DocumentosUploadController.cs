using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentalTourismSystem.Data;
using RentalTourismSystem.Models;
using RentalTourismSystem.Services;

namespace RentalTourismSystem.Controllers
{
    [Authorize]
    public class DocumentosUploadController : Controller
    {
        private readonly RentalTourismContext _context;
        private readonly IFileService _fileService;
        private readonly ILogger<DocumentosUploadController> _logger;

        // Configurações de upload
        private readonly string[] _extensoesPermitidasImagem = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
        private readonly string[] _extensoesPermitidasPdf = { ".pdf" };
        private readonly string[] _extensoesPermitidasTodas = { ".pdf", ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
        private readonly long _tamanhoMaximo = 10 * 1024 * 1024; // 10MB

        public DocumentosUploadController(
            RentalTourismContext context,
            IFileService fileService,
            ILogger<DocumentosUploadController> logger)
        {
            _context = context;
            _fileService = fileService;
            _logger = logger;
        }

        // ========== UPLOAD DE DOCUMENTOS DE CLIENTES ==========

        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> UploadCliente(int id)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Documentos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null)
            {
                _logger.LogWarning("Cliente {ClienteId} não encontrado para upload", id);
                return NotFound();
            }

            ViewBag.ClienteId = id;
            ViewBag.ClienteNome = cliente.Nome;
            ViewBag.TiposDocumento = TipoDocumentoEnum.ObterTiposCliente();
            ViewBag.DocumentosExistentes = cliente.Documentos.OrderByDescending(d => d.DataUpload).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> UploadCliente(
            int clienteId,
            IFormFile arquivo,
            string tipoDocumento,
            string? descricao)
        {
            try
            {
                // Validar cliente
                var cliente = await _context.Clientes.FindAsync(clienteId);
                if (cliente == null)
                {
                    return Json(new { success = false, message = "Cliente não encontrado" });
                }

                // Validar arquivo
                if (arquivo == null || arquivo.Length == 0)
                {
                    return Json(new { success = false, message = "Nenhum arquivo foi enviado" });
                }

                // Salvar arquivo
                var resultado = await _fileService.SalvarArquivoAsync(
                    arquivo,
                    $"clientes/{clienteId}",
                    _extensoesPermitidasTodas,
                    _tamanhoMaximo);

                if (!resultado.Success)
                {
                    return Json(new { success = false, message = resultado.ErrorMessage });
                }

                // Criar registro no banco
                var documento = new Documento
                {
                    NomeArquivo = arquivo.FileName,
                    CaminhoArquivo = resultado.FilePath,
                    TipoDocumento = tipoDocumento,
                    ContentType = arquivo.ContentType,
                    TamanhoBytes = arquivo.Length,
                    Descricao = descricao,
                    DataUpload = DateTime.Now,
                    UsuarioUpload = User.Identity?.Name,
                    ClienteId = clienteId
                };

                _context.Documentos.Add(documento);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Documento {TipoDocumento} enviado para cliente {ClienteId} por {Usuario}",
                    tipoDocumento, clienteId, User.Identity?.Name);

                return Json(new
                {
                    success = true,
                    message = "Documento enviado com sucesso!",
                    documento = new
                    {
                        id = documento.Id,
                        nomeArquivo = documento.NomeArquivo,
                        tipoDocumento = documento.TipoDocumento,
                        tamanhoFormatado = documento.TamanhoFormatado,
                        dataUpload = documento.DataUpload.ToString("dd/MM/yyyy HH:mm"),
                        ehImagem = documento.EhImagem,
                        ehPdf = documento.EhPdf
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fazer upload de documento para cliente {ClienteId}", clienteId);
                return Json(new { success = false, message = "Erro ao fazer upload do documento" });
            }
        }

        // ========== UPLOAD DE DOCUMENTOS DE VEÍCULOS ==========

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UploadVeiculo(int id)
        {
            var veiculo = await _context.Veiculos
                .Include(v => v.Documentos)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (veiculo == null)
            {
                _logger.LogWarning("Veículo {VeiculoId} não encontrado para upload", id);
                return NotFound();
            }

            ViewBag.VeiculoId = id;
            ViewBag.VeiculoInfo = $"{veiculo.Marca} {veiculo.Modelo} - {veiculo.Placa}";
            ViewBag.TiposDocumento = TipoDocumentoEnum.ObterTiposVeiculo();
            ViewBag.DocumentosExistentes = veiculo.Documentos.OrderByDescending(d => d.DataUpload).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UploadVeiculo(
            int veiculoId,
            IFormFile arquivo,
            string tipoDocumento,
            string? descricao)
        {
            try
            {
                // Validar veículo
                var veiculo = await _context.Veiculos.FindAsync(veiculoId);
                if (veiculo == null)
                {
                    return Json(new { success = false, message = "Veículo não encontrado" });
                }

                // Validar arquivo
                if (arquivo == null || arquivo.Length == 0)
                {
                    return Json(new { success = false, message = "Nenhum arquivo foi enviado" });
                }

                // Salvar arquivo
                var resultado = await _fileService.SalvarArquivoAsync(
                    arquivo,
                    $"veiculos/{veiculoId}",
                    _extensoesPermitidasTodas,
                    _tamanhoMaximo);

                if (!resultado.Success)
                {
                    return Json(new { success = false, message = resultado.ErrorMessage });
                }

                // Criar registro no banco
                var documento = new Documento
                {
                    NomeArquivo = arquivo.FileName,
                    CaminhoArquivo = resultado.FilePath,
                    TipoDocumento = tipoDocumento,
                    ContentType = arquivo.ContentType,
                    TamanhoBytes = arquivo.Length,
                    Descricao = descricao,
                    DataUpload = DateTime.Now,
                    UsuarioUpload = User.Identity?.Name,
                    VeiculoId = veiculoId
                };

                _context.Documentos.Add(documento);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Documento {TipoDocumento} enviado para veículo {VeiculoId} por {Usuario}",
                    tipoDocumento, veiculoId, User.Identity?.Name);

                return Json(new
                {
                    success = true,
                    message = "Documento enviado com sucesso!",
                    documento = new
                    {
                        id = documento.Id,
                        nomeArquivo = documento.NomeArquivo,
                        tipoDocumento = documento.TipoDocumento,
                        tamanhoFormatado = documento.TamanhoFormatado,
                        dataUpload = documento.DataUpload.ToString("dd/MM/yyyy HH:mm"),
                        ehImagem = documento.EhImagem,
                        ehPdf = documento.EhPdf
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fazer upload de documento para veículo {VeiculoId}", veiculoId);
                return Json(new { success = false, message = "Erro ao fazer upload do documento" });
            }
        }

        // ========== VISUALIZAR/BAIXAR DOCUMENTO ==========

        [HttpGet]
        public async Task<IActionResult> Download(int id)
        {
            try
            {
                var documento = await _context.Documentos.FindAsync(id);

                if (documento == null)
                {
                    _logger.LogWarning("Documento {DocumentoId} não encontrado", id);
                    return NotFound();
                }

                // Verificar permissões (simplificado - pode ser melhorado)
                if (!User.IsInRole("Admin") && !User.IsInRole("Manager"))
                {
                    _logger.LogWarning(
                        "Usuário {Usuario} sem permissão para acessar documento {DocumentoId}",
                        User.Identity?.Name, id);
                    return Forbid();
                }

                var resultado = await _fileService.ObterArquivoAsync(documento.CaminhoArquivo);

                if (!resultado.Success || resultado.FileBytes == null)
                {
                    _logger.LogError("Erro ao obter arquivo físico do documento {DocumentoId}", id);
                    TempData["Erro"] = "Arquivo não encontrado no servidor";
                    return RedirectToAction("Index", "Home");
                }

                _logger.LogInformation(
                    "Documento {DocumentoId} baixado por {Usuario}",
                    id, User.Identity?.Name);

                return File(resultado.FileBytes, resultado.ContentType ?? "application/octet-stream", documento.NomeArquivo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao baixar documento {DocumentoId}", id);
                TempData["Erro"] = "Erro ao baixar documento";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Visualizar(int id)
        {
            try
            {
                var documento = await _context.Documentos
                    .Include(d => d.Cliente)
                    .Include(d => d.Veiculo)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (documento == null)
                {
                    return NotFound();
                }

                // Verificar permissões
                if (!User.IsInRole("Admin") && !User.IsInRole("Manager"))
                {
                    return Forbid();
                }

                // Se for imagem, exibir inline
                if (documento.EhImagem)
                {
                    var resultado = await _fileService.ObterArquivoAsync(documento.CaminhoArquivo);
                    if (resultado.Success && resultado.FileBytes != null)
                    {
                        return File(resultado.FileBytes, resultado.ContentType ?? "image/jpeg");
                    }
                }

                // Para PDF e outros, retornar view com visualizador
                return View(documento);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao visualizar documento {DocumentoId}", id);
                TempData["Erro"] = "Erro ao visualizar documento";
                return RedirectToAction("Index", "Home");
            }
        }

        // ========== EXCLUIR DOCUMENTO ==========

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Excluir(int id)
        {
            try
            {
                var documento = await _context.Documentos.FindAsync(id);

                if (documento == null)
                {
                    return Json(new { success = false, message = "Documento não encontrado" });
                }

                // Excluir arquivo físico
                await _fileService.ExcluirArquivoAsync(documento.CaminhoArquivo);

                // Excluir registro do banco
                _context.Documentos.Remove(documento);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Documento {DocumentoId} ({TipoDocumento}) excluído por {Usuario}",
                    id, documento.TipoDocumento, User.Identity?.Name);

                return Json(new { success = true, message = "Documento excluído com sucesso!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir documento {DocumentoId}", id);
                return Json(new { success = false, message = "Erro ao excluir documento" });
            }
        }

        // ========== LISTAR DOCUMENTOS (API para AJAX) ==========

        [HttpGet]
        public async Task<IActionResult> ListarPorCliente(int clienteId)
        {
            try
            {
                var documentos = await _context.Documentos
                    .Where(d => d.ClienteId == clienteId)
                    .OrderByDescending(d => d.DataUpload)
                    .Select(d => new
                    {
                        id = d.Id,
                        nomeArquivo = d.NomeArquivo,
                        tipoDocumento = d.TipoDocumento,
                        tamanhoFormatado = d.TamanhoFormatado,
                        dataUpload = d.DataUpload.ToString("dd/MM/yyyy HH:mm"),
                        ehImagem = d.EhImagem,
                        ehPdf = d.EhPdf,
                        iconeFont = d.IconeFont,
                        iconeColor = d.IconeColor,
                        descricao = d.Descricao
                    })
                    .ToListAsync();

                return Json(documentos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar documentos do cliente {ClienteId}", clienteId);
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListarPorVeiculo(int veiculoId)
        {
            try
            {
                var documentos = await _context.Documentos
                    .Where(d => d.VeiculoId == veiculoId)
                    .OrderByDescending(d => d.DataUpload)
                    .Select(d => new
                    {
                        id = d.Id,
                        nomeArquivo = d.NomeArquivo,
                        tipoDocumento = d.TipoDocumento,
                        tamanhoFormatado = d.TamanhoFormatado,
                        dataUpload = d.DataUpload.ToString("dd/MM/yyyy HH:mm"),
                        ehImagem = d.EhImagem,
                        ehPdf = d.EhPdf,
                        iconeFont = d.IconeFont,
                        iconeColor = d.IconeColor,
                        descricao = d.Descricao
                    })
                    .ToListAsync();

                return Json(documentos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar documentos do veículo {VeiculoId}", veiculoId);
                return Json(new List<object>());
            }
        }
    }
}
