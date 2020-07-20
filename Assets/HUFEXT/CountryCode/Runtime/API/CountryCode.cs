using JetBrains.Annotations;

namespace HUFEXT.CountryCode.Runtime.API
{
    public class CountryCode
    {
        [PublicAPI] public readonly string Language;
        [PublicAPI] public readonly string Country;
        [PublicAPI] public readonly string Region;

        public CountryCode(string language, string country, string region = "")
        {
            Language = language.ToLower();
            Country = country.ToLower();
            Region = region.ToLower();
        }

        public CountryCode(string countryCode)
        {
            var result = countryCode.Split('-', '_');

            switch ( result.Length )
            {
                case 1: 
                    Language = result[0].ToLower();
                    break;
                
                case 2: 
                    Language = result[0].ToLower();
                    Country = result[1].ToLower();
                    break;
                
                case 3: 
                    Language = result[0].ToLower();
                    Country = result[1].ToLower();
                    Region = result[2].ToLower();
                    break;
                
                default: 
                    Language = "en";
                    Country = "us";
                    Region = string.Empty;
                    break;
            }
        }

        [PublicAPI]
        public string GetCountryCode()
        {
            return $"{Language}-{Country}";
        }
    }
}