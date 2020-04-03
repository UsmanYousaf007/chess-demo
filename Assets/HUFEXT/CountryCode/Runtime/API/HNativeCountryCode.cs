using HUFEXT.NativeCountryCode.Implementation;
using JetBrains.Annotations;
using UnityEngine;

#if UNITY_ANDROID && !UNITY_EDITOR
using PlatformCountryCodeProvider = HUFEXT.NativeCountryCode.Implementation.AndroidCountryCodeProvider;
#elif UNITY_IPHONE && !UNITY_EDITOR
using PlatformCountryCodeProvider = HUFEXT.NativeCountryCode.Implementation.iOSCountryCodeProvider;
#else
using PlatformCountryCodeProvider = HUFEXT.NativeCountryCode.Implementation.DefaultCountryCodeProvider;
#endif

namespace HUFEXT.NativeCountryCode.API
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
