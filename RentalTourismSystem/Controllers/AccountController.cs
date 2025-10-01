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
    }
}