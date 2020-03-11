using HUFEXT.PackageManager.Editor.Implementation.Remote.Auth;
using HUFEXT.PackageManager.Editor.Utils;
using UnityEngine.EventSystems;

namespace HUFEXT.PackageManager.Editor.API.Data
{
    [System.Serializable]
    public class PackageManagerState
    {
        public enum SortingType
        {
            AllPackages,
            InProject,
            UpdateAvailable,
            PreviewPackages
        }

        public enum CategoryType
        {
            All,
            HUF,
            HUFEXT
        }
        
        public string developerId = string.Empty;
        public SortingType sortingType;
        public CategoryType categoryType;
        public PackageManifest selectedPackage;
        public bool showPreviewPackages;
        public string lastFetchDate;
        public int nextFetchTimestamp;
        
        private void Initialize()
        {
            var token = Token.LoadExistingToken();
            if ( token != null )
            {
                if ( !token.IsValid )
                {
                    Invalidate();
                    return;
                }

                developerId = token.DeveloperID;
            }

            sortingType = SortingType.AllPackages;
            categoryType = CategoryType.All;
        }

        public void Invalidate()
        {
            Token.LoadExistingToken()?.Invalidate();
            Cache.RemoveFromCache( Registry.Keys.PACKAGE_MANAGER_STATUS );
            developerId = string.Empty;
        }
        
        public void Save()
        {
            Cache.SaveInCache( Registry.Keys.PACKAGE_MANAGER_STATUS, this );
        }
        
        public void Load()
        {
            Cache.LoadFromCache( this, Registry.Keys.PACKAGE_MANAGER_STATUS );
            if ( string.IsNullOrEmpty( developerId ) )
            {
                Initialize();
            }
        }
    }
}