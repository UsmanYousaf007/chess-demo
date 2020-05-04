using System.Collections.Generic;
using System.Linq;
using HUF.Ads.Runtime.API;
using HUF.Utils.Runtime.AndroidManifest;
using UnityEngine;

namespace HUF.Ads.Runtime.Implementation
{
    public abstract class AdsProviderConfig : AndroidManifestKeysConfig, IAdsProviderConfig
    {
        #pragma warning disable 0649
        [SerializeField] ApplicationIdentifier appId;
        [SerializeField] List<AdPlacementData> adPlacementData;
        #pragma warning restore 0649
        [SerializeField] bool useEditorMockProvider = true;
        
        public string AppId => appId.Value;
        public List<AdPlacementData> AdPlacementData => adPlacementData;
        public bool UseEditorMockProvider => useEditorMockProvider;
        
#if UNITY_EDITOR
        public ApplicationIdentifier EditorApplicationIdentifier => appId;
#endif
        
        public AdPlacementData GetPlacementData(string placementId)
        {
            var data = AdPlacementData.FirstOrDefault(x => x.PlacementId == placementId);
            if (data == null)
                Debug.LogError(
                    $"[{GetType().Name}] Placement data for {placementId} not found! Make sure it's set in config file!");
            return data;
        }

        public AdPlacementData GetPlacementData(PlacementType type)
        {
            var data = AdPlacementData.FirstOrDefault(x => x.PlacementType == type);
            if (data == null)
                Debug.LogError(
                    $"[{GetType().Name}] Placement data with type {type} not found! Make sure it's set in config file!");
            return data;
        }
    }
}