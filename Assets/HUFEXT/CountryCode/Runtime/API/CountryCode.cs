using JetBrains.Annotations;

namespace HUFEXT.CountryCode.Runtime.API
{
    public class CountryCode
    {
        /// <summary>
        /// System language code (ISO 639-1)
        /// </summary>
        [PublicAPI]
        public readonly string Language;

        /// <summary>
        /// System set country code (ISO 3166-1)
        /// </summary>
        [PublicAPI]
        public readonly string Country;

        /// <summary>
        /// IOS only region descriptor.
        /// </summary>
        /// <example>
        /// <para>Assume IOS returns zh-Hans_HK as the language used.</para>
        /// <para>It will be split to: Language: zh, Country: Hans, Region: HK.</para>
        /// </example>
        [PublicAPI]
        public readonly string Region;

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

        /// <summary>
        /// Gets formatted country code.
        /// </summary>
        /// <returns>Country code formatted as $"{<see cref="Language"/>}-{<see cref="Country"/>}".</returns>
        [PublicAPI]
        public string GetCountryCode()
        {
            return $"{Language}-{Country}";
        }

        /// <summary>
        /// Converts localization details to dash separated format.
        /// </summary>
        /// <returns>Country code formatted as $"{<see cref="Language"/>}-{<see cref="Country"/>}".</returns>
        [PublicAPI]
        public override string ToString()
        {
            return GetCountryCode().ToLower().TrimEnd( '-' );
        }
    }
}