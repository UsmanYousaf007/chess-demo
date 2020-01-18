using System.Collections.Generic;
using HUF.Ads.Implementation;

namespace HUF.Ads.API
{
    public interface IAdsProviderConfig
    {
        string AppId { get; }
        List<AdPlacementData> AdPlacementData { get; }

        AdPlacementData GetPlacementData(string placementId);
        AdPlacementData GetPlacementData(PlacementType type);
    }
}