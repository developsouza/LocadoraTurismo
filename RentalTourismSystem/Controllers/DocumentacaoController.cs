using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalTourismSystem.Services;

namespace RentalTourismSystem.Controllers
{
    [Authorize]
    public class DocumentacaoController : Controller
    {
        private readonly IDocumentacaoService _documentacaoService;
        private readonly ILogger<DocumentacaoController> _logger;

        public DocumentacaoController(
            IDocumentacaoService documentacaoService,
            ILogger<DocumentacaoController> logger)
        {
            _documentacaoService = documentacaoService;
            _logger = logger;
        }

        // GET: Documentacao
        public IActionResult Index()
        {
            try
            {
                var documentos = _documentacaoService.ObterListaDocumentos();
                return View(documentos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar lista de documentação");
                TempData["Erro"] = "Erro ao carregar documentação";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Documentacao/Visualizar/nome-do-arquivo
        public IActionResult Visualizar(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["Erro"] = "Documento não especificado";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var documento = _documentacaoService.ObterDocumento(id);

                if (documento == null)
                {
                    TempData["Erro"] = "Documento não encontrado";
                    return RedirectToAction(nameof(Index));
                }

                return View(documento);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar documento: {DocumentoId}", id);
                TempData["Erro"] = "Erro ao carregar documento";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Documentacao/Buscar
        public IActionResult Buscar(string termo)
        {
            if (string.IsNullOrWhiteSpace(termo))
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var resultados = _documentacaoService.BuscarNaDocumentacao(termo);
                ViewBag.TermoBusca = termo;
                return View(resultados);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar na documentação: {Termo}", termo);
                TempData["Erro"] = "Erro ao realizar busca";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Documentacao/Download/nome-do-arquivo
        public IActionResult Download(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["Erro"] = "Documento não especificado";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var (conteudo, nomeArquivo) = _documentacaoService.ObterArquivoParaDownload(id);

                if (conteudo == null)
                {
                    TempData["Erro"] = "Documento não encontrado";
                    return RedirectToAction(nameof(Index));
                }

                return File(conteudo, "text/markdown", nomeArquivo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fazer download do documento: {DocumentoId}", id);
                TempData["Erro"] = "Erro ao fazer download do documento";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Documentacao/GuiaRapido - Atalho para o guia de início rápido
        public IActionResult GuiaRapido()
        {
            return RedirectToAction(nameof(Visualizar), new { id = "GUIA_INICIO_RAPIDO" });
        }

        // GET: Documentacao/ReferenciaTecnica - Atalho para referência técnica
        public IActionResult ReferenciaTecnica()
        {
            return RedirectToAction(nameof(Visualizar), new { id = "REFERENCIA_TECNICA" });
        }

        // GET: Documentacao/PorPerfil
        public IActionResult PorPerfil(string perfil)
        {
            try
            {
                var documentos = _documentacaoService.ObterDocumentosPorPerfil(perfil);
                ViewBag.Perfil = perfil;
                return View(documentos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar documentos por perfil: {Perfil}", perfil);
                TempData["Erro"] = "Erro ao carregar documentação";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
