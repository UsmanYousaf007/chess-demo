using HUF.Ads.API;
using UnityEngine;

namespace HUF.AdsAdMobMediation.Implementation
{
    public abstract class AdMobAdProvider : IAdProvider
    {
        public bool IsInitialized => baseProvider.IsInitialized;
        public string ProviderId => baseProvider.ProviderId;

        protected readonly AdMobProviderBase baseProvider;
        protected readonly string logPrefix;
        protected AdMobProviderConfig Config => baseProvider.Config;

        protected AdMobAdProvider(AdMobProviderBase baseProvider)
        {
            this.baseProvider = baseProvider;
            logPrefix = GetType().Name;
        }

        public bool Init()
        {
            if (IsInitialized)
            {
                Debug.LogWarning($"[{logPrefix}] Already initialized!");
                return false;
            }

            baseProvider.Init();
            return true;
        }

        public void CollectSensitiveData(bool consentStatus)
        {
            baseProvider.CollectSensitiveData(consentStatus);
        }
    }
}