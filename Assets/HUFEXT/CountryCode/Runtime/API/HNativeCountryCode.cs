#if UNITY_ANDROID && !UNITY_EDITOR
using PlatformCountryCodeProvider = HUFEXT.CountryCode.Runtime.Implementation.AndroidCountryCodeProvider;
#elif UNITY_IPHONE && !UNITY_EDITOR
using PlatformCountryCodeProvider = HUFEXT.CountryCode.Runtime.Implementation.iOSCountryCodeProvider;
#else
using PlatformCountryCodeProvider = HUFEXT.CountryCode.Runtime.Implementation.DefaultCountryCodeProvider;
#endif
using HUFEXT.CountryCode.Runtime.Implementation;
using JetBrains.Annotations;

namespace HUFEXT.CountryCode.Runtime.API
{
    public static class HNativeCountryCode
    {
        static readonly ICountryCodeProvider countryCodeProvider;
        
        static HNativeCountryCode()
        {
            countryCodeProvider = new PlatformCountryCodeProvider();
        }
        
        [PublicAPI]
        public static CountryCode GetCountryCode()
        {
            return countryCodeProvider.GetCountryCode();
        }
    }
}
