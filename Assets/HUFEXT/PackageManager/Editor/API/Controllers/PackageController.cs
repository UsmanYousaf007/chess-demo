using System;
using System.Collections.Generic;
using HUFEXT.PackageManager.Editor.API.Data;
using HUFEXT.PackageManager.Editor.API.Installers;
using HUFEXT.PackageManager.Editor.Implementation.Local.Services;
using HUFEXT.PackageManager.Editor.Implementation.Remote.Auth;
using HUFEXT.PackageManager.Editor.Implementation.Remote.Data;
using HUFEXT.PackageManager.Editor.Implementation.Remote.Services;
using HUFEXT.PackageManager.Editor.Utils;
using HUFEXT.PackageManager.Editor.Utils.Helpers;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Cache = HUFEXT.PackageManager.Editor.Utils.Cache;

namespace HUFEXT.PackageManager.Editor.API.Controllers
{
    [System.Serializable]
    public class PackageController
    {
        public enum UpdateType
        {
            Local,
            Remote
        }
        
        [Flags] 
        public enum RefreshType
        {
            FetchLocal  = 1,
            FetchRemote = 2,
            FetchAll    = 4,
            Clear       = 8
        }

        [SerializeField]
        private List<PackageManifest> packages;

        private RemotePackageService remoteService;

        public List<PackageManifest> Packages => packages ?? ( packages = new List<PackageManifest>() );
        
        public event UnityAction<UpdateType> OnPackageListChanged;
        
        public static PackageController CreateOrLoadFromCache()
        {
            var controller = new PackageController();
            Cache.LoadFromCache( controller, Registry.Keys.PACKAGES_CACHE_LIST_KEY );
            return controller;
        }

        private PackageController() {}
        
        public void InstallPackageRequest( PackageManifest manifest )
        {
            if ( remoteService == null )
            {
                remoteService = new RemotePackageService( Token.LoadExistingToken() );
            }
            remoteService.RequestPackageDownload( manifest, () =>
            {
                PackageInstaller.InstallPackage( manifest );
                Refresh();
            } );
        }

        public void RemovePackageRequest( string packageName )
        {
            PackageInstaller.RemovePackage( packageName );
            Refresh();
        }

        public void UpdatePackagesRequest()
        {
            EditorApplication.update += UpdateNextPackageRequest;
        }

        public void UpdateNextPackageRequest()
        {
            if ( PackageInstaller.IsLocked )
            {
                return;
            }

            if ( !PlayerPrefs.HasKey( Registry.Keys.PACKAGE_MANAGER_UPDATES ) )
            {
                EditorApplication.update -= UpdateNextPackageRequest;
                return;
            }

            var str = PlayerPrefs.GetString( Registry.Keys.PACKAGE_MANAGER_UPDATES, string.Empty );
            var toUpdate = new PackageUpdateList();
            EditorJsonUtility.FromJsonOverwrite( str, toUpdate );

            if ( toUpdate.Items.Count > 0 )
            {
                PackageInstaller.Lock();
                InstallPackageRequest( toUpdate.Items[0] );
                toUpdate.Items.RemoveAt( 0 );
                PlayerPrefs.SetString( Registry.Keys.PACKAGE_MANAGER_UPDATES, EditorJsonUtility.ToJson( toUpdate ) );
            }
            else
            {
                PlayerPrefs.DeleteKey( Registry.Keys.PACKAGE_MANAGER_UPDATES );
            }
        }

        public void RequestPackageManifest( Data.PackageConfig config, UnityAction<PackageManifest> onComplete, string version = "" )
        {
            if (remoteService == null)
            {
                remoteService = new RemotePackageService( Token.LoadExistingToken() );
            }
            remoteService.RequestPackageManifest( config, onComplete, version );
        }

        public void FetchLocalPackages()
        {
            new LocalPackagesService().RequestPackagesList( string.Empty, MergeLocalPackages );
        }

        public void FetchRemotePackages()
        {
            if ( remoteService == null )
            {
                remoteService = new RemotePackageService( Token.LoadExistingToken() );
            }
            remoteService.RequestPackagesList( RoutingScheme.Channel.Stable, MergeRemotePackages );
        }
        
        public void Refresh( RefreshType type = RefreshType.FetchLocal )
        {
            if ( type.HasFlag( RefreshType.Clear ) )
            {
                Packages.Clear();
            }

            if ( type.HasFlag( RefreshType.FetchLocal ) || type.HasFlag( RefreshType.FetchAll ) )
            {
                FetchLocalPackages();
            }

            if ( type.HasFlag( RefreshType.FetchRemote ) || type.HasFlag( RefreshType.FetchAll ) )
            {
                FetchRemotePackages();
            }
        }

