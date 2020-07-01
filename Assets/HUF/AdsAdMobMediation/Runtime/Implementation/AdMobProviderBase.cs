using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using GoogleMobileAds.Api;
using GoogleMobileAds.Api.Mediation.AdColony;
using GoogleMobileAds.Api.Mediation.AppLovin;
using GoogleMobileAds.Api.Mediation.Chartboost;
using GoogleMobileAds.Api.Mediation.Fyber;
//using GoogleMobileAds.Api.Mediation.InMobi;
using GoogleMobileAds.Api.Mediation.Tapjoy;
using GoogleMobileAds.Api.Mediation.UnityAds;
//using GoogleMobileAds.Api.Mediation.Vungle;
using GoogleMobileAdsMediationTestSuite.Api;
using HUF.Ads.Runtime.API;
using HUF.Ads.Runtime.Implementation;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using UnityEngine;
using UnityEngine.Events;
using PresageLib;

#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
using UnityEngine.iOS;
#endif

namespace HUF.AdsAdMobMediation.Runtime.Implementation
{
    public class AdMobProviderBase : IAdProvider
    {
        public event UnityAction<PaidEventData> OnPaidEvent;

        const string VUNGLE_CONSENT_VERSION = "1.0.0";
        public string ProviderId => "AdMob";
        public bool IsInitialized { get; private set; }

        bool isInitializing;
        string testDeviceId;

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
                return false;

            isInitializing = true;

            if ( Debug.isDebugBuild )
                EnableTestMode();

            if ( Application.isEditor )
            {
                IsInitialized = true;
                MobileAds.Initialize( Config.AppId );
                adsService?.ServiceInitialized();
            }
            else
            {
                MobileAds.Initialize( HandleAdMobInitialized );
            }

            AppLovin.Initialize();

            if ( !Config.MoPubAppId.IsNullOrEmpty() )
                GoogleMobileAds.Api.Mediation.MoPub.MoPub.Initialize( Config.MoPubAppId );

            if ( !Config.OguryAppId.IsNullOrEmpty() )
            {
#if UNITY_ANDROID
                Presage.Initialize( Config.OguryAppId );
#elif UNITY_IOS
                PresageIos.Initialize(Config.OguryAppId);
#endif
            }

            return true;
        }

        void HandleAdMobInitialized( InitializationStatus status )
        {
            IsInitialized = true;

            syncContext.Post(
                data =>
                {
                    var adapters = status.getAdapterStatusMap();

                    foreach ( var adapter in adapters )
                    {
                        HLog.LogImportant( logPrefix,
                            $"Adapter status {adapter.Key} {adapter.Value.InitializationState.ToString()}" );
                    }

                    adsService?.ServiceInitialized();
                },
                null );
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
            //SetConsentVungle( consentStatus );
            SetConsentInMobi( consentStatus );
            AppLovin.SetHasUserConsent( consentStatus );
            GoogleMobileAds.Api.Mediation.IronSource.IronSource.SetConsent( consentStatus );
            Chartboost.RestrictDataCollection( consentStatus );
            UnityAds.SetGDPRConsentMetaData( consentStatus );
            AdColonyAppOptions.SetGDPRRequired( consentStatus );
            AdColonyAppOptions.SetGDPRConsentString( consentStatus ? "1" : "0" );
            Tapjoy.SetUserConsent( consentStatus ? "1" : "0" );
            Tapjoy.SubjectToGDPR( consentStatus );
            Fyber.SetGDPRConsent(consentStatus);
/* Uncomment when we will be whitelisted by Ogury
 #if UNITY_IOS
            ConsentIos.ClientConsent.SetConsent(Config.OguryAppId, "Huuuge", new string[0],0);
#elif UNITY_ANDROID
            ConsentAndroid.ClientConsent.SetConsent( Config.OguryAppId, "Huuuge", new string[0] );
#endif*/
        }

