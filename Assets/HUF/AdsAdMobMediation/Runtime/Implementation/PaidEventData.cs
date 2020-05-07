namespace HUF.AdsAdMobMediation.Runtime.Implementation
{
    public struct PaidEventData
    {
        public string AdapterId { get; }
        public string MediationId { get; }
        public int Cents { get; }
        public string PlacementType { get; }
        public string CurrencyCode { get; }
        public string Precision { get; }
        public string PlacementId { get; }

        public PaidEventData( string adapterId,
            string mediationId,
            int cents,
            string placementId,
            string placementType,
            string currencyCode,
            string precision )
        {
            AdapterId = adapterId;
            MediationId = mediationId;
            Cents = cents;
            PlacementType = placementType;
            CurrencyCode = currencyCode;
            Precision = precision;
            PlacementId = placementId;
        }
    }
}