        public void RequestPackageVersions( PackageManifest manifest, UnityAction<List<string>> onComplete, string channel = "" )
        {
            if (remoteService == null)
            {
                remoteService = new RemotePackageService( Token.LoadExistingToken() );
            }
            remoteService.RequestPackageVersions( manifest, onComplete, channel );
        }

        private void MergeLocalPackages( List<PackageManifest> toMerge )
        {
            for( int i = 0; i < toMerge.Count; ++i )
            {
                var localIndex = Packages.FindIndex( ( x ) => x.name == toMerge[i].name || x.huf.path == toMerge[i].huf.path );

                if( localIndex == -1 )
                {
                    Packages.Add( toMerge[i] );
                }
                else
                {
                    var config = Packages[localIndex].huf.config;
                    var packageWasLocal = Packages[localIndex].huf.isLocal;

                    var currentPackageStatus = Packages[localIndex].huf.status;
                    bool packageIsNotInstalled = currentPackageStatus == PackageStatus.NotInstalled;
                    bool localPackageIsNewer = toMerge[i].IsNewer( Packages[localIndex].version );
                    
                    if( packageIsNotInstalled || localPackageIsNewer )
                    {
                        Packages[localIndex] = toMerge[i];
                    }
                    
                    Packages[localIndex].huf.isLocal = packageWasLocal;
                    Packages[localIndex].huf.config = config;
                }
            }

            // Remove packages that was uninstalled.
            for ( int i = 0; i < Packages.Count; ++i )
            {
                var index = toMerge.FindIndex( ( x ) => x.name == Packages[i].name || x.huf.path == Packages[i].huf.path );

                if ( Packages[i].huf.status == PackageStatus.Installed && index == -1 )
                {
                    Packages[i].huf.status = PackageStatus.NotInstalled;
                }
            }
            
            RemoveUninstalledPackages();

            Cache.SaveInCache( Registry.Keys.PACKAGES_CACHE_LIST_KEY, this );
            OnPackageListChanged?.Invoke( UpdateType.Local );
        }
        
        private void MergeRemotePackages( List<PackageManifest> toMerge )
        {
            for( int i = 0; i < toMerge.Count; ++i )
            {
                // Search for local package with same name or path...
                var localIndex = Packages.FindIndex( ( x ) => x.name == toMerge[i].name || x.huf.path == toMerge[i].huf.path );

                // If remote package is not found locally...
                if( localIndex == -1 )
                {
                    toMerge[i].huf.status = PackageStatus.NotInstalled;
                    toMerge[i].huf.isLocal = false;
                    Packages.Add( toMerge[i] );
                }
                else
                {
                    bool namesAreEqual = Packages[localIndex].name == toMerge[i].name;
                    bool pathsAreEqual = Packages[localIndex].huf.path == toMerge[i].huf.path;

                    // Apply config from remote to local package to check if package should be updated.
                    Packages[localIndex].huf.config = toMerge[i].huf.config;
                    Packages[localIndex].huf.scope = toMerge[i].huf.scope;
                    Packages[localIndex].huf.channel = toMerge[i].huf.channel;
                    Packages[localIndex].huf.isLocal = false;
                    
                    // If package name has conflict but package should be installed in same path,
                    // package probably need migration because installed version has no manifest.
                    if ( !namesAreEqual && pathsAreEqual )
                    {
                        Packages[localIndex].name = toMerge[i].name;
                        Packages[localIndex].huf.status = PackageStatus.Migration;
                        continue;
                    } 
                    else if ( namesAreEqual && !pathsAreEqual )
                    {
                        Packages[localIndex].huf.status = PackageStatus.Conflict;
                        continue;
                    }
                    
                    if ( Packages[localIndex].IsOlder( toMerge[i].version ) )
                    {
                        Packages[localIndex].huf.status = PackageStatus.UpdateAvailable;

                        if ( Packages[localIndex].IsOlder( toMerge[i].huf.config.minimumVersion ) )
                        {
                            Packages[localIndex].huf.status = PackageStatus.ForceUpdate;
                        }
                    }
                }
            }

            RemoveUninstalledPackages();

            Cache.SaveInCache( Registry.Keys.PACKAGES_CACHE_LIST_KEY, this );
            OnPackageListChanged?.Invoke( UpdateType.Remote );
        }
        
        private void RemoveUninstalledPackages()
        {
            Packages.RemoveAll( package => package.huf.status == PackageStatus.NotInstalled && package.huf.isLocal );
        }
    }
}