        /*void SetConsentVungle( bool consentStatus )
        {
            var consent = consentStatus ? VungleConsent.ACCEPTED : VungleConsent.DENIED;
            Vungle.UpdateConsentStatus( consent, VUNGLE_CONSENT_VERSION );
        }*/

        void SetConsentInMobi( bool consentStatus )
        {
            Dictionary<string, string> consentObject = new Dictionary<string, string>();
            consentObject.Add( "gdpr_consent_available", consentStatus ? "true" : "false" );
            consentObject.Add( "gdpr", consentStatus ? "1" : "0" );
            //InMobi.UpdateGDPRConsent( consentObject );
        }

        internal AdRequest CreateRequest()
        {
            var builder = new AdRequest.Builder();

            if ( HAds.GetGDPRConsent() != true )
                builder.AddExtra( "npa", "1" );

            if ( Debug.isDebugBuild && Config.TestDevices != null )
            {
                foreach ( var device in Config.TestDevices )
                {
                    builder.AddTestDevice( device );
                }
            }

            if ( !string.IsNullOrEmpty( testDeviceId ) )
                builder.AddTestDevice( testDeviceId );
            return builder.Build();
        }

        internal AdPlacementData GetPlacementData( PlacementType type )
        {
            var data = Config.AdPlacementData.FirstOrDefault( x => x.PlacementType == type );

            if ( data == null )
                HLog.LogError( logPrefix,
                    $"Placement data with type {type} not found! Make sure it's set in config file!" );
            return data;
        }

        public void ShowTestSuite()
        {
            MediationTestSuite.Show();
        }

        public void HandleAdPaidEvent( PlacementType placementType,
            string placementId,
            string adapterName,
            object sender,
            AdValueEventArgs e )
        {
            syncContext.Post(
                data =>
                {
                    var shortAdapterId = GetShortAdapterId( adapterName );

                    HLog.Log( logPrefix,
                        $"HandleAdPaidEvent received with ad value (in micros):{e.AdValue.Value}, precision: {e.AdValue.Precision}, currency:{e.AdValue.CurrencyCode}" );

                    OnPaidEvent.Dispatch( new PaidEventData( shortAdapterId,
                        ProviderId,
                        (int)e.AdValue.Value,
                        placementId,
                        placementType.ToString(),
                        e.AdValue.CurrencyCode,
                        e.AdValue.Precision.ToString() ) );
                },
                null );
        }

        string GetShortAdapterId( string adapterName )
        {
            var shortAdapterName = adapterName;
#if UNITY_IOS
            shortAdapterName = shortAdapterName.Replace( "GADMAdapter", "");
            shortAdapterName = shortAdapterName.Replace( "GADMediationAdapter", "");

            if ( shortAdapterName == "GoogleAdMobAds" )
            {
                shortAdapterName = "AdMob";
            }
#elif UNITY_ANDROID
            var splitedAdapter = shortAdapterName.Split( '.' );
            shortAdapterName = splitedAdapter[splitedAdapter.Length - 1];
            shortAdapterName = shortAdapterName.Replace( "MediationAdapter", "" );
            shortAdapterName = shortAdapterName.Replace( "RewardedAdapter", "" );
            shortAdapterName = shortAdapterName.Replace( "Adapter", "" );
#endif
            return shortAdapterName;
        }

        public void EnableTestMode()
        {
            AdColonyAppOptions.SetTestMode( true );
            string deviceId = GetDeviceId();

            if ( testDeviceId != deviceId )
            {
                testDeviceId = deviceId;

                if ( !testDeviceId.IsNullOrEmpty() )
                    HLog.LogAlways( logPrefix, $"Test mode is enabled for device id: {testDeviceId}" );
            }
        }

        static string GetDeviceId()
        {
            return Application.platform == RuntimePlatform.Android
                ? SystemInfo.deviceUniqueIdentifier.ToUpper()
                : SystemInfo.deviceUniqueIdentifier;
        }
    }
}