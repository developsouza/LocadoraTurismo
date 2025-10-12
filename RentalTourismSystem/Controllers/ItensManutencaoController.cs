using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentalTourismSystem.Data;
using RentalTourismSystem.Models;

namespace RentalTourismSystem.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ItensManutencaoController : Controller
    {
        private readonly RentalTourismContext _context;
        private readonly ILogger<ItensManutencaoController> _logger;

        public ItensManutencaoController(RentalTourismContext context, ILogger<ItensManutencaoController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: ItensManutencao/Create?manutencaoId=5
        public async Task<IActionResult> Create(int? manutencaoId)
        {
            if (manutencaoId == null)
            {
                _logger.LogWarning("Tentativa de criar item sem manuten��o ID por {User}", User.Identity?.Name);
                return NotFound();
            }

            try
            {
                var manutencao = await _context.ManutencoesVeiculos
                    .Include(m => m.Veiculo)
                    .FirstOrDefaultAsync(m => m.Id == manutencaoId);

                if (manutencao == null)
                {
                    _logger.LogWarning("Manuten��o {ManutencaoId} n�o encontrada por {User}", manutencaoId, User.Identity?.Name);
                    return NotFound();
                }

                ViewBag.Manutencao = manutencao;
                var item = new ItemManutencao { ManutencaoVeiculoId = manutencaoId.Value };

                _logger.LogInformation("Formul�rio de cria��o de item de manuten��o acessado por {User}", User.Identity?.Name);
                return View(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formul�rio de cria��o de item por {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar formul�rio.";
                return RedirectToAction("Details", "Manutencoes", new { id = manutencaoId });
            }
        }

        // POST: ItensManutencao/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ManutencaoVeiculoId,Descricao,Tipo,Quantidade,ValorUnitario,Fornecedor,CodigoPeca,Observacoes")] ItemManutencao item)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(item);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Item de manuten��o criado para manuten��o {ManutencaoId} por {User}", 
                        item.ManutencaoVeiculoId, User.Identity?.Name);

                    TempData["Sucesso"] = "Item adicionado com sucesso!";
                    return RedirectToAction("Details", "Manutencoes", new { id = item.ManutencaoVeiculoId });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar item de manuten��o por {User}", User.Identity?.Name);
                ModelState.AddModelError(string.Empty, "Erro ao salvar item. Tente novamente.");
            }

            var manutencao = await _context.ManutencoesVeiculos
                .Include(m => m.Veiculo)
                .FirstOrDefaultAsync(m => m.Id == item.ManutencaoVeiculoId);
            ViewBag.Manutencao = manutencao;

            return View(item);
        }

        // GET: ItensManutencao/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Tentativa de edi��o de item com ID nulo por {User}", User.Identity?.Name);
                return NotFound();
            }

            try
            {
                var item = await _context.ItensManutencao
                    .Include(i => i.ManutencaoVeiculo)
                        .ThenInclude(m => m.Veiculo)
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (item == null)
                {
                    _logger.LogWarning("Item de manuten��o {ItemId} n�o encontrado por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                ViewBag.Manutencao = item.ManutencaoVeiculo;
                _logger.LogInformation("Formul�rio de edi��o do item {ItemId} acessado por {User}", id, User.Identity?.Name);
                return View(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formul�rio de edi��o do item {ItemId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar dados do item.";
                return RedirectToAction("Index", "Manutencoes");
            }
        }

        // POST: ItensManutencao/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ManutencaoVeiculoId,Descricao,Tipo,Quantidade,ValorUnitario,Fornecedor,CodigoPeca,Observacoes")] ItemManutencao item)
        {
            if (id != item.Id)
            {
                _logger.LogWarning("Tentativa de edi��o com ID inconsistente {Id} != {ItemId} por {User}", 
                    id, item.Id, User.Identity?.Name);
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Item de manuten��o {ItemId} atualizado por {User}", item.Id, User.Identity?.Name);

                    TempData["Sucesso"] = "Item atualizado com sucesso!";
                    return RedirectToAction("Details", "Manutencoes", new { id = item.ManutencaoVeiculoId });
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ItemExists(item.Id))
                {
                    _logger.LogWarning("Item {ItemId} n�o existe mais durante edi��o por {User}", item.Id, User.Identity?.Name);
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Erro de concorr�ncia ao editar item {ItemId} por {User}", item.Id, User.Identity?.Name);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao editar item {ItemId} por {User}", item.Id, User.Identity?.Name);
                ModelState.AddModelError(string.Empty, "Erro ao salvar item. Tente novamente.");
            }

            var manutencao = await _context.ManutencoesVeiculos
                .Include(m => m.Veiculo)
                .FirstOrDefaultAsync(m => m.Id == item.ManutencaoVeiculoId);
            ViewBag.Manutencao = manutencao;

            return View(item);
        }

        // GET: ItensManutencao/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Tentativa de exclus�o de item com ID nulo por {User}", User.Identity?.Name);
                return NotFound();
            }

            try
            {
                var item = await _context.ItensManutencao
                    .Include(i => i.ManutencaoVeiculo)
                        .ThenInclude(m => m.Veiculo)
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (item == null)
                {
                    _logger.LogWarning("Item de manuten��o {ItemId} n�o encontrado por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                ViewBag.Manutencao = item.ManutencaoVeiculo;
                _logger.LogInformation("Formul�rio de confirma��o de exclus�o do item {ItemId} acessado por {User}", id, User.Identity?.Name);
                return View(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formul�rio de exclus�o do item {ItemId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar dados do item.";
                return RedirectToAction("Index", "Manutencoes");
            }
        }

        // POST: ItensManutencao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int? manutencaoId = null;

            try
            {
                var item = await _context.ItensManutencao.FindAsync(id);

                if (item != null)
                {
                    manutencaoId = item.ManutencaoVeiculoId;
                    _context.ItensManutencao.Remove(item);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Item de manuten��o {ItemId} exclu�do por {User}", id, User.Identity?.Name);
                    TempData["Sucesso"] = "Item exclu�do com sucesso!";
                }
                else
                {
                    _logger.LogWarning("Tentativa de exclus�o de item inexistente {ItemId} por {User}", id, User.Identity?.Name);
                    TempData["Erro"] = "Item n�o encontrado.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir item {ItemId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao excluir item. Tente novamente.";
            }

            if (manutencaoId.HasValue)
            {
                return RedirectToAction("Details", "Manutencoes", new { id = manutencaoId });
            }

            return RedirectToAction("Index", "Manutencoes");
        }

        // ========== M�TODOS AUXILIARES ==========

        private bool ItemExists(int id)
        {
            return _context.ItensManutencao.Any(e => e.Id == id);
        }
    }
}
