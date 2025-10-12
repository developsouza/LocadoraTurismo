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
                _logger.LogWarning("Tentativa de criar item sem manutenção ID por {User}", User.Identity?.Name);
                return NotFound();
            }

            try
            {
                var manutencao = await _context.ManutencoesVeiculos
                    .Include(m => m.Veiculo)
                    .FirstOrDefaultAsync(m => m.Id == manutencaoId);

                if (manutencao == null)
                {
                    _logger.LogWarning("Manutenção {ManutencaoId} não encontrada por {User}", manutencaoId, User.Identity?.Name);
                    return NotFound();
                }

                ViewBag.Manutencao = manutencao;
                var item = new ItemManutencao { ManutencaoVeiculoId = manutencaoId.Value };

                _logger.LogInformation("Formulário de criação de item de manutenção acessado por {User}", User.Identity?.Name);
                return View(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de criação de item por {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar formulário.";
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

                    _logger.LogInformation("Item de manutenção criado para manutenção {ManutencaoId} por {User}", 
                        item.ManutencaoVeiculoId, User.Identity?.Name);

                    TempData["Sucesso"] = "Item adicionado com sucesso!";
                    return RedirectToAction("Details", "Manutencoes", new { id = item.ManutencaoVeiculoId });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar item de manutenção por {User}", User.Identity?.Name);
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
                _logger.LogWarning("Tentativa de edição de item com ID nulo por {User}", User.Identity?.Name);
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
                    _logger.LogWarning("Item de manutenção {ItemId} não encontrado por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                ViewBag.Manutencao = item.ManutencaoVeiculo;
                _logger.LogInformation("Formulário de edição do item {ItemId} acessado por {User}", id, User.Identity?.Name);
                return View(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de edição do item {ItemId} por {User}", id, User.Identity?.Name);
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
                _logger.LogWarning("Tentativa de edição com ID inconsistente {Id} != {ItemId} por {User}", 
                    id, item.Id, User.Identity?.Name);
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Item de manutenção {ItemId} atualizado por {User}", item.Id, User.Identity?.Name);

                    TempData["Sucesso"] = "Item atualizado com sucesso!";
                    return RedirectToAction("Details", "Manutencoes", new { id = item.ManutencaoVeiculoId });
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ItemExists(item.Id))
                {
                    _logger.LogWarning("Item {ItemId} não existe mais durante edição por {User}", item.Id, User.Identity?.Name);
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Erro de concorrência ao editar item {ItemId} por {User}", item.Id, User.Identity?.Name);
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
                _logger.LogWarning("Tentativa de exclusão de item com ID nulo por {User}", User.Identity?.Name);
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
                    _logger.LogWarning("Item de manutenção {ItemId} não encontrado por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                ViewBag.Manutencao = item.ManutencaoVeiculo;
                _logger.LogInformation("Formulário de confirmação de exclusão do item {ItemId} acessado por {User}", id, User.Identity?.Name);
                return View(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de exclusão do item {ItemId} por {User}", id, User.Identity?.Name);
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

                    _logger.LogInformation("Item de manutenção {ItemId} excluído por {User}", id, User.Identity?.Name);
                    TempData["Sucesso"] = "Item excluído com sucesso!";
                }
                else
                {
                    _logger.LogWarning("Tentativa de exclusão de item inexistente {ItemId} por {User}", id, User.Identity?.Name);
                    TempData["Erro"] = "Item não encontrado.";
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

        // ========== MÉTODOS AUXILIARES ==========

        private bool ItemExists(int id)
        {
            return _context.ItensManutencao.Any(e => e.Id == id);
        }
    }
}
