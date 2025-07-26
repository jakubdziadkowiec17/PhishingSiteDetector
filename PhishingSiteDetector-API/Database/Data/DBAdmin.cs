using PhishingSiteDetector_API.Constants;
using PhishingSiteDetector_API.Models.Entities;

namespace PhishingSiteDetector_API.Models.Constants
{
    public static class DBAdmin
    {
        public static readonly ApplicationUser Account = new ApplicationUser
        {
            Id = "1111-1111-1111-1111-111111111111",
            Email = "Admin@Admin.pl",
            NormalizedEmail = "ADMIN@ADMIN.PL",
            UserName = "Admin@Admin.pl",
            NormalizedUserName = "ADMIN@ADMIN.PL",
            PhoneNumberConfirmed = true,
            FirstName = "Application",
            LastName = "Admin",
            DateOfBirth = new DateOnly(2000,01,01),
            Address = "Poland, Cracow 12345678",
            AreaCode = 48,
            PhoneNumber = 100000000,
            LanguageCode = LanguageCode.EN
        };
        public static readonly string Password = "Admin11!";
    }
}