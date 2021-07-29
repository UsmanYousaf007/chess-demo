using System;
using HUF.Ads.Runtime.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using UnityEngine.Events;

namespace HUF.Ads.Runtime.Implementation.EditorAds
{
    public class InterstitialEditorAdProvider : BaseEditorAdProvider, IInterstitialAdProvider
    {
#pragma warning disable CS0067
        public event Action<IAdCallbackData> OnInterstitialEnded;
        public event Action<IAdCallbackData> OnInterstitialFetched;
        public event Action<IAdCallbackData> OnInterstitialClicked;
#pragma warning restore CS0067

        protected override HLogPrefix LogPrefix { get; } = new HLogPrefix( nameof(InterstitialEditorAdProvider) );

        public override bool Init()
        {
            placementType = PlacementType.Interstitial;
            return base.Init();
        }

        protected override void OnAdResult( AdResult adResult )
        {
            OnInterstitialEnded.Dispatch( new AdCallbackData( ProviderId, lastShownPlacement, adResult ) );
        }

        public override void Fetch()
        {
            base.Fetch();

            OnInterstitialFetched.Dispatch( new AdCallbackData( ProviderId,
                lastFetchedPlacement,
                AdResult.Completed ) );
        }

        public override void Fetch( string placementId )
        {
            base.Fetch( placementId );
            OnInterstitialFetched.Dispatch( new AdCallbackData( ProviderId, placementId, AdResult.Completed ) );
        }
    }
}