using System;
using System.Collections;
using HUF.Ads.Runtime.API;
using HUF.Analytics.Runtime.API;
using HUF.GenericDialog.Runtime.API;
using HUF.GenericDialog.Runtime.Configs;
using HUF.PolicyGuard.Runtime.API;
using HUF.PolicyGuard.Runtime.Configs;
using HUF.PolicyGuard.Runtime.Configs.Data;
using HUF.Utils.Runtime;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using HUF.Utils.Runtime.PlayerPrefs;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;
#if HUFEXT_COUNTRY_CODE
using HUFEXT.CountryCode.Runtime.API;

#endif

namespace HUF.PolicyGuard.Runtime.Implementations
{
    public class PolicyGuardService
    {
        public const string ATT_POSTPONED_KEY = "HUF_ATT_POSTPONED";
        const float SHOW_PREFAB_DELAY = 0.2f;
        EventSystem eventSystem;

        readonly WaitForSecondsRealtime waitTime = new WaitForSecondsRealtime( SHOW_PREFAB_DELAY );
        readonly HLogPrefix logPrefix = new HLogPrefix( HPolicyGuard.logPrefix, nameof(PolicyGuardService) );
#pragma warning disable 0067
        public event Action OnGDPRPopupShowed;
        public event Action<bool> OnGDPRPopupCloses;
        public event Action OnPersonalizedAdsPopupShowed;
        public event Action<bool> OnPersonalizedAdsPopupCloses;
        public event Action OnATTPopupShowed;
        public event Action<bool> OnATTPopupCloses;
        public event Action OnATTNativePopupShowed;
        public event Action OnEndCheckingPolicy;

#if UNITY_IOS
        public event Action<AppTrackingTransparencyBridge.AuthorizationStatus> OnATTNativePopupClosed;
#endif

#pragma warning restore 0067
        PolicyGuardConfig config;
        bool isCheckFlowEnded = false;

        public bool IsCheckFlowEnded => isCheckFlowEnded;

        public PolicyGuardService()
        {
            config = HConfigs.GetConfig<PolicyGuardConfig>();

            if ( config.OverridePersonalizedAdsPopup )
                config.ReferenceToPersonalizedAdsPopup.prefab = config.OverridePersonalizedAdsPopup;

            if ( config.OverrideGDPRPopup )
                config.ReferenceToGDPRPopup.prefab = config.OverrideGDPRPopup;

            if ( config.OverrideATTPreOptInPopup )
                config.ReferenceToATTPreOptInPopup.prefab = config.OverrideATTPreOptInPopup;

            if ( config.OverrideGDPRWithAdsPopup )
                config.ReferenceToGDPRWithAdsPopup.prefab = config.OverrideGDPRWithAdsPopup;
#if UNITY_IOS && !UNITY_EDITOR
            // Refresh ATT status on game start
            if ( HPolicyGuard.WasATTPopupDisplayed() )
                AppTrackingTransparencyBridge.CheckAuthorizationStatus( );
#endif
        }

        public void CheckFlow()
        {
            if ( !config.UseAutomatedFlow )
            {
                isCheckFlowEnded = true;
                return;
            }

            if ( config.UseCustomAutomatedPopupsFlow && config.PopupsFlow.Count > 0 )
            {
                CheckPerPlatformFlow();
                return;
            }

            if ( TryShowGDPRWithAds() )
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

            isCheckFlowEnded = true;
            OnEndCheckingPolicy.Dispatch();
        }

        public bool TryShowATT()
        {
#if UNITY_IOS
            if ( !CanShowATT() )
            {
                return false;
            }

            if ( config.ShowATTPreOptInPopup )
            {
                ShowPreATTPopup();
                return true;
            }

            ShowNativeATTPopup();
            return true;
#else
            return false;
#endif
        }

        public bool TryShowGDPRWithAds()
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
            return TryShowGDPR();
        }

        public bool TryShowGDPR()
        {
            if ( HAnalytics.GetGDPRConsent() != null )
                return false;

            OnGDPRPopupShowed.Dispatch();
            ShowGDPRPopup();
            return true;
        }

        public bool TryShowPersonalizedAdsPopup()
        {
            if ( !HPolicyGuard.IsATTAuthorized() || HAds.HasPersonalizedAdConsent() != null )
            {
                return false;
            }

            ShowGenericDialog(config.ReferenceToPersonalizedAdsPopup,
                delegate (bool? consent)
                {
                    HAds.CollectSensitiveData(consent == true);
                    OnPersonalizedAdsPopupCloses.Dispatch(consent == true);
                    CheckFlow();
                });

            OnPersonalizedAdsPopupShowed.Dispatch();
            return true;
        }

#if UNITY_IOS
        void ShowPreATTPopup()
        {
            CoroutineManager.StartCoroutine( ShowDialogWithDelay( config.ReferenceToATTPreOptInPopup,
                HandlePreATTPopupClose ) );
            OnATTPopupShowed.Dispatch();
        }

