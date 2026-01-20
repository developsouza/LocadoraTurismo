using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentalTourismSystem.Data;
using RentalTourismSystem.Models;
using RentalTourismSystem.ViewModels;

namespace RentalTourismSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly RentalTourismContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            RentalTourismContext context,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            // Limpar cookie existente para login limpo
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        _logger.LogInformation("Usuário {Email} ({Nome}) logado com sucesso",
                            model.Email, user.NomeCompleto);

                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        return RedirectToAction("Index", "Home");
                    }
                }

                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction("LoginWith2fa", new { returnUrl, model.RememberMe });
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Conta do usuário {Email} está bloqueada", model.Email);
                    ModelState.AddModelError(string.Empty, "Sua conta está temporariamente bloqueada por tentativas de login inválidas.");
                    return View(model);
                }

                _logger.LogWarning("Tentativa de login inválida para {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "Email ou senha inválidos.");
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register()
        {
            ViewBag.AgenciaId = new SelectList(await _context.Agencias.ToListAsync(), "Id", "Nome");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    NomeCompleto = model.NomeCompleto,
                    Cpf = model.Cpf,
                    Cargo = model.Cargo,
                    AgenciaId = model.AgenciaId,
                    DataCadastro = DateTime.Now,
                    Ativo = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Atribuir role padrão "Employee" 
                    await _userManager.AddToRoleAsync(user, "Employee");

                    _logger.LogInformation("Novo usuário criado: {Email} ({Nome})", user.Email, user.NomeCompleto);
                    TempData["Sucesso"] = $"Usuário {user.NomeCompleto} criado com sucesso!";
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewBag.AgenciaId = new SelectList(await _context.Agencias.ToListAsync(), "Id", "Nome", model.AgenciaId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var userName = User.Identity?.Name ?? "Usuário desconhecido";
            await _signInManager.SignOutAsync();

            _logger.LogInformation("Usuário {UserName} fez logout", userName);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                _logger.LogInformation("Usuário {Email} alterou a senha", user.Email);
                TempData["Sucesso"] = "Senha alterada com sucesso!";
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageUsers()
        {
            var users = await _userManager.Users
                .Include(u => u.Agencia)
                .OrderBy(u => u.NomeCompleto)
                .ToListAsync();

            var userList = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new
                {
                    Id = user.Id,
                    NomeCompleto = user.NomeCompleto,
                    Email = user.Email,
                    Cargo = user.Cargo ?? "Não informado",
                    Agencia = user.Agencia?.Nome ?? "Não vinculado",
                    Roles = string.Join(", ", roles),
                    Ativo = user.Ativo,
                    DataCadastro = user.DataCadastro
                });
            }

            return View(userList);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            user.Ativo = !user.Ativo;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                var status = user.Ativo ? "ativado" : "desativado";
                _logger.LogInformation("Usuário {Email} foi {Status} por {Admin}",
                    user.Email, status, User.Identity?.Name);

                TempData["Sucesso"] = $"Usuário {user.NomeCompleto} foi {status} com sucesso!";
            }
            else
            {
                TempData["Erro"] = "Erro ao alterar status do usuário.";
            }

            return RedirectToAction(nameof(ManageUsers));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUserRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            // Remover roles existentes
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // Adicionar nova role
            await _userManager.AddToRoleAsync(user, role);

            _logger.LogInformation("Role do usuário {Email} alterada para {Role} por {Admin}",
                user.Email, role, User.Identity?.Name);

            TempData["Sucesso"] = $"Permissão do usuário {user.NomeCompleto} alterada para {role}!";
            return RedirectToAction(nameof(ManageUsers));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditUser(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Tentativa de edição de usuário com ID nulo por {User}", User.Identity?.Name);
                return NotFound();
            }

            try
            {
                var user = await _userManager.Users
                    .Include(u => u.Agencia)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    _logger.LogWarning("Tentativa de edição de usuário inexistente {UserId} por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                var roles = await _userManager.GetRolesAsync(user);
                var currentRole = roles.FirstOrDefault() ?? "Employee";

                var model = new EditUserViewModel
                {
                    Id = user.Id,
                    NomeCompleto = user.NomeCompleto,
                    Email = user.Email,
                    Cpf = user.Cpf,
                    Cargo = user.Cargo,
                    AgenciaId = user.AgenciaId,
                    Ativo = user.Ativo,
                    Role = currentRole
                };

                ViewBag.AgenciaId = new SelectList(await _context.Agencias.ToListAsync(), "Id", "Nome", model.AgenciaId);
                _logger.LogInformation("Formulário de edição do usuário {UserId} acessado por {Admin}", id, User.Identity?.Name);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de edição do usuário {UserId} por {Admin}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar dados do usuário para edição.";
                return RedirectToAction(nameof(ManageUsers));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AgenciaId = new SelectList(await _context.Agencias.ToListAsync(), "Id", "Nome", model.AgenciaId);
                return View(model);
            }

            try
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    _logger.LogWarning("Usuário {UserId} não encontrado durante edição por {Admin}", model.Id, User.Identity?.Name);
                    return NotFound();
                }

                // Verificar se o email já está em uso por outro usuário
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    ModelState.AddModelError("Email", "Este email já está em uso por outro usuário.");
                    ViewBag.AgenciaId = new SelectList(await _context.Agencias.ToListAsync(), "Id", "Nome", model.AgenciaId);
                    return View(model);
                }

                // Atualizar dados do usuário
                user.NomeCompleto = model.NomeCompleto;
                user.Email = model.Email;
                user.UserName = model.Email;
                user.Cpf = model.Cpf;
                user.Cargo = model.Cargo;
                user.AgenciaId = model.AgenciaId;
                user.Ativo = model.Ativo;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    ViewBag.AgenciaId = new SelectList(await _context.Agencias.ToListAsync(), "Id", "Nome", model.AgenciaId);
                    return View(model);
                }

                // Atualizar senha se fornecida
                if (!string.IsNullOrWhiteSpace(model.NewPassword))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                    if (!passwordResult.Succeeded)
                    {
                        foreach (var error in passwordResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        ViewBag.AgenciaId = new SelectList(await _context.Agencias.ToListAsync(), "Id", "Nome", model.AgenciaId);
                        return View(model);
                    }
                    _logger.LogInformation("Senha do usuário {Email} foi alterada pelo Admin {Admin}", user.Email, User.Identity?.Name);
                }

                // Atualizar role se alterada
                if (!string.IsNullOrEmpty(model.Role))
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    if (!currentRoles.Contains(model.Role))
                    {
                        await _userManager.RemoveFromRolesAsync(user, currentRoles);
                        await _userManager.AddToRoleAsync(user, model.Role);
                        _logger.LogInformation("Role do usuário {Email} alterada para {Role} por {Admin}",
                            user.Email, model.Role, User.Identity?.Name);
                    }
                }

                _logger.LogInformation("Usuário {UserId} ({Email}) atualizado por {Admin}",
                    user.Id, user.Email, User.Identity?.Name);

                TempData["Sucesso"] = $"Usuário {user.NomeCompleto} atualizado com sucesso!";
                return RedirectToAction(nameof(ManageUsers));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao editar usuário {UserId} por {Admin}", model.Id, User.Identity?.Name);
                ModelState.AddModelError(string.Empty, "Erro interno do sistema. Tente novamente.");
                ViewBag.AgenciaId = new SelectList(await _context.Agencias.ToListAsync(), "Id", "Nome", model.AgenciaId);
                return View(model);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Tentativa de exclusão de usuário com ID nulo por {Admin}", User.Identity?.Name);
                return NotFound();
            }

            try
            {
                var user = await _userManager.Users
                    .Include(u => u.Agencia)
                    .Include(u => u.Funcionario)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    _logger.LogWarning("Tentativa de exclusão de usuário inexistente {UserId} por {Admin}", id, User.Identity?.Name);
                    return NotFound();
                }

                // Verificar se há impedimentos para exclusão
                var impedimentos = new List<string>();

                // Verificar se o usuário está vinculado a um funcionário com locações
                if (user.Funcionario != null)
                {
                    var funcionario = await _context.Funcionarios
                        .Include(f => f.Locacoes)
                        .FirstOrDefaultAsync(f => f.Id == user.FuncionarioId);

                    if (funcionario != null)
                    {
                        var locacoesAtivas = funcionario.Locacoes?.Where(l => l.DataDevolucaoReal == null).Count() ?? 0;
                        if (locacoesAtivas > 0)
                        {
                            impedimentos.Add($"{locacoesAtivas} locação(ões) ativa(s) vinculada(s) ao funcionário");
                        }

                        var totalLocacoes = funcionario.Locacoes?.Count ?? 0;
                        if (totalLocacoes > 0)
                        {
                            impedimentos.Add($"{totalLocacoes} locação(ões) no histórico do funcionário");
                        }
                    }
                }

                if (impedimentos.Any())
                {
                    ViewBag.Impedimentos = impedimentos;
                    _logger.LogInformation("Exclusão do usuário {UserId} bloqueada devido a impedimentos: {Impedimentos}",
                        id, string.Join(", ", impedimentos));
                }

                var roles = await _userManager.GetRolesAsync(user);
                ViewBag.UserRole = string.Join(", ", roles);

                _logger.LogInformation("Formulário de confirmação de exclusão do usuário {UserId} acessado por {Admin}", id, User.Identity?.Name);
                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de exclusão do usuário {UserId} por {Admin}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar dados do usuário para exclusão.";
                return RedirectToAction(nameof(ManageUsers));
            }
        }

        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUserConfirmed(string id)
        {
            try
            {
                var user = await _userManager.Users
                    .Include(u => u.Funcionario)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    _logger.LogWarning("Tentativa de exclusão de usuário inexistente {UserId} por {Admin}", id, User.Identity?.Name);
                    TempData["Erro"] = "Usuário não encontrado.";
                    return RedirectToAction(nameof(ManageUsers));
                }

                // Verificar novamente os impedimentos (segurança extra)
                if (user.Funcionario != null)
                {
                    var funcionario = await _context.Funcionarios
                        .Include(f => f.Locacoes)
                        .FirstOrDefaultAsync(f => f.Id == user.FuncionarioId);

                    if (funcionario != null)
                    {
                        var locacoesAtivas = funcionario.Locacoes?.Where(l => l.DataDevolucaoReal == null).Count() ?? 0;

                        if (locacoesAtivas > 0)
                        {
                            TempData["Erro"] = $"Não é possível excluir o usuário. Existem {locacoesAtivas} locação(ões) ativa(s) vinculada(s) ao funcionário associado.";
                            _logger.LogWarning("Exclusão do usuário {UserId} negada devido a locações ativas por {Admin}", id, User.Identity?.Name);
                            return RedirectToAction(nameof(DeleteUser), new { id });
                        }
                    }
                }

                string nomeUsuario = user.NomeCompleto;
                string emailUsuario = user.Email ?? "Email não disponível";

                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Usuário {UserId} ('{NomeUsuario}' - {EmailUsuario}) excluído por {Admin}",
                        id, nomeUsuario, emailUsuario, User.Identity?.Name);

                    TempData["Sucesso"] = "Usuário excluído com sucesso!";
                }
                else
                {
                    _logger.LogError("Erro ao excluir usuário {UserId}: {Errors}", id,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                    TempData["Erro"] = "Erro ao excluir usuário. " + string.Join(" ", result.Errors.Select(e => e.Description));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir usuário {UserId} por {Admin}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao excluir usuário. Tente novamente.";
            }

            return RedirectToAction(nameof(ManageUsers));
        }
    }
}