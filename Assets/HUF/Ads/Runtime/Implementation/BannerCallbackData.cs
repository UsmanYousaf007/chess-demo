using HUF.Ads.Runtime.API;

namespace HUF.Ads.Runtime.Implementation
{
    public struct BannerCallbackData : IBannerCallbackData
    {
        public string ProviderId { get; }
        public float HeightInPx { get; }
        public float HeightInDp { get; }
        
        public float Height { get; }

        public string PlacementId { get; }

        public BannerCallbackData(string providerId, string placementId, float heightInPx)
        {
            ProviderId = providerId;
            PlacementId = placementId;
            HeightInPx = heightInPx;
            HeightInDp = HAdsUtils.ConvertPixelsToDp(heightInPx);

            Height = heightInPx;
        }
    }
}