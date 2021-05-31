using System.Collections.Generic;
using HUF.GenericDialog.Runtime.API;
using HUF.GenericDialog.Runtime.Configs;
using HUF.PolicyGuard.Runtime.API;
using HUF.PolicyGuard.Runtime.Configs.Data;
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
        [SerializeField] bool delayClosingATTPreOptInPopup = true;
        [SerializeField] bool showNativeATT = true;
        [SerializeField] bool showAdsPrivacyConsentInGDPRPopup = true;

        [FormerlySerializedAs( "showAdsConsentAfterGDPR" )]

        [SerializeField] bool showAdsConsent = false;
        [SerializeField] HGenericDialogConfig referenceToATTPreOptInPopup;
        [SerializeField] HGenericDialogConfig referenceToGDPRWithAdsPopup;
        [SerializeField] HGenericDialogConfig referenceToGDPRPopup;
        [SerializeField] HGenericDialogConfig referenceToPersonalizedAdsPopup;

        [Header( "Prefabs override" )]

        [SerializeField] HGenericDialogInstance overrideATTPreOptInPopup;
        [SerializeField] HGenericDialogInstance overrideGDPRWithAdsPopup;
        [SerializeField] HGenericDialogInstance overrideGDPRPopup;
        [SerializeField] HGenericDialogInstance overridePersonalizedAdsPopup;
        [SerializeField] public GameObject anonymizationBlocker;

        [Header( "Country Check" )]

        [SerializeField] bool enableCountryCheck = false;
        [SerializeField] string showForCountries =
            "AT,BE,BG,HR,CY,CZ,DK,EE,FI,FR,DE,GR,HU,IE,IT,LV,LT,LU,MT,NL,PT,RO,SK,SI,ES,SE,GB,GF,GP,MQ,ME,YT,RE,MF,GI,AX,PM,GL,BL,SX,AW,CW,WF,PF,NC,TF,AI,BM,IO,VG,KY,FK,MS,PN,SH,GS,TC,AD,LI,MC,SM,VA,JE,GG,GI,CH,PL";

        [Header( "Other" )]
        [SerializeField] bool createUnityUIEventSystemIfNotExists = false;

        [Header( "Platforms flow" )]
        [SerializeField] bool useCustomAutomatedPopupsFlow;
        [SerializeField] List<PolicyPopup> androidPopupsFlow;
        [SerializeField] List<PolicyPopup> iOSPopupsFlow;

        [PublicAPI] public bool UseAutomatedFlow => useAutomatedFlow;
        [PublicAPI] public bool ShowATTPreOptInPopup { get { return showATTPreOptInPopup; }  set { showATTPreOptInPopup = value; } }
        [PublicAPI] public bool ShowNativeATT { get { return showNativeATT; }  set { showNativeATT = value; } }
        [PublicAPI] public bool DelayClosingATTPreOptInPopup => delayClosingATTPreOptInPopup;
        [PublicAPI] public bool ShowAdsPrivacyConsentInGDPRPopup => showAdsPrivacyConsentInGDPRPopup;
        [PublicAPI] public bool ShowAdsConsent { get { return showAdsConsent; } set { showAdsConsent = value; } }

        [PublicAPI] public HGenericDialogConfig ReferenceToATTPreOptInPopup => referenceToATTPreOptInPopup;
        [PublicAPI] public HGenericDialogConfig ReferenceToGDPRWithAdsPopup => referenceToGDPRWithAdsPopup;
        [PublicAPI] public HGenericDialogConfig ReferenceToGDPRPopup => referenceToGDPRPopup;
        [PublicAPI] public HGenericDialogConfig ReferenceToPersonalizedAdsPopup => referenceToPersonalizedAdsPopup;

        [PublicAPI] public HGenericDialogInstance OverrideATTPreOptInPopup => overrideATTPreOptInPopup;
        [PublicAPI] public HGenericDialogInstance OverrideGDPRWithAdsPopup => overrideGDPRWithAdsPopup;
        [PublicAPI] public HGenericDialogInstance OverrideGDPRPopup => overrideGDPRPopup;
        [PublicAPI] public HGenericDialogInstance OverridePersonalizedAdsPopup => overridePersonalizedAdsPopup;

        [PublicAPI] public bool EnableCountryCheck => enableCountryCheck;
        [PublicAPI] public string ShowForCountries => showForCountries;

        [PublicAPI] public bool CreateUnityUIEventSystemIfNotExists => createUnityUIEventSystemIfNotExists;

        [PublicAPI]
        public bool UseCustomAutomatedPopupsFlow
        {
#if UNITY_EDITOR
            set => useCustomAutomatedPopupsFlow = value;
#endif
            get => useCustomAutomatedPopupsFlow;
        }

        [PublicAPI]
        public List<PolicyPopup> PopupsFlow
        {
            get
            {
#if UNITY_IOS
                return iOSPopupsFlow;
#else
                return androidPopupsFlow;
#endif
            }
        }

#if UNITY_EDITOR
        [PublicAPI] public List<PolicyPopup> AndroidPopupsFlow => androidPopupsFlow;
        [PublicAPI] public List<PolicyPopup> IOSPopupsFlow => iOSPopupsFlow;
#endif

        public override void RegisterManualInitializers()
        {
            AddManualInitializer( "Policy Guard", HPolicyGuard.Initialize );
        }
    }
}