        void HandlePreATTPopupClose( bool? consent )
        {
            OnATTPopupCloses.Dispatch( consent == true );

            if ( consent == true )
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
            OnATTNativePopupClosed.Dispatch( status );
            CheckFlow();
        }

        static bool CanShowATT()
        {
            if ( HPolicyGuard.WasATTPopupDisplayed() ||
                 AppTrackingTransparencyBridge.GetCurrentAuthorizationStatus() !=
                 AppTrackingTransparencyBridge.AuthorizationStatus.NotDetermined )
                return false;

            return true;
        }
#endif

#if HUFEXT_COUNTRY_CODE
        bool IsGDPRRequiredForCurrentCountry()
        {
            string country = HNativeCountryCode.GetCountryCode().Country.ToUpper();
            return config.ShowForCountries.Contains( country );
        }
#endif

        void ShowGDPRPopup()
        {
            if ( !HPolicyGuard.IsATTAuthorized() || !config.ShowAdsPrivacyConsentInGDPRPopup )
            {
                CoroutineManager.StartCoroutine( ShowDialogWithDelay( config.ReferenceToGDPRPopup,
                    HandleGDPRPopupClose ) );
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

            TryCreateEventSystem();
            var popup = (GDPRWithAdsDialog)instance;
            popup.OnPrimaryButtonClicked += () => HandleGDPRPopupClose( true );
            popup.OnAdsConsentSet += consent => { OnPersonalizedAdsPopupCloses.Dispatch( consent ); };
        }

        void HandleGDPRPopupClose( bool? consent )
        {
            TryDestroyEventSystem();
            HAnalytics.CollectSensitiveData( consent == true );
            OnGDPRPopupCloses.Dispatch( consent == true );
            CheckFlow();
        }

        void ShowGenericDialog( HGenericDialogConfig config, Action<bool?> response )
        {
            if ( !HGenericDialog.ShowDialog( config, out var instance ) )
            {
                HLog.LogError( logPrefix,
                    $"Generic dialog creation error for {( config == null ? "Null config" : config.ConfigId )}" );
                return;
            }

            TryCreateEventSystem();
            var popup = (GenericEventDialog)instance;

            popup.OnPrimaryButtonClicked += () =>
            {
                response.Dispatch( true );
                TryDestroyEventSystem();
            };

            popup.OnSecondaryButtonClicked += () =>
            {
                response.Dispatch( false );
                TryDestroyEventSystem();
            };

            popup.OnTertiaryButtonClicked += () =>
            {
                response.Dispatch( null );
                TryDestroyEventSystem();
            };
        }

        IEnumerator ShowDialogWithDelay( HGenericDialogConfig config, Action<bool?> response )
        {
            yield return waitTime;

            ShowGenericDialog( config, response );
        }

        void CheckPerPlatformFlow()
        {
            for ( int i = 0; i < config.PopupsFlow.Count; i++ )
            {
                if ( config.PopupsFlow[i].type == PolicyPopup.PopupType.Custom )
                {
                    if ( HPlayerPrefs.HasKey( config.PopupsFlow[i].playerPrefsKey ) ||
                         config.PopupsFlow[i].popupConfig == null )
                        continue;

                    CoroutineManager.StartCoroutine( ShowDialogWithDelay( config.PopupsFlow[i].popupConfig,
                        result =>
                        {
                            if ( config.PopupsFlow[i].setKeyAutomatically )
                                PlayerPrefs.SetInt( config.PopupsFlow[i].playerPrefsKey,
                                    (int)( result == null
                                        ? PlayerPrefsKeyState.Postponed
                                        : result == true
                                            ? PlayerPrefsKeyState.Accepted
                                            : PlayerPrefsKeyState.Denied
                                    ) );
                            CheckFlow();
                        } ) );
                    return;
                }

                switch ( config.PopupsFlow[i].type )
                {
                    case PolicyPopup.PopupType.GDPRWithAds:
                        if ( TryShowGDPRWithAds() )
                            return;

                        break;
                    case PolicyPopup.PopupType.GDPR:
                        if ( TryShowGDPR() )
                            return;

                        break;
                    case PolicyPopup.PopupType.ATT:
                        if ( !HPlayerPrefs.GetBool( ATT_POSTPONED_KEY ) && TryShowATT() )
                            return;

                        break;
                    case PolicyPopup.PopupType.PersonalizedAds:
                        if ( TryShowPersonalizedAdsPopup() )
                            return;

                        break;
                    default:
                        continue;
                }
            }

            isCheckFlowEnded = true;
            OnEndCheckingPolicy.Dispatch();
        }

        void TryCreateEventSystem()
        {
            if ( config.CreateUnityUIEventSystemIfNotExists && EventSystem.current == null )
            {
                eventSystem = new GameObject( "EventSystem", typeof(EventSystem), typeof(StandaloneInputModule) )
                    .GetComponent<EventSystem>();
            }
        }

        void TryDestroyEventSystem()
        {
            if ( eventSystem != null )
                Object.Destroy( eventSystem.gameObject );
        }
    }
}