using System;
using System.Collections;
using HUF.Ads.Runtime.API;
using HUF.Analytics.Runtime.API;
using HUF.GenericDialog.Runtime.API;
using HUF.GenericDialog.Runtime.Configs;
using HUF.PolicyGuard.Runtime.API;
using HUF.PolicyGuard.Runtime.Configs;
using HUF.Utils.Runtime;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using HUF.Utils.Runtime.PlayerPrefs;
using UnityEngine;
#if HUFEXT_COUNTRY_CODE
using HUFEXT.CountryCode.Runtime.API;

#endif

namespace HUF.PolicyGuard.Runtime.Implementations
{
    public class PolicyGuardService
    {
        const string ATT_POSTPONED_KEY = "HUF_ATT_POSTPONED";
        const float SHOW_PREFAB_DELAY = 0.2f;

        readonly WaitForSecondsRealtime waitTime = new WaitForSecondsRealtime( SHOW_PREFAB_DELAY );
        readonly HLogPrefix logPrefix = new HLogPrefix( HPolicyGuard.logPrefix, nameof(PolicyGuardService) );

        public event Action OnGDPRPopupShowed;
        public event Action<bool> OnGDPRPopupCloses;
        public event Action OnPersonalizedAdsPopupShowed;
        public event Action<bool> OnPersonalizedAdsPopupCloses;
        public event Action OnATTPopupShowed;
        public event Action<bool> OnATTPopupCloses;
        public event Action OnATTNativePopupShowed;
        public event Action OnEndCheckingPolicy;

#if UNITY_IOS
        public event Action<AppTrackingTransparencyBridge.AuthorizationStatus> OnATTNativePopupClose;
#endif

        PolicyGuardConfig config;

        public PolicyGuardService()
        {
            config = HConfigs.GetConfig<PolicyGuardConfig>();
#if UNITY_IOS && !UNITY_EDITOR
            // Refresh ATT status on game start
            if ( HPolicyGuard.WasATTPopupDisplayed() )
                AppTrackingTransparencyBridge.CheckAuthorizationStatus( );
#endif
            CheckFlow();
        }

        public bool TryShowATT()
        {
#if UNITY_IOS
            if ( HPolicyGuard.WasATTPopupDisplayed() )
                return false;

            if ( config.ShowATTPreOptInPopup )
            {
                ShowATTPopup();
                return true;
            }

            ShowNativeATTPopup();
            return true;
#else
            return false;
#endif
        }

        public bool TryShowGDPR()
        {
#if HUFEXT_COUNTRY_CODE
            if ( config.EnableCountryCheck && !IsGDPRRequiredForCurrentCountry() )
            {
                HLog.LogImportant( logPrefix, "GDPR is not required for current country." );
                HAnalytics.CollectSensitiveData( true );
                OnGDPRPopupCloses.Dispatch( true );
                return false;
            }
#endif
            if ( HAnalytics.GetGDPRConsent() != null )
                return false;

            OnGDPRPopupShowed.Dispatch();
            ShowGDPRPopup();
            return true;
        }

        public bool TryShowPersonalizedAdsPopup()
        {
            if ( !HPolicyGuard.IsATTAuthorized() )
            {
                return false;
            }

            OnPersonalizedAdsPopupShowed.Dispatch();

            CoroutineManager.StartCoroutine( ShowDialogWithDelay( config.ReferenceToPersonalizedAdsPopup,
                delegate( bool consent )
                {
                    HAds.CollectSensitiveData( consent );
                    OnPersonalizedAdsPopupCloses.Dispatch( consent );
                    CheckFlow();
                } ) );
            return true;
        }

#if UNITY_IOS
        void ShowATTPopup()
        {
            CoroutineManager.StartCoroutine( ShowDialogWithDelay( config.ReferenceToATTPreOptInPopup,
                HandleATTPopupClose ) );
            OnATTPopupShowed.Dispatch();
        }

        void HandleATTPopupClose( bool consent )
        {
            OnATTPopupCloses.Dispatch( consent );

            if ( consent )
                ShowNativeATTPopup();
            else
            {
                HPlayerPrefs.SetBool( ATT_POSTPONED_KEY, true );
                CheckFlow();
            }
        }

        void ShowNativeATTPopup()
        {
            AppTrackingTransparencyBridge.CheckAuthorizationStatus( HandleATTStatus );
            OnATTNativePopupShowed.Dispatch();
        }

        void HandleATTStatus( AppTrackingTransparencyBridge.AuthorizationStatus status )
        {
            OnATTNativePopupClose.Dispatch( status );
            CheckFlow();
        }
#endif

#if HUFEXT_COUNTRY_CODE
        bool IsGDPRRequiredForCurrentCountry()
        {
            string country = HNativeCountryCode.GetCountryCode().Country.ToUpper();
            return config.ShowForCountries.Contains( country );
        }
#endif

        void CheckFlow()
        {
            if ( !config.UseAutomatedFlow )
                return;

            if ( TryShowGDPR() )
            {
                return;
            }

            if ( !HPlayerPrefs.GetBool( ATT_POSTPONED_KEY ) && config.ShowNativeATT && TryShowATT() )
            {
                return;
            }

            if ( config.ShowAdsConsent && HAds.HasConsent() == null && TryShowPersonalizedAdsPopup() )
            {
                return;
            }

            OnEndCheckingPolicy.Dispatch();
        }

        void ShowGDPRPopup()
        {
            if ( !HPolicyGuard.IsATTAuthorized() || !config.ShowAdsPrivacyConsentInGDPRPopup )
            {
                CoroutineManager.StartCoroutine( ShowDialogWithDelay( config.ReferenceToGDPRPopup, HandleGDPRPopupClose ) );
            }
            else
            {
                CoroutineManager.StartCoroutine( ShowGDPRAdsWithDelay() );
            }
        }

        IEnumerator ShowGDPRAdsWithDelay()
        {
            yield return waitTime;

            ShowGDPRWithAds();
        }

        void ShowGDPRWithAds()
        {
            OnPersonalizedAdsPopupShowed.Dispatch();

            if ( !HGenericDialog.ShowDialog( config.ReferenceToGDPRWithAdsPopup, out var instance ) )
                return;

            var popup = (GDPRWithAdsDialog)instance;
            popup.OnPrimaryButtonClicked += () => HandleGDPRPopupClose( true );

            popup.OnAdsConsentSet += consent =>
            {
                HAds.CollectSensitiveData( consent );
                OnPersonalizedAdsPopupCloses.Dispatch( consent );
            };
        }

        void HandleGDPRPopupClose( bool consent )
        {
            HAnalytics.CollectSensitiveData( consent );
            OnGDPRPopupCloses.Dispatch( consent );
            CheckFlow();
        }

        void ShowGenericDialog( HGenericDialogConfig config, Action<bool> response )
        {
            if ( !HGenericDialog.ShowDialog( config, out var instance ) )
            {
                HLog.LogError( logPrefix,
                    $"Generic dialog creation error for {( config == null ? "Null config" : config.ConfigId )}" );
            }

            var popup = (GenericEventDialog)instance;
            popup.OnPrimaryButtonClicked += () => { response.Dispatch( true ); };
            popup.OnSecondaryButtonClicked += () => { response.Dispatch( false ); };
        }

        IEnumerator ShowDialogWithDelay( HGenericDialogConfig config, Action<bool> response )
        {
            yield return waitTime;

            ShowGenericDialog( config, response );
        }
    }
}