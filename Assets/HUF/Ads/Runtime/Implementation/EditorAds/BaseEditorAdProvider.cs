using System.Collections.Generic;
using System.Linq;
using HUF.Utils.Configs.API;
using UnityEngine;

namespace HUF.Ads.Implementation.EditorAds
{
    public abstract class BaseEditorAdProvider
    {
        public string ProviderId => "EditorAds";
        public bool IsInitialized => true;

        readonly HashSet<string> fetchedPlacements = new HashSet<string>();
        protected string lastShownPlacement;
        protected string lastFetchedPlacement;
        protected PlacementType placementType;

        string logPrefix;
        GameObject editorAdsGuiObject;

        public virtual bool Init()
        {
            logPrefix = $"[{GetType().Name}]";
            Debug.Log($"{logPrefix} Initialized Editor ads provider");
            return true;
        }

        public void CollectSensitiveData(bool consentStatus)
        {
            Debug.Log($"{logPrefix} Collect sensitive data: {consentStatus}");
        }

        public bool IsReady()
        {
            return IsReady(GetPlacementId());
        }

        public bool IsReady(string placementId)
        {
            Debug.Log($"{logPrefix} Is ready");
            return fetchedPlacements.Contains(placementId);
        }

        public bool Show()
        {
            return Show(GetPlacementId());
        }

        public bool Show(string placementId)
        {
            if (editorAdsGuiObject != null || !fetchedPlacements.Contains(placementId))
                return false;
            lastShownPlacement = placementId;
            ShowGUI().AdResult += HandleAdResult;
            fetchedPlacements.Remove(placementId);
            return true;
        }

        public virtual void Fetch()
        {
            lastFetchedPlacement = GetPlacementId();
            fetchedPlacements.Add(lastFetchedPlacement);
        }

        public virtual void Fetch(string placementId)
        {
            lastFetchedPlacement = placementId;
            fetchedPlacements.Add(placementId);
        }

        protected abstract void OnAdResult(AdResult adResult);

        EditorAdsGUI ShowGUI()
        {
            editorAdsGuiObject = new GameObject("EditorAds");
            return editorAdsGuiObject.AddComponent<EditorAdsGUI>();
        }

        void HandleAdResult(AdResult adResult)
        {
            OnAdResult(adResult);
            Object.Destroy(editorAdsGuiObject);
            editorAdsGuiObject = null;
            lastShownPlacement = string.Empty;
        }

        string GetPlacementId()
        {
            var adPlacementData =
                HConfigs.GetConfigsByBaseClass<AdsProviderConfig>().FirstOrDefault()?.AdPlacementData;
            var placement = adPlacementData?.FirstOrDefault(x => x.PlacementType == placementType);
            return placement != null ? placement.PlacementId : string.Empty;
        }
    }
}