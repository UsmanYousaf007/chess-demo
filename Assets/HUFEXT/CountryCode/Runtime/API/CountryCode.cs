using System.Linq;
using JetBrains.Annotations;

namespace HUFEXT.NativeCountryCode.API
{
    public class CountryCode
    {
        [PublicAPI] public readonly string Language;
        [PublicAPI] public readonly string Country;

        public CountryCode(string language, string country)
        {
            Language = language.ToLower();
            Country = country.ToLower();
        }

        public CountryCode(string countryCode)
        {
            var result = countryCode.Split('-', '_');
            Language = result.FirstOrDefault()?.ToLower();
            Country = result.LastOrDefault()?.ToLower();
        }

        [PublicAPI]
        public string GetCountryCode()
        {
            return $"{Language}-{Country}";
        }
    }
}