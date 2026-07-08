using Microsoft.AspNetCore.Identity;
using RentalTourismSystem.Models;

namespace RentalTourismSystem.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(
            IServiceProvider serviceProvider,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger logger)
        {
            // Criar roles
            string[] roleNames = { "Admin", "Manager", "Employee" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    var role = new IdentityRole(roleName);
                    await roleManager.CreateAsync(role);
                    logger.LogInformation("Role {Role} criada", roleName);
                }
            }

            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var adminEmail = configuration["BootstrapAdmin:Email"];
            var adminPassword = configuration["BootstrapAdmin:Password"];
            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
            {
                logger.LogWarning("Bootstrap do administrador ignorado: configure BootstrapAdmin__Email e BootstrapAdmin__Password");
                return;
            }
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    NomeCompleto = "Administrador do Sistema",
                    Cargo = "Administrador",
                    EmailConfirmed = true,
                    Ativo = true,
                    DataCadastro = DateTime.Now
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    logger.LogInformation("Usuário admin criado: {Email}", adminEmail);
                }
                else
                {
                    logger.LogError("Erro ao criar usuário admin: {Errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
