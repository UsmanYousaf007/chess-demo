using System;
using UnityEngine;

namespace HUF.Ads.Implementation
{
    [Serializable]
    public class AdPlacementData
    {
        #pragma warning disable 0649
        [SerializeField] ApplicationIdentifier appId;
        [SerializeField] string placementId;
        [SerializeField] PlacementType placementType;
        #pragma warning restore 0649

        public string AppId => appId.Value;
        public string PlacementId => placementId;
        public PlacementType PlacementType => placementType;
    }
}