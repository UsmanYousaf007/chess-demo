#if UNITY_ANDROID
using HUFEXT.NativeCountryCode.API;
using UnityEngine;

namespace HUFEXT.NativeCountryCode.Implementation
{
    public class AndroidCountryCodeProvider : ICountryCodeProvider
    {
        public CountryCode GetCountryCode()
        {
            return ReadCountryCodeFromLocale();
        }

        CountryCode ReadCountryCodeFromLocale()
        {
            using (var localeObject = new AndroidJavaClass("java.util.Locale"))
            {
                using(var localeInstance = localeObject.CallStatic<AndroidJavaObject>("getDefault"))
                {
                    if (localeInstance != null)
                    {
                        var language = localeInstance.Call<string>("getLanguage");
                        var country = localeInstance.Call<string>("getCountry");
                        return new CountryCode(language, country);
                    }
                }
            }

            return null;
        }
    }
}
#endif