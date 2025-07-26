using System.Text.RegularExpressions;

namespace PhishingSiteDetector_API.Constants
{
    public static class RegularExpression
    {
        public static Regex Password = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
    }
}