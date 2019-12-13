using System.Linq;
using GoogleMobileAds.Api;
using GoogleMobileAds.Api.Mediation.AppLovin;
using GoogleMobileAds.Api.Mediation.Vungle;
using GoogleMobileAdsMediationTestSuite.Api;
using HUF.Ads.API;
using HUF.Ads.Implementation;
using HUF.Utils.Configs.API;
using UnityEngine;

namespace HUF.AdsAdMobMediation.Implementation
{
    public class AdMobProviderBase : IAdProvider
    {
        const string VUNGLE_CONSENT_VERSION = "1.0.0";
        public string ProviderId => "adMob";
        public bool IsInitialized { get; private set; }

        readonly string logPrefix;
        internal AdMobProviderConfig Config { get; }

        public AdMobProviderBase()
        {
            Config = HConfigs.GetConfig<AdMobProviderConfig>();
            logPrefix = GetType().Name;
        }
        
        public bool Init()
        {
            if (IsInitialized)
            {
                return false;
            }

            MobileAds.Initialize(Config.AppId);
            AppLovin.Initialize();

            IsInitialized = true;
            return true;
        }

        public void CollectSensitiveData(bool consentStatus)
        {
            SetConsentVungle(consentStatus);
            AppLovin.SetHasUserConsent(consentStatus);
        }

        void SetConsentVungle(bool consentStatus)
        {
            var consent = consentStatus ? VungleConsent.ACCEPTED : VungleConsent.DENIED;
            Vungle.UpdateConsentStatus(consent, VUNGLE_CONSENT_VERSION);
        }

        internal AdRequest CreateRequest()
        {
            var builder = new AdRequest.Builder();

            if (Debug.isDebugBuild && Config.TestDevices != null)
            {
                foreach (var device in Config.TestDevices)
                {
                    builder.AddTestDevice(device);
                }
            }

            return builder.Build();
        }
        
        internal AdPlacementData GetPlacementData(PlacementType type)
        {
            var data = Config.AdPlacementData.FirstOrDefault(x => x.PlacementType == type);
            if (data == null)
                Debug.LogError(
                    $"[{logPrefix}] Placement data with type {type} not found! Make sure it's set in config file!");
            return data;
        }

        public void ShowTestSuite()
        {
            MediationTestSuite.Show(Config.AppId);   
        }
    }
}