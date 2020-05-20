#if UNITY_IPHONE
using System.Runtime.InteropServices;

namespace HUFEXT.CountryCode.Runtime.Implementation
{
    public class iOSCountryCodeProvider : ICountryCodeProvider
    {
        [DllImport("__Internal")]
        static extern string GetCountryCodeNative();
        
        public CountryCode.Runtime.API.CountryCode GetCountryCode()
        {
            var result = GetCountryCodeNative();
            return new CountryCode.Runtime.API.CountryCode(result); 
        }
    }
}
#endif