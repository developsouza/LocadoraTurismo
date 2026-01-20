using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentalTourismSystem.Data;
using RentalTourismSystem.Models.ViewModels;

namespace RentalTourismSystem.Controllers
{
    [Authorize]
    public class DocumentosController : Controller
    {
        private readonly RentalTourismContext _context;
        private readonly ILogger<DocumentosController> _logger;

        public DocumentosController(RentalTourismContext context, ILogger<DocumentosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Documentos/ContratoLocacao/5
        public async Task<IActionResult> ContratoLocacao(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Tentativa de gerar contrato com ID nulo");
                return NotFound();
            }

            try
            {
                var locacao = await _context.Locacoes
                    .Include(l => l.Cliente)
                    .Include(l => l.Veiculo)
                        .ThenInclude(v => v.Agencia)
                    .Include(l => l.Veiculo)
                        .ThenInclude(v => v.StatusCarro)
                    .Include(l => l.Funcionario)
                    .Include(l => l.Agencia)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (locacao == null)
                {
                    _logger.LogWarning("Locação {LocacaoId} não encontrada para gerar contrato", id);
                    return NotFound();
                }

                // Verificação adicional de dados obrigatórios
                if (locacao.Cliente == null)
                {
                    _logger.LogError("Cliente não encontrado para locação {LocacaoId}", id);
                    TempData["Erro"] = "Dados do cliente não encontrados.";
                    return RedirectToAction("Details", "Locacoes", new { id });
                }

                if (locacao.Veiculo == null)
                {
                    _logger.LogError("Veículo não encontrado para locação {LocacaoId}", id);
                    TempData["Erro"] = "Dados do veículo não encontrados.";
                    return RedirectToAction("Details", "Locacoes", new { id });
                }

                if (locacao.Agencia == null)
                {
                    _logger.LogError("Agência não encontrada para locação {LocacaoId}", id);
                    TempData["Erro"] = "Dados da agência não encontrados.";
                    return RedirectToAction("Details", "Locacoes", new { id });
                }

                // Preparar ViewModel com dados calculados
                var viewModel = new ContratoLocacaoViewModel
                {
                    Locacao = locacao,
                    DiasLocacao = (locacao.DataDevolucao - locacao.DataRetirada).Days,
                    ValorDiaria = locacao.Veiculo.ValorDiaria,
                    ValorCaucao = locacao.Veiculo.ValorDiaria * 4, // 4 diárias como caução
                    ValorFranquia = 3000.00m, // Valor padrão da franquia
                    DataEmissao = DateTime.Now
                };

                _logger.LogInformation("Contrato de locação {LocacaoId} gerado por {User}", id, User.Identity?.Name);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar contrato da locação {LocacaoId}", id);
                TempData["Erro"] = "Erro ao gerar contrato. Tente novamente.";
                return RedirectToAction("Details", "Locacoes", new { id });
            }
        }

        // GET: Documentos/LaudoVistoria/5
        public async Task<IActionResult> LaudoVistoria(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Tentativa de gerar laudo com ID nulo");
                return NotFound();
            }

            try
            {
                var locacao = await _context.Locacoes
                    .Include(l => l.Cliente)
                    .Include(l => l.Veiculo)
                        .ThenInclude(v => v.Agencia)
                    .Include(l => l.Veiculo)
                        .ThenInclude(v => v.StatusCarro)
                    .Include(l => l.Funcionario)
                    .Include(l => l.Agencia)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (locacao == null)
                {
                    _logger.LogWarning("Locação {LocacaoId} não encontrada para gerar laudo", id);
                    return NotFound();
                }

                // Criar ViewModel com itens de vistoria padrão
                var viewModel = new LaudoVistoriaViewModel
                {
                    Locacao = locacao,
                    DataVistoria = locacao.DataRetirada,
                    ItensVistoria = ObterItensVistoriaPadrao()
                };

                _logger.LogInformation("Laudo de vistoria da locação {LocacaoId} gerado por {User}", id, User.Identity?.Name);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar laudo da locação {LocacaoId}", id);
                TempData["Erro"] = "Erro ao gerar laudo de vistoria. Tente novamente.";
                return RedirectToAction("Details", "Locacoes", new { id });
            }
        }

