using PhishingSiteDetector_API.Constants;
using PhishingSiteDetector_API.Models.Entities;

namespace Soccerity_API_IntegrationTests.Data.Database
{
    public static class DBSecondAdmin
    {
        public static readonly ApplicationUser Account = new ApplicationUser
        {
            Id = "2222-2222-2222-2222-222222222222",
            Email = "SecondAdmin@SecondAdmin.pl",
            NormalizedEmail = "SECONDADMIN@SECONDADMIN.PL",
            UserName = "SecondAdmin@SecondAdmin.pl",
            NormalizedUserName = "SECONDADMIN@SECONDADMIN.PL",
            PhoneNumberConfirmed = true,
            FirstName = "Second Application",
            LastName = "Admin",
            DateOfBirth = new DateOnly(2000, 01, 02),
            Address = "Poland, Cracow 12345678",
            AreaCode = 48,
            PhoneNumber = 100000001,
            LanguageCode = LanguageCode.EN
        };
        public static readonly string Password = "Admin22!";
    }
}