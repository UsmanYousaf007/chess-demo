namespace HUFEXT.CountryCode.Runtime.Implementation
{
    public class DefaultCountryCodeProvider : ICountryCodeProvider
    {
        public API.CountryCode GetCountryCode()
        {
            return new API.CountryCode("en", "US");
        }
    }
}