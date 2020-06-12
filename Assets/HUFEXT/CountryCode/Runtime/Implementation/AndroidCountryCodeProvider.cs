#if UNITY_ANDROID
using UnityEngine;

namespace HUFEXT.CountryCode.Runtime.Implementation
{
    public class AndroidCountryCodeProvider : ICountryCodeProvider
    {
        public API.CountryCode GetCountryCode()
        {
            return ReadCountryCodeFromLocale();
        }

        API.CountryCode ReadCountryCodeFromLocale()
        {
            using (var localeObject = new AndroidJavaClass("java.util.Locale"))
            {
                using(var localeInstance = localeObject.CallStatic<AndroidJavaObject>("getDefault"))
                {
                    if (localeInstance != null)
                    {
                        var language = localeInstance.Call<string>("getLanguage");
                        var country = localeInstance.Call<string>("getCountry");
                        return new API.CountryCode(language, country);
                    }
                }
            }

            return null;
        }
    }
}
#endif