using UnityEngine;
using UnityEngine.Serialization;

namespace HUFEXT.PackageManager.Editor.Models
{
    public enum PackageChannel
    {
        Stable,
        Preview,
        Experimental,
        Development
    }

    public enum PackageSortingType
    {
        AllPackages,
        InProject,
        UpdateAvailable,
        PreviewPackages
    }

    public enum PackageCategoryType
    {
        All,
        HUF,
        HUFEXT,
        SDK,
        UNITY
    }

    [System.Serializable]
    public class PackageManagerState
    {
        public string developerId;
        public PackageSortingType sortingType = PackageSortingType.AllPackages;
        public PackageCategoryType categoryType = PackageCategoryType.All;
        public PackageChannel channel = PackageChannel.Stable;
        public PackageManifest selectedPackage = null;
        public PackageManifest originalSelectedPackage = null;
        public bool showPreviewPackages = true;
        public bool showUnityPackages = true;
        public bool ignoreVersionTags = false;
        public bool enableDebugLogs = false;
        public string lastFetchDate;

        [System.NonSerialized] public string searchText = string.Empty;

        public void Load()
        {
            Core.Registry.Load( Keys.CACHE_PACKAGE_MANAGER_STATE_KEY, this, Core.CachePolicy.Prefs );
            channel = Core.Packages.Channel;
            developerId = Token.ID;
        }

        public void Save()
        {
            Core.Packages.Channel = channel;
            Core.Registry.Save( Keys.CACHE_PACKAGE_MANAGER_STATE_KEY, this, Core.CachePolicy.Prefs );
        }
    }
}