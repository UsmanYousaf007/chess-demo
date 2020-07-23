using System.Threading;
using HUF.Ads.Runtime.API;
using HUF.Utils.Runtime.Logging;

namespace HUF.AdsAdMobMediation.Runtime.Implementation
{
    public abstract class AdMobAdProvider : IAdProvider
    {
        public bool IsInitialized => baseProvider.IsInitialized;
        public string ProviderId => baseProvider.ProviderId;

        protected readonly AdMobProviderBase baseProvider;
        protected readonly HLogPrefix logPrefix;
        protected SynchronizationContext syncContext;

        protected AdMobProviderConfig Config => baseProvider.Config;

        protected AdMobAdProvider( AdMobProviderBase baseProvider )
        {
            this.baseProvider = baseProvider;
            logPrefix = new HLogPrefix( HAds.logPrefix, nameof(AdMobAdProvider) );
            syncContext = SynchronizationContext.Current;
        }

        public bool Init()
        {
            if ( IsInitialized )
            {
                HLog.LogWarning( logPrefix, "Already initialized!" );
                return false;
            }

            baseProvider.Init();
            return true;
        }

        public void CollectSensitiveData( bool consentStatus )
        {
            baseProvider.CollectSensitiveData( consentStatus );
        }
    }
}