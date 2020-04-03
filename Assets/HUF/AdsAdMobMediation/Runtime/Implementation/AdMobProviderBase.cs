using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GoogleMobileAds.Api;
using GoogleMobileAds.Api.Mediation.AdColony;
using GoogleMobileAds.Api.Mediation.AppLovin;
using GoogleMobileAds.Api.Mediation.Chartboost;
using GoogleMobileAds.Api.Mediation.InMobi;
using GoogleMobileAds.Api.Mediation.Vungle;
using GoogleMobileAds.Api.Mediation.IronSource;
using GoogleMobileAds.Api.Mediation.MoPub;
using GoogleMobileAds.Api.Mediation.Tapjoy;
using GoogleMobileAds.Api.Mediation.UnityAds;
using GoogleMobileAdsMediationTestSuite.Api;
using HUF.Ads.API;
using HUF.Ads.Implementation;
using HUF.Utils.Configs.API;
using HUF.Utils.Extensions;
using HUF.Utils.Runtime.Logging;
using PresageLib;
using UnityEngine;

namespace HUF.AdsAdMobMediation.Implementation
{
    public class AdMobProviderBase : IAdProvider
    {
        const string VUNGLE_CONSENT_VERSION = "1.0.0";
        public string ProviderId => "adMob";
        public bool IsInitialized { get; private set; }

        bool isInitializing;

        static HLogPrefix logPrefix = new HLogPrefix( nameof(AdMobProviderBase) );
        internal AdMobProviderConfig Config { get; }

        SynchronizationContext syncContext;
        IAdsService adsService;

        public AdMobProviderBase()
        {
            syncContext = SynchronizationContext.Current;
            Config = HConfigs.GetConfig<AdMobProviderConfig>();
            
        }
        
        public bool Init()
        {
            if ( isInitializing )
            {
                return false;
            }

            isInitializing = true;
            AdColonyAppOptions.SetTestMode( Debug.isDebugBuild );

            HLog.Log(logPrefix, "Try Initialize Admob" );

            if ( Application.isEditor )
            {
                IsInitialized = true;
                MobileAds.Initialize( Config.AppId );
                adsService?.ServiceInitialized();
            }
            else
            {
                MobileAds.Initialize(
                    status =>
                    {
                        IsInitialized = true;

                        syncContext.Post(
                            s =>
                            {
                                HLog.Log(logPrefix, "AdMob initialized" );
                                var adapters = status.getAdapterStatusMap();

                                foreach ( var adapter in adapters )
                                {
                                    HLog.LogImportant(logPrefix, $"Adapter status {adapter.Key} {adapter.Value.InitializationState.ToString()}" );
                                }

                                adsService?.ServiceInitialized();
                            },
                            null );
                    } );
            }

            AppLovin.Initialize();

            if ( !Config.MoPubAppId.IsNullOrEmpty() )
                MoPub.Initialize( Config.MoPubAppId );
            
            if ( !Config.OguryAppId.IsNullOrEmpty() )
            {
#if UNITY_ANDROID
                Presage.Initialize(Config.OguryAppId);
#elif UNITY_IOS
                PresageIos.Initialize(Config.OguryAppId);
#endif
            }

            return true;
        }

        public void SetAdsService( IAdsService inAdsService )
        {
            adsService = inAdsService;

            if ( IsInitialized )
            {
                adsService.ServiceInitialized();
            }
        }

        public void CollectSensitiveData( bool consentStatus )
        {
            SetConsentVungle( consentStatus );
            SetConsentInMobi( consentStatus );
            AppLovin.SetHasUserConsent( consentStatus );
            IronSource.SetConsent( consentStatus );
            Chartboost.RestrictDataCollection( consentStatus );
            UnityAds.SetGDPRConsentMetaData( consentStatus );
            AdColonyAppOptions.SetGDPRRequired( consentStatus );
            AdColonyAppOptions.SetGDPRConsentString( consentStatus ? "1" : "0" );
            Tapjoy.SetUserConsent( consentStatus ? "1" : "0" );
            Tapjoy.SubjectToGDPR( consentStatus );
/* Uncomment when we will be whitelisted by Ogury
 #if UNITY_IOS
            ConsentIos.ClientConsent.SetConsent(Config.OguryAppId, "Huuuge", new string[0],0);
#elif UNITY_ANDROID
            ConsentAndroid.ClientConsent.SetConsent( Config.OguryAppId, "Huuuge", new string[0] );
#endif*/
        }

        void SetConsentVungle( bool consentStatus )
        {
            var consent = consentStatus ? VungleConsent.ACCEPTED : VungleConsent.DENIED;
            Vungle.UpdateConsentStatus( consent, VUNGLE_CONSENT_VERSION );
        }

        void SetConsentInMobi( bool consentStatus )
        {
            Dictionary<string, string> consentObject = new Dictionary<string, string>();
            consentObject.Add( "gdpr_consent_available", consentStatus ? "true" : "false" );
            consentObject.Add( "gdpr", consentStatus ? "1" : "0" );
            InMobi.UpdateGDPRConsent( consentObject );
        }

        internal AdRequest CreateRequest()
        {
            var builder = new AdRequest.Builder();
            
            if (HAds.GetGDPRConsent() != true)
                builder.AddExtra( "npa", "1" );
            
            if ( Debug.isDebugBuild && Config.TestDevices != null )
            {
                foreach ( var device in Config.TestDevices )
                {
                    builder.AddTestDevice( device );
                }
            }

            return builder.Build();
        }

        internal AdPlacementData GetPlacementData( PlacementType type )
        {
            var data = Config.AdPlacementData.FirstOrDefault( x => x.PlacementType == type );

            if ( data == null )
                HLog.LogError(logPrefix, $"Placement data with type {type} not found! Make sure it's set in config file!" );
            return data;
        }

        public void ShowTestSuite()
        {
            MediationTestSuite.Show( Config.AppId );
        }
    }
}