using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PhishingSiteDetector_API.Models.Constants;
using PhishingSiteDetector_API.Models.Entities;

namespace PhishingSiteDetector_API.Database
{
    public static class SeedManager
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await CreateLanguages(context);
            await CreateRoles(roleManager);
            await CreateAdminUser(userManager);
        }

        public static async Task CreateLanguages(ApplicationDbContext context)
        {
            List<Language> languages = [DBLanguage.EN, DBLanguage.PL];

            foreach (var language in languages)
            {
                if (!await context.Languages.AnyAsync(a => a.Code == language.Code))
                {
                    await context.Languages.AddAsync(language);
                }
            }
            await context.SaveChangesAsync();
        }

        public static async Task CreateRoles(RoleManager<IdentityRole> roleManager)
        {
            List<IdentityRole> roles = [DBRole.Admin];

            foreach (var role in roles)
            {
                if (await roleManager.FindByIdAsync(role.Id) is null)
                {
                    await roleManager.CreateAsync(role);
                }
            }
        }

        public static async Task CreateAdminUser(UserManager<ApplicationUser> userManager)
        {
            if (await userManager.FindByIdAsync(DBAdmin.Account.Id) is null)
            {
                await userManager.CreateAsync(DBAdmin.Account, DBAdmin.Password);
                await userManager.AddToRoleAsync(DBAdmin.Account, DBRole.Admin.Name);
            }
        }
    }
}