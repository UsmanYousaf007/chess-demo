using System.Collections.Generic;
using System.IO;

namespace HUFEXT.PackageManager.Editor.Core
{
    // Wrapper for easy access to packages data.
    public static class Packages
    {
        public static bool Locked => File.Exists( Models.Keys.FILE_PACKAGE_LOCK );
        public static bool Empty => !Registry.IsSet( Models.Keys.CACHE_PACKAGE_REGISTRY_KEY );

        public static bool Installing
        {
            get => Registry.IsSet( Models.Keys.CACHE_PACKAGE_INSTALL_IN_PROGRESS );

            set
            {
                if ( value )
                {
                    Registry.Push( Models.Keys.CACHE_PACKAGE_INSTALL_IN_PROGRESS );
                }
                else
                {
                    Registry.Pop( Models.Keys.CACHE_PACKAGE_INSTALL_IN_PROGRESS );
                }
            }
        }
        
        public static bool UpdateInProgress
        {
            get => Registry.IsSet( Models.Keys.CACHE_PACKAGES_UPDATE_IN_PROGRESS );
            
            set
            {
                if ( value )
                {
                    Registry.Push( Models.Keys.CACHE_PACKAGES_UPDATE_IN_PROGRESS );
                }
                else
                {
                    Registry.Pop( Models.Keys.CACHE_PACKAGES_UPDATE_IN_PROGRESS );
                }
            }
        }

        public static Models.PackageChannel Channel
        {
            get
            {
                Registry.Load( Models.Keys.CACHE_PACKAGES_CURRENT_CHANNEL, out int channel );
                return ( Models.PackageChannel ) channel;
            }

            set => Registry.Save( Models.Keys.CACHE_PACKAGES_CURRENT_CHANNEL, ( int ) value );
        }

        public static void RemoveLock()
        {
            Registry.Remove( Models.Keys.FILE_PACKAGE_LOCK );
            Utils.Common.Log( "Package lock removed." );
        }

        public static List<Models.PackageManifest> Data
        {
            set => Registry.Save( Models.Keys.CACHE_PACKAGE_REGISTRY_KEY,
                                  new Utils.Common.Wrapper<Models.PackageManifest> { Items = value },
                                  CachePolicy.Prefs );

            get => Registry.Get<Utils.Common.Wrapper<Models.PackageManifest>>( Models.Keys.CACHE_PACKAGE_REGISTRY_KEY,
                                                                               CachePolicy.Prefs ).Items;
        }

        public static List<Models.PackageManifest> Local
        {
            set => Registry.Save( Models.Keys.CACHE_PACKAGE_LOCAL_REGISTRY_KEY,
                                  new Utils.Common.Wrapper<Models.PackageManifest> { Items = value },
                                  CachePolicy.Prefs );

            get => Registry.Get<Utils.Common.Wrapper<Models.PackageManifest>>( Models.Keys.CACHE_PACKAGE_LOCAL_REGISTRY_KEY,
                                                                               CachePolicy.Prefs ).Items;
        }
        
        public static List<Models.PackageManifest> Remote
        {
            set => Registry.Save( Models.Keys.CACHE_PACKAGE_REMOTE_REGISTRY_KEY,
                                  new Utils.Common.Wrapper<Models.PackageManifest> { Items = value },
                                  CachePolicy.Prefs );

            get => Registry
                   .Get<Utils.Common.Wrapper<Models.PackageManifest>>( Models.Keys.CACHE_PACKAGE_REMOTE_REGISTRY_KEY,
                                                                       CachePolicy.Prefs ).Items;
        }
        
        public static List<Models.PackageManifest> Unity
        {
            set => Registry.Save( Models.Keys.CACHE_PACKAGE_UNITY_REGISTRY_KEY,
                new Utils.Common.Wrapper<Models.PackageManifest> { Items = value },
                CachePolicy.Prefs );

            get => Registry.Get<Utils.Common.Wrapper<Models.PackageManifest>>( Models.Keys.CACHE_PACKAGE_UNITY_REGISTRY_KEY,
                CachePolicy.Prefs ).Items;
        }
        
        public static void ClearLocalData()
        {
            Registry.Remove( Models.Keys.CACHE_PACKAGE_LOCAL_REGISTRY_KEY );
        }
        
        public static void ClearRemoteData()
        {
            Registry.Remove( Models.Keys.CACHE_PACKAGE_REMOTE_REGISTRY_KEY );
        }
        
        public static void ClearUnityData()
        {
            Registry.Remove( Models.Keys.CACHE_PACKAGE_REMOTE_REGISTRY_KEY );
        }
        
        public static void Clear()
        {
            Registry.Remove( Models.Keys.CACHE_PACKAGE_REGISTRY_KEY );
        }

        public static void ClearAll()
        {
            ClearLocalData();
            ClearRemoteData();
            ClearUnityData();
            Clear();
        }
    }
}
