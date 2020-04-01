namespace HUFEXT.AdsManager.Runtime.API
{
    public struct AdsManagerFetchCallbackData
    {
        public string ProviderId { get; }
        public string PlacementId { get; }

        public AdsManagerFetchCallbackData( string providerId, string placementId )
        {
            ProviderId = providerId;
            PlacementId = placementId;
        }
    }
}