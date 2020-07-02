#if UNITY_IPHONE
using System.Runtime.InteropServices;
using UnityEngine;

namespace HUFEXT.CountryCode.Runtime.Implementation
{
    public class iOSCountryCodeProvider : ICountryCodeProvider
    {
        [DllImport("__Internal")]
        static extern string GetCountryCodeNative();
        
        [DllImport("__Internal")]
        static extern string GetRegionNative();
        
        public CountryCode.Runtime.API.CountryCode GetCountryCode()
        {
            var result = GetCountryCodeNative();
            var region = GetRegionNative();
            return new CountryCode.Runtime.API.CountryCode( result + "-" + region );
        }
    }
}
#endif