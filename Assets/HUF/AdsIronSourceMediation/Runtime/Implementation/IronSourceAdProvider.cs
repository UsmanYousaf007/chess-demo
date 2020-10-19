using HUF.Ads.Runtime.API;
using HUF.Ads.Runtime.Implementation;
using HUF.AdsIronSourceMediation.Runtime.API;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Logging;

namespace HUF.AdsIronSourceMediation.Runtime.Implementation
{
    public abstract class IronSourceAdProvider : IAdProvider
    {
        protected static readonly HLogPrefix logPrefix =
            new HLogPrefix( HAdsIronSourceMediation.logPrefix, nameof(IronSourceAdProvider) );

        protected readonly IronSourceAdsProviderConfig config;
        protected readonly PlacementType placementType;

        readonly IronSourceBaseProvider baseProvider;

        protected IronSourceAdProvider( IronSourceBaseProvider baseProvider, PlacementType placementType )
        {
            config = HConfigs.GetConfig<IronSourceAdsProviderConfig>();
            this.baseProvider = baseProvider;
            this.placementType = placementType;
        }

        ~IronSourceAdProvider()
        {
            UnsubscribeEvents();
        }

        protected abstract void SubscribeEvents();
        protected abstract void UnsubscribeEvents();

        public string ProviderId => baseProvider.ProviderId;
        public bool IsInitialized { get; private set; }

        public bool Init()
        {
            if ( IsInitialized )
            {
                return false;
            }

            baseProvider.Init();
            SubscribeEvents();
            IsInitialized = true;
            return true;
        }

        public void CollectSensitiveData( bool consentStatus )
        {
            baseProvider.CollectSensitiveData( consentStatus );
        }

        protected bool IsCorrectPlacementType( PlacementType placementType )
        {
            var isCorrect = this.placementType == placementType;

            if ( !isCorrect )
            {
                HLog.LogWarning( logPrefix,
                    $"Wrong placement type. Given: {placementType}, expected: {this.placementType}" );
            }

            return isCorrect;
        }
    }
}