using HUF.Ads.Runtime.API;

namespace HUF.Ads.Runtime.Implementation
{
    public struct AdCallbackData : IAdCallbackData
    {
        public string ProviderId { get; }
        public string PlacementId { get; }
        public AdResult Result { get; }

        public AdCallbackData(string providerId, string placementId, AdResult result)
        {
            ProviderId = providerId;
            PlacementId = placementId;
            Result = result;
        }
    }
}