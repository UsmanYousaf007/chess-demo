using System.Collections.Generic;
using HUFEXT.CountryCode.Runtime.API;
using HUFEXT.GenericGDPR.Runtime.API;
using UnityEngine;

namespace HUFEXT.GenericGDPR.Runtime.Utils
{
    public static class GDPRTranslationsProvider
    {
        public struct GDPRTranslation
        {
            public string lang;
            public SystemLanguage langType;
            public string header;
            public string policy;
            public string footer;
            public string toggle;
            public string button;
        }

        public const string AD_PARTNERS_LINK = "\"https://huuugegames.com/privacy-policy/#huuuge%27s+partner+list\"";
        public const string PRIVACY_POLICY_LINK = "\"https://huuugegames.com/privacy-policy\"";
        public const string TERMS_OF_USE_LINK = "\"https://www.huuugegames.com/terms-of-use\"";
        
        // First element in list is default translation.
        static readonly List<GDPRTranslation> translations = new List<GDPRTranslation>
        {
            new GDPRTranslation
            {
                lang = "en",
                langType = SystemLanguage.English,
                header = "We care about your privacy",
                policy = "I hereby consent to the usage and disclosure of my personal data (including device information and my preferences) to <link=" + AD_PARTNERS_LINK + "><color=#{0}><u>advertising network companies</u></color></link> for the purpose of serving targeted advertisements to me in the game. I understand that I can withdraw this consent at any time within the game Settings, as also described in our <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>Privacy Policy</u></color></link>.",
                footer = "By clicking Continue, you agree to be bound by our <link=" + TERMS_OF_USE_LINK + "><color=#{0}><b><u>Terms of Use</u></b></color></link> and our <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><b><u>Privacy Policy</u></b></color></link>.",
                toggle = "I Accept",
                button = "Continue"
            },

            new GDPRTranslation
            {
                lang = "pl",
                langType = SystemLanguage.Polish,
                header = "Cenimy twoją prywatność",
                policy = "Wyrażam zgodę na używanie i ujawnianie moich danych osobowych (w tym informacje o urządzeniu i moich preferencjach) <link=" + AD_PARTNERS_LINK + "><color=#{0}><u>sieciom reklamowym</u></color></link> w celu dostarczania mi dopasowanych reklam w grze. Rozumiem, że mogę wycofać swoją zgodę w dowolnej chwili w Ustawieniach gry, jak wskazano też w <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>Polityce Prywatności</u></color></link>.",
                footer = "Klikając w Kontyntynuj, akceptujesz nasze <link=" + TERMS_OF_USE_LINK + "><color=#{0}><b><u>Warunki Korzystania</u></b></color></link> oraz <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><b><u>Politykę Prywatności</u></b></color></link>.",
                toggle = "Wyrażam zgodę",
                button = "Kontyntynuj"
            }
        };
        
        public static GDPRTranslation DefaultTranslation => translations[0];

        public static List<GDPRTranslation> Translations => translations;
        
        public static GDPRTranslation GetTranslation( GDPRConfig config, string forceLang = "" )
        {
            if ( config == null || !config.IsTranslationEnabled )
            {
                return translations[0];
            }

            var forceLanguage = !string.IsNullOrEmpty( forceLang );
            
            // Check by Unity API.
            if ( Application.systemLanguage != SystemLanguage.Unknown && !forceLanguage )
            {
                for ( int i = 0; i < translations.Count; ++i )
                {
                    if ( Application.systemLanguage == translations[i].langType )
                    {
                        return translations[i];
                    }
                }
            }

            var lang = HNativeCountryCode.GetCountryCode().Language;
            var code = HNativeCountryCode.GetCountryCode().GetCountryCode();

            if ( forceLanguage )
            {
                lang = forceLang;
                code = forceLang;
            }
            
            // Check by full country code {xx-yy}.
            for ( int i = 0; i < translations.Count; ++i )
            {
                if ( translations[i].lang == code )
                {
                    return translations[i];
                }
            }
            
            // Check only via language code {xx}.
            for ( int i = 0; i < translations.Count; ++i )
            {
                if ( translations[i].lang == lang || translations[i].lang == code )
                {
                    return translations[i];
                }
            }

            return translations[0];
        }
    }
}