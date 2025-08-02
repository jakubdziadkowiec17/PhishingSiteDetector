using PhishingSiteDetector_API.Constants;
using PhishingSiteDetector_API.Models.Entities;

namespace PhishingSiteDetector_API.Models.Constants
{
    public static class DBLanguages
    {
        public static readonly Language EN = new Language { Code = LanguageCode.EN };
        public static readonly Language PL = new Language { Code = LanguageCode.PL };

        public static readonly List<Language> All = new()
        {
            EN,
            PL
        };
    }
}