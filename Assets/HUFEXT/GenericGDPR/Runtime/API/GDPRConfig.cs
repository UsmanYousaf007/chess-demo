using System;
using HUF.Utils.Runtime.Configs.API;
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
    public class GDPRConfig : FeatureConfigBase
    {
        [Header( "Common" )]
        [SerializeField] GameObject prefab = null;
        [SerializeField] 
        [FormerlySerializedAs("customPrefsKey")]
        string gdprCustomPlayerPrefsKey = string.Empty;
        [SerializeField] string personalizedAdsCustomPlayerPrefsKey = string.Empty;
        [SerializeField] bool destroyOnAccept = true;

        [Header( "Translations" )] 
        [SerializeField] bool enableTranslation = true;
        
        public GameObject Prefab => prefab;
        public string CustomGDPRKey => gdprCustomPlayerPrefsKey;
        public string CustomPersonalizedAdsKey => personalizedAdsCustomPlayerPrefsKey;
        public bool DestroyOnAccept => destroyOnAccept;
        public bool IsTranslationEnabled => enableTranslation;
        
        public override void RegisterManualInitializers()
        {
            AddManualInitializer( "GenericGDPR", HGenericGDPR.Initialize );
        }
    }
}