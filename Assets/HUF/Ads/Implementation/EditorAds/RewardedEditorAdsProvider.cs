using HUF.Ads.API;
using HUF.Utils.Extensions;
using UnityEngine.Events;

namespace HUF.Ads.Implementation.EditorAds
{
    public class RewardedEditorAdsProvider : BaseEditorAdProvider, IRewardedAdProvider
    {
        public event UnityAction<IAdCallbackData> OnRewardedEnded;
        public event UnityAction<IAdCallbackData> OnRewardedFetched;
        public event UnityAction<IAdCallbackData> OnRewardedClicked;

        public override bool Init()
        {
            placementType = PlacementType.Rewarded;
            
            return base.Init();
        }

        protected override void OnAdResult(AdResult adResult)
        {
            OnRewardedEnded.Dispatch(new AdCallbackData(ProviderId, lastShownPlacement, adResult));
        }

        public override void Fetch()
        {
            base.Fetch();
            OnRewardedFetched.Dispatch(new AdCallbackData(ProviderId, lastFetchedPlacement, AdResult.Completed));
        }

        public override void Fetch(string placementId)
        {
            base.Fetch(placementId);
            OnRewardedFetched.Dispatch(new AdCallbackData(ProviderId, placementId, AdResult.Completed));
        }
    }
}