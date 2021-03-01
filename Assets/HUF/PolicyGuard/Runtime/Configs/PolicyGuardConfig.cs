using HUF.GenericDialog.Runtime.Configs;
using HUF.PolicyGuard.Runtime.API;
using HUF.Utils.Runtime.Configs.API;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace HUF.PolicyGuard.Runtime.Configs
{

    [CreateAssetMenu( menuName = "HUF/PolicyGuard/" + nameof(PolicyGuardConfig), fileName = nameof(PolicyGuardConfig) )]
    public class PolicyGuardConfig : FeatureConfigBase
    {
        [SerializeField] bool useAutomatedFlow = true;
        [SerializeField] bool showATTPreOptInPopup = false;
        [SerializeField] bool showNativeATT = true;
        [SerializeField] bool showAdsPrivacyConsentInGDPRPopup = true;
        [FormerlySerializedAs( "showAdsConsentAfterGDPR" )] [SerializeField] bool showAdsConsent = false;

        [SerializeField] HGenericDialogConfig referenceToATTPreOptInPopup;
        [SerializeField] HGenericDialogConfig referenceToGDPRWithAdsPopup;
        [SerializeField] HGenericDialogConfig referenceToGDPRPopup;
        [SerializeField] HGenericDialogConfig referenceToPersonalizedAdsPopup;

        [Header( "Country Check" )]
        [SerializeField] bool enableCountryCheck = false;
        [SerializeField] string showForCountries =
            "AT,BE,BG,HR,CY,CZ,DK,EE,FI,FR,DE,GR,HU,IE,IT,LV,LT,LU,MT,NL,PT,RO,SK,SI,ES,SE,GB,GF,GP,MQ,ME,YT,RE,MF,GI,AX,PM,GL,BL,SX,AW,CW,WF,PF,NC,TF,AI,BM,IO,VG,KY,FK,MS,PN,SH,GS,TC,AD,LI,MC,SM,VA,JE,GG,GI,CH,PL";

        [PublicAPI] public bool UseAutomatedFlow => useAutomatedFlow;
        [PublicAPI] public bool ShowATTPreOptInPopup { get { return showATTPreOptInPopup; }  set { showATTPreOptInPopup = value; } }
        [PublicAPI] public bool ShowNativeATT { get { return showNativeATT; }  set { showNativeATT = value; } }
        [PublicAPI] public bool ShowAdsPrivacyConsentInGDPRPopup => showAdsPrivacyConsentInGDPRPopup;
        [PublicAPI] public bool ShowAdsConsent { get { return showAdsConsent; } set { showAdsConsent = value; } }

        [PublicAPI] public HGenericDialogConfig ReferenceToATTPreOptInPopup => referenceToATTPreOptInPopup;
        [PublicAPI] public HGenericDialogConfig ReferenceToGDPRWithAdsPopup => referenceToGDPRWithAdsPopup;
        [PublicAPI] public HGenericDialogConfig ReferenceToGDPRPopup => referenceToGDPRPopup;
        [PublicAPI] public HGenericDialogConfig ReferenceToPersonalizedAdsPopup => referenceToPersonalizedAdsPopup;

        public bool EnableCountryCheck => enableCountryCheck;
        public string ShowForCountries => showForCountries;

        public override void RegisterManualInitializers()
        {
            AddManualInitializer( "Policy Guard", HPolicyGuard.Initialize );
        }
    }
}
