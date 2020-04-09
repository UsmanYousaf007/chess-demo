#if UNITY_IPHONE
using System.Runtime.InteropServices;
using HUFEXT.NativeCountryCode.API;

namespace HUFEXT.NativeCountryCode.Implementation
{
    public class iOSCountryCodeProvider : ICountryCodeProvider
    {
        [DllImport("__Internal")]
        static extern string GetCountryCodeNative();
        
        public CountryCode GetCountryCode()
        {
            var result = GetCountryCodeNative();
            return new CountryCode(result); 
        }
    }
}
#endif