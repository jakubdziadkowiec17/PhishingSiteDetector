using Microsoft.AspNetCore.Identity;
using PhishingSiteDetector_API.Constants;

namespace PhishingSiteDetector_API.Models.Constants
{
    public static class DBRole
    {
        public static readonly IdentityRole Admin = new IdentityRole { Id = "11111111-1111-1111-1111-111111111111", Name = Role.Admin, NormalizedName = Role.Admin.ToUpper() };
    }
}