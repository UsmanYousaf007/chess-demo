using HUF.Ads.Runtime.API;
using HUF.Ads.Runtime.Implementation;

namespace HUFEXT.AdsManager.Runtime.AdMediation
{
    public struct AdCallback
    {
        public string PlacementId { get; }
        public string MediationId { get; }
        public string AdapterId { get; }
        
        public float HeightInPx { get; }

        public float HeightInDp => HAdsUtils.ConvertPixelsToDp(HeightInPx);

        public AdResult Result { get; }

        public AdCallback(string placementId, string mediationId)
        {
            PlacementId = placementId;
            MediationId = mediationId;
            AdapterId = string.Empty;
            HeightInPx = 0;
            Result = AdResult.None;
        }
        
        public AdCallback(string placementId, string mediationId, AdResult adResult)
        {
            PlacementId = placementId;
            MediationId = mediationId;
            AdapterId = string.Empty;
            HeightInPx = 0;
            Result = adResult;
        }
        
        public AdCallback(string placementId, string mediationId, string adapterId, AdResult adResult)
        {
            PlacementId = placementId;
            MediationId = mediationId;
            AdapterId = adapterId;
            HeightInPx = 0;
            Result = adResult;
        }

        public AdCallback(string placementId, string mediationId, AdResult adResult, float heightInPx)
        {
            PlacementId = placementId;
            MediationId = mediationId;
            AdapterId = string.Empty;
            HeightInPx = heightInPx;
            Result = adResult;
        }
        
        public AdCallback(string placementId, string mediationId, string adapterId, AdResult adResult, float heightInPx)
        {
            PlacementId = placementId;
            MediationId = mediationId;
            AdapterId = adapterId;
            HeightInPx = heightInPx;
            Result = adResult;
        }
    }
}