        // Método auxiliar para criar itens de vistoria padrão
        private List<ItemVistoria> ObterItensVistoriaPadrao()
        {
            return new List<ItemVistoria>
            {
                new ItemVistoria { Numero = 1, Descricao = "BUZINA", Funcionando = true, NaoFunciona = false },
                new ItemVistoria { Numero = 2, Descricao = "CINTO DE SEGURANÇA", Funcionando = true, NaoFunciona = false },
                new ItemVistoria { Numero = 3, Descricao = "QUEBRA SOL", Funcionando = true, NaoFunciona = false },
                new ItemVistoria { Numero = 4, Descricao = "RETROVISOR INTERNO", Funcionando = true, NaoFunciona = false },
                new ItemVistoria { Numero = 5, Descricao = "RETROVISOR - DIREITO/ESQUERDO", Funcionando = true, NaoFunciona = false },
                new ItemVistoria { Numero = 6, Descricao = "LIMPADOR DE PARA-BRISA", Funcionando = true, NaoFunciona = false },
                new ItemVistoria { Numero = 7, Descricao = "LIMPADOR PARA-BRISA TRASEIRO", Funcionando = true, NaoFunciona = false },
                new ItemVistoria { Numero = 8, Descricao = "FAROL BAIXO", Funcionando = true, NaoFunciona = false },
                new ItemVistoria { Numero = 9, Descricao = "FAROL ALTO", Funcionando = true, NaoFunciona = false },
                new ItemVistoria { Numero = 10, Descricao = "MEIA LUZ", Funcionando = true, NaoFunciona = false },
                new ItemVistoria { Numero = 11, Descricao = "LUZ DE FREIO", Funcionando = true, NaoFunciona = false },
                new ItemVistoria { Numero = 12, Descricao = "LUZ DE RÉ", Funcionando = true, NaoFunciona = false },
                new ItemVistoria { Numero = 13, Descricao = "LUZ DA PLACA", Funcionando = true, NaoFunciona = false },
                new ItemVistoria { Numero = 14, Descricao = "LUZES DO PAINEL", Funcionando = true, NaoFunciona = false },
                new ItemVistoria { Numero = 15, Descricao = "SETA - DIREITA/ESQUERDA", Funcionando = true, NaoFunciona = false },
                new ItemVistoria { Numero = 16, Descricao = "PISCA ALERTA", Funcionando = true, NaoFunciona = false },
                new ItemVistoria { Numero = 17, Descricao = "LUZ INTERNA", Funcionando = true, NaoFunciona = false },
                new ItemVistoria { Numero = 18, Descricao = "VELOCÍMETRO / TACÓGRAFO", Funcionando = true, NaoFunciona = false },
                new ItemVistoria { Numero = 19, Descricao = "FREIOS", Funcionando = true, NaoFunciona = false },
                new ItemVistoria { Numero = 20, Descricao = "MACACO", Funcionando = true, NaoFunciona = false },
                new ItemVistoria { Numero = 21, Descricao = "CHAVE DE RODA", Ok = true, Faltando = false },
                new ItemVistoria { Numero = 22, Descricao = "TRIÂNGULO DE SINALIZAÇÃO", Ok = true, Faltando = false },
                new ItemVistoria { Numero = 23, Descricao = "EXTINTOR DE INCÊNDIO", EmDia = true, Vencido = false },
                new ItemVistoria { Numero = 24, Descricao = "PORTAS - TRAVAS", Funcionando = true, NaoFunciona = false },
                new ItemVistoria { Numero = 25, Descricao = "ALARME", Funcionando = true, NaoFunciona = false },
                new ItemVistoria { Numero = 26, Descricao = "FECHAMENTO DAS JANELAS", Funcionando = true, NaoFunciona = false },
                new ItemVistoria { Numero = 27, Descricao = "PARA-BRISA", Normal = true, Trincado = false },
                new ItemVistoria { Numero = 28, Descricao = "ÓLEO DO MOTOR", EmDia = true, Vencido = false },
                new ItemVistoria { Numero = 29, Descricao = "ÓLEO DE FREIO", NoNivel = true, Completar = false },
                new ItemVistoria { Numero = 30, Descricao = "NÍVEL DA ÁGUA DO RADIADOR", NoNivel = true, Completar = false },
                new ItemVistoria { Numero = 31, Descricao = "PNEUS (ESTADO/CALIBRAGEM)", Bom = true, Ruim = false },
                new ItemVistoria { Numero = 32, Descricao = "PNEU RESERVA (ESTEPE)", Bom = true, Ruim = false },
                new ItemVistoria { Numero = 33, Descricao = "BANCOS ENCOSTO/ASSENTOS", Bom = true, Ruim = false },
                new ItemVistoria { Numero = 34, Descricao = "PARA-CHOQUE DIANTEIRO", Normal = true, RiscadoAmassado = false },
                new ItemVistoria { Numero = 35, Descricao = "PARA-CHOQUE TRASEIRO", Normal = true, RiscadoAmassado = false },
                new ItemVistoria { Numero = 36, Descricao = "LATARIA", Normal = true, RiscadoAmassado = false }
            };
        }

        // GET: Documentos/ContratoLocacaoTemplate
        public IActionResult ContratoLocacaoTemplate()
        {
            _logger.LogInformation("Template de contrato de locação acessado por {User}", User.Identity?.Name);
            return View();
        }

        // GET: Documentos/LaudoVistoriaTemplate
        public IActionResult LaudoVistoriaTemplate()
        {
            _logger.LogInformation("Template de laudo de vistoria acessado por {User}", User.Identity?.Name);
            return View();
        }
    }
}