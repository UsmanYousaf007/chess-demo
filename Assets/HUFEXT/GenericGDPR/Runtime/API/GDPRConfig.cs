using System;
using System.Collections.Generic;
using HUF.Utils.Configs.API;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace HUFEXT.GenericGDPR.Runtime.API
{
    [Serializable]
    public class TranslationFont
    {
        public string key;
        public TMP_FontAsset font;
    }
    
    [CreateAssetMenu(menuName = "HUFEXT/GDPR/GDPR Config", fileName = "GDPRConfig.asset")]
    public class GDPRConfig : AbstractConfig
    {
        [Header( "Common" )]
        [SerializeField] GameObject prefab = null;
        [SerializeField] 
        [FormerlySerializedAs("customPrefsKey")]
        string gdprCustomPlayerPrefsKey = string.Empty;
        [SerializeField] string personalizedAdsCustomPlayerPrefsKey = string.Empty;
        [SerializeField] bool autoInit = true;
        [SerializeField] bool destroyOnAccept = true;
        [SerializeField] TMP_FontAsset defaultFont;

        [Header( "Translations" )] 
        [SerializeField] bool enableTranslation = true;
        [SerializeField] List<TranslationFont> fonts = new List<TranslationFont>();
        
        public GameObject Prefab => prefab;
        public string CustomGDPRKey => gdprCustomPlayerPrefsKey;
        public string CustomPersonalizedAdsKey => personalizedAdsCustomPlayerPrefsKey;
        public bool AutoInit => autoInit;
        public bool DestroyOnAccept => destroyOnAccept;
        public TMP_FontAsset DefaultFont => defaultFont;
        public bool IsTranslationEnabled => enableTranslation;
        public List<TranslationFont> Fonts => fonts;
    }
}