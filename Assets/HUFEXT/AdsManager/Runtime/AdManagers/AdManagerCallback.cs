using HUF.Ads.Runtime.API;
using HUF.Ads.Runtime.Implementation;

namespace HUFEXT.AdsManager.Runtime.AdManagers
{
    public class AdManagerCallback
    {
        public string ProviderId { get; }
        public string PlacementId { get; }

        public float HeightInPx { get; }
        public float HeightInDp { get; }
        public AdResult Result { get; }

        public AdManagerCallback( string providerId, string placementId, AdResult result )
        {
            ProviderId = providerId;
            PlacementId = placementId;
            Result = result;
        }

        public AdManagerCallback( string providerId, string placementId, float heightInPx )
        {
            ProviderId = providerId;
            PlacementId = placementId;
            HeightInPx = heightInPx;
            HeightInDp = HAdsUtils.ConvertPixelsToDp( heightInPx );
            Result = AdResult.Completed;
        }
    }
}