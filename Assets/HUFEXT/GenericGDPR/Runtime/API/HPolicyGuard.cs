using System;
using HUF.Ads.Runtime.API;
using HUF.Analytics.Runtime.API;
using HUF.Analytics.Runtime.Implementation;
using HUF.GenericDialog.Runtime.API;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;

namespace HUFEXT.GenericGDPR.Runtime.API
{
    public static class HPolicyGuard
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( HGenericGDPR.logPrefix, nameof(HPolicyGuard) );

        static event Action OnPolicyCheckEnded;
        static int sessionNumber;
        static GDPRConfig config;

        [PublicAPI]
        public static void RunChecks( int sessionNumber, Action callback )
        {
#if UNITY_IOS
            void HandleCurrentAuthorizationStatus( AppTrackingTransparencyBridge.AuthorizationStatus status )
            {
                if ( status == AppTrackingTransparencyBridge.AuthorizationStatus.NotDetermined )
                {
                    TryShowATTWindow();
                }
                else
                {
                    TryShowPersonalizedAds();
                }
            }
#endif
            
            OnPolicyCheckEnded = callback;

            if ( !HGenericGDPR.IsPolicyAccepted )
            {
                HLog.LogError( logPrefix, "Handle GDPR before, aborting!" );
                OnPolicyCheckEnded.Dispatch();
                return;
            }

            config = HConfigs.GetConfig<GDPRConfig>();
            HPolicyGuard.sessionNumber = sessionNumber;

#if UNITY_IOS
            AppTrackingTransparencyBridge.GetCurrentAuthorizationStatus( HandleCurrentAuthorizationStatus );
#else
            TryShowPersonalizedAds();
#endif
        }


        static void TryShowATTWindow()
        {
            if ( config.AttConfig != null && !config.SkipAttPopup &&
                 HGenericDialog.CheckSchedule( config.AttConfig, sessionNumber ) &&
                 HGenericDialog.ShowDialog( config.AttConfig, out var instance ) )
            {
                instance.OnClosePopup.AddListener( TryShowPersonalizedAds );
            }
            else
            {
                if ( config.AttConfig == null )
                {
                    HLog.LogError( logPrefix, "There is no AttConfig config specified in GDPR config!" );
                }

                TryShowPersonalizedAds();
            }
        }

        static void TryShowPersonalizedAds()
        {
            bool? adConsent = HAds.HasPersonalizedAdConsent();

            if ( !adConsent.HasValue && config.PersonalizedAdsConfig != null && !config.SkipPersonalizedAds &&
                 HGenericDialog.CheckSchedule( config.PersonalizedAdsConfig, sessionNumber ) &&
                 HGenericDialog.ShowDialog( config.PersonalizedAdsConfig, out var instance ) )
            {
                instance.OnClosePopup.AddListener( OnPolicyCheckEnded.Dispatch );
            }
            else
            {
                if ( config.PersonalizedAdsConfig == null )
                {
                    HLog.LogError( logPrefix, "There is no PersonalizedAds config specified in GDPR config!" );
                }

                OnPolicyCheckEnded.Dispatch();
            }
        }
    }
}