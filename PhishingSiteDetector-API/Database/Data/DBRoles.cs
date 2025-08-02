using Microsoft.AspNetCore.Identity;
using PhishingSiteDetector_API.Constants;

namespace PhishingSiteDetector_API.Models.Constants
{
    public static class DBRoles
    {
        public static readonly IdentityRole Admin = new()
        {
            Id = "11111111-1111-1111-1111-111111111111",
            Name = Role.Admin,
            NormalizedName = Role.Admin.ToUpper()
        };

        public static readonly List<IdentityRole> All = new()
        {
            Admin
        };
    }
}