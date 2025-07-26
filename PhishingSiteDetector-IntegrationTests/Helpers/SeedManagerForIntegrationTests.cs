using Microsoft.AspNetCore.Identity;
using PhishingSiteDetector_API.Database;
using PhishingSiteDetector_API.Models.Constants;
using PhishingSiteDetector_API.Models.Entities;

namespace PhishingSiteDetector_API_IntegrationTests.Helpers
{
    public static class SeedManagerForIntegrationTests
    {
        public static void Seed(ApplicationDbContext context)
        {
            CreateLanguages(context);
            CreateRoles(context);
            CreateUsers(context);
            CreateUsersRoles(context);
        }

        public static void CreateLanguages(ApplicationDbContext context)
        {
            List<Language> languages = [DBLanguage.EN, DBLanguage.PL];

            foreach (var language in languages)
            {
                if (!context.Languages.Any(a => a.Code == language.Code))
                {
                    context.Languages.Add(language);
                }
            }
            context.SaveChanges();
        }

        public static void CreateRoles(ApplicationDbContext context)
        {
            List<IdentityRole> roles = [DBRole.Admin];

            foreach (var role in roles)
            {
                if (!context.Roles.Any(a => a.Id == role.Id))
                {
                    context.Roles.Add(role);
                }
            }
            context.SaveChanges();
        }

        public static void CreateUsers(ApplicationDbContext context)
        {
            var hasher = new PasswordHasher<IdentityUser>();
            var adminUser = DBAdmin.Account;
            adminUser.PasswordHash = hasher.HashPassword(adminUser, DBAdmin.Password);
            List<ApplicationUser> users = [adminUser];

            foreach (var user in users)
            {
                if (!context.Roles.Any(a => a.Id == user.Id))
                {
                    context.Users.Add(user);
                }
            }
            context.SaveChanges();
        }

        public static void CreateUsersRoles(ApplicationDbContext context)
        {
            List<IdentityUserRole<string>> usersRoles = [
                new IdentityUserRole<string> { UserId = DBAdmin.Account.Id, RoleId = DBRole.Admin.Id }
            ];

            foreach (var item in usersRoles)
            {
                if (!context.UserRoles.Any(a => a.UserId == item.UserId && a.RoleId == item.RoleId))
                {
                    context.UserRoles.Add(item);
                }
            }
            context.SaveChanges();
        }
    }
}