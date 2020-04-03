using HUFEXT.NativeCountryCode.API;

namespace HUFEXT.NativeCountryCode.Implementation
{
    public interface ICountryCodeProvider
    {
        CountryCode GetCountryCode();
    }
}