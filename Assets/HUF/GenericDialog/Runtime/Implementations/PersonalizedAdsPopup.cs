using HUF.Ads.Runtime.API;
using HUF.GenericDialog.Runtime.API;
using HUF.GenericDialog.Runtime.Configs;
using HUF.Utils.Runtime.Logging;
using UnityEngine;
using UnityEngine.UI;

namespace HUF.GenericDialog.Runtime.Implementations
{
    public class PersonalizedAdsPopup : HGenericDialogInstance
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(PersonalizedAdsPopup) );

        HGenericDialogConfig config;
        [SerializeField] Toggle adsConsentToggle;

        protected override HLogPrefix LogPrefix => logPrefix;

        protected override void HandleInitialization( HGenericDialogConfig config )
        {
            this.config = config;
#if HUF_ADS
            bool? consent = HAds.HasPersonalizedAdConsent();

            adsConsentToggle.isOn = consent.HasValue && consent.Value;
#endif
        }

        protected override void HandlePrimaryButtonClick()
        {
#if HUF_ADS
            HAds.CollectSensitiveData( adsConsentToggle.isOn );
#endif
            HGenericDialog.SetAsHandled( config );
            OnClosePopup.Invoke();
        }

        protected override void HandleSecondaryButtonClick()
        {
            Postpone();

            OnClosePopup.Invoke();
        }
    }
}