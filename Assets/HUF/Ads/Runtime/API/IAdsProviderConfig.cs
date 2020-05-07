using System.Collections.Generic;
using HUF.Ads.Runtime.Implementation;

namespace HUF.Ads.Runtime.API
{
    public interface IAdsProviderConfig
    {
        string AppId { get; }
        List<AdPlacementData> AdPlacementData { get; }

        AdPlacementData GetPlacementData(string placementId);
        AdPlacementData GetPlacementData(PlacementType type);
    }
}