using System;
using UnityEngine;

namespace HUF.Ads.Runtime.Implementation
{
    [Serializable]
    public class AdPlacementData
    {
        #pragma warning disable 0649
        [SerializeField] string placementId;
        [SerializeField] ApplicationIdentifier appId;
        [SerializeField] PlacementType placementType;
        #pragma warning restore 0649

        public string PlacementId => placementId;
        public string AppId => appId.Value;
        public PlacementType PlacementType => placementType;
    }
}