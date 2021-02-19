#if HUF_TRANSLATION_SYSTEM
using HUF.TranslationSystem.Runtime.API;
#elif HUFEXT_COUNTRY_CODE
using HUFEXT.CountryCode.Runtime.API;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HUF.Utils.Runtime.Translation
{
    [Serializable]
    public class MultiLanguageText : ISerializationCallbackReceiver
    {
        [SerializeField] TextPair[] textPairs = default;

        Dictionary<string, string> dictionary;

        public string GetText()
        {
            var languageCode = "en";
#if HUF_TRANSLATION_SYSTEM
            languageCode = HTranslator.CurrentLanguage;
#elif HUFEXT_COUNTRY_CODE
            languageCode = HNativeCountryCode.GetCountryCode().Language;
#endif
            if ( dictionary.Count == 0 )
            {
                return string.Empty;
            }

            bool fetched = dictionary.TryGetValue( languageCode, out string translatedMessage );
#if HUF_TRANSLATION_SYSTEM
            if ( fetched )
                return translatedMessage;

            if ( dictionary.ContainsKey( HTranslator.DEFAULT ) )
                return dictionary[HTranslator.DEFAULT];
#endif
            return fetched ? translatedMessage : dictionary.FirstOrDefault().Value;
        }

        public void OnAfterDeserialize()
        {
            if (textPairs == null)
                return;

            dictionary = new Dictionary<string, string>();

            for ( var i = 0; i < textPairs.Length; i++ )
            {
                dictionary.Add( textPairs[i].Language, textPairs[i].Text );
            }
        }

        public void OnBeforeSerialize() { }

        public bool IsEmpty => textPairs.Length == 0;
    }
}