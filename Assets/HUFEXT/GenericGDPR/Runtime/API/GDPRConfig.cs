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
        
        [Header( "Country Check" )]
        [SerializeField] bool enableCountryCheck = false;
        [SerializeField] bool defaultPersonalizedAdsValue = false;
        [SerializeField] string showForCountries = "AT,BE,BG,HR,CY,CZ,DK,EE,FI,FR,DE,GR,HU,IE,IT,LV,LT,LU,MT,NL,PT,RO,SK,SI,ES,SE,GB,GF,GP,MQ,ME,YT,RE,MF,GI,AX,PM,GL,BL,SX,AW,CW,WF,PF,NC,TF,AI,BM,IO,VG,KY,FK,MS,PN,SH,GS,TC,AD,LI,MC,SM,VA,JE,GG,GI,CH,PL";
        
        [Header( "Translations" )]
        [SerializeField] bool enableTranslation = true;

        public GameObject Prefab => prefab;
        public string CustomGDPRKey => gdprCustomPlayerPrefsKey;
        public string CustomPersonalizedAdsKey => personalizedAdsCustomPlayerPrefsKey;
        public bool DestroyOnAccept => destroyOnAccept;
        public bool IsTranslationEnabled => enableTranslation;
        public bool EnableCountryCheck => enableCountryCheck;
        public bool DefaultPersonalizedAdsValue => defaultPersonalizedAdsValue;
        public string ShowForCountries => showForCountries;

        public override void RegisterManualInitializers()
        {
            AddManualInitializer( "GenericGDPR", ()=>
            {
                if ( !HGenericGDPR.IsPolicyAccepted )
                    HGenericGDPR.Initialize();
            } );
        }
    }
}