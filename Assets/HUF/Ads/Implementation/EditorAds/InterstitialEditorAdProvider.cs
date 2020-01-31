using HUF.Ads.API;
using HUF.Utils.Extensions;
using UnityEngine.Events;

namespace HUF.Ads.Implementation.EditorAds
{
    public class InterstitialEditorAdProvider : BaseEditorAdProvider, IInterstitialAdProvider
    {
        public event UnityAction<IAdCallbackData> OnInterstitialEnded;
        public event UnityAction<IAdCallbackData> OnInterstitialFetched;
        public event UnityAction<IAdCallbackData> OnInterstitialClicked;

        public override bool Init()
        {
            placementType = PlacementType.Interstitial;
            return base.Init();
        }

        protected override void OnAdResult(AdResult adResult)
        {
            OnInterstitialEnded.Dispatch(new AdCallbackData(ProviderId, lastShownPlacement, adResult));
        }

        public override void Fetch()
        {
            base.Fetch();
            OnInterstitialFetched.Dispatch(new AdCallbackData(ProviderId, lastFetchedPlacement, AdResult.Completed));
        }

        public override void Fetch(string placementId)
        {
            base.Fetch(placementId);
            OnInterstitialFetched.Dispatch(new AdCallbackData(ProviderId, placementId, AdResult.Completed));
        }
    }
}