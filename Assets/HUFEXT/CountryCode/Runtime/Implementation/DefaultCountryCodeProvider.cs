using HUFEXT.NativeCountryCode.API;

namespace HUFEXT.NativeCountryCode.Implementation
{
    public class DefaultCountryCodeProvider : ICountryCodeProvider
    {
        public CountryCode GetCountryCode()
        {
            return new CountryCode("en", "US");
        }
    }
}