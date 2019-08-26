using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using UnityEngine;
using static MoPubBase.SupportedNetwork;

namespace TurboLabz.InstantFramework
{
    public class MoPubService : IAdsService
    {
        MoPubAdUnits adUnits;

        public void Init()
        {
            adUnits = new MoPubAdUnits();
            MoPubManager.OnSdkInitializedEvent += OnSdkInitializedEvent;

            // MoPub.InitializeSdk(adUnits.GetGenericAdUnit());

            MoPub.InitializeSdk(new MoPub.SdkConfiguration
            {
                AdUnitId = adUnits.GetGenericAdUnit(),

                // Set desired log level here to override default level of MPLogLevelNone
                LogLevel = MoPubBase.logLevel,

                // Uncomment the following line to allow supported SDK networks to collect user information on the basis
                // of legitimate interest.
                //AllowLegitimateInterest = true,

                // Specify the mediated networks you are using here:
                MediatedNetworks = new MoPub.MediatedNetwork[]
           {
               /*
                   // Example using AdMob.  Follow this template for other supported networks as well.
                   // Note that keys must be strings, and values must be JSON-serializable (strings only, for MoPubRequestOptions).
                   new MoPub.SupportedNetwork.AdMob
                   {
                       // Network adapter configuration settings (initialization).
                       NetworkConfiguration = {
                           { "key1", value },
                           { "key2", value },
                       },

                       // Global mediation settings (per ad request).
                       MediationSettings = {
                           { "key1", value },
                           { "key2", value },
                       },

                       // Additional options to pass to the MoPub servers (per ad request).
                       MoPubRequestOptions = {
                           { "key1", "value" },
                           { "key2", "value" },
                       }
                   },

                   // Example using a custom network adapter:
                   new MoPub.MediatedNetwork
                   {
                       // Specify the class name that implements the AdapterConfiguration interface.
                   #if UNITY_ANDROID
                       AdapterConfigurationClassName = "classname",  // include the full package name
                   #else // UNITY_IOS
                       AdapterConfigurationClassName = "classname",
                   #endif

                       // Specify the class name that implements the MediationSettings interface.
                       // Note: Custom network mediation settings are currently not supported on Android.
                   #if UNITY_IOS
                       MediationSettingsClassName = "classname",
                   #endif

                       // Fill in settings and configuration options the same way as for supported networks:

                       NetworkConfiguration = { ... },

                   #if UNITY_IOS  // See note above.
                       MediationSettings    = { ... },
                   #endif

                       MoPubRequestOptions  = { ... },
                   }
               */
           },
            });
        }

        void OnSdkInitializedEvent(string p1)
        {
            Debug.Log("MoPub: Initialized: " + p1);
            MoPubRewardedVideo.Initialize(adUnits);
            MoPubInterstitial.Initialize(adUnits);
            MoPubBanner.Initialize(adUnits);
        }

        public bool IsRewardedVideoAvailable()
        {
            return MoPubRewardedVideo.IsAvailable();
        }

        public IPromise<AdsResult> ShowRewardedVideo()
        {
            return MoPubRewardedVideo.Show();
        }

        public bool IsInterstitialAvailable()
        {
            return MoPubInterstitial.IsAvailable();
        }

        public void ShowInterstitial()
        {
            MoPubInterstitial.Show();
        }

        public void ShowBanner()
        {
            MoPubBanner.Show(MoPubBase.AdPosition.TopCenter);
        }

        public void HideBanner()
        {
            MoPubBanner.Hide();
        }
    }
}
