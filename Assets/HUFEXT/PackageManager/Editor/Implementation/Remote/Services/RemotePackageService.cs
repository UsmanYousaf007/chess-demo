using HUFEXT.PackageManager.Editor.API.Data;
using HUFEXT.PackageManager.Editor.API.Services;
using HUFEXT.PackageManager.Editor.Utils;
using HUFEXT.PackageManager.Editor.Utils.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HUFEXT.PackageManager.Editor.Implementation.Remote.Auth;
using HUFEXT.PackageManager.Editor.Implementation.Remote.Data;
using HUFEXT.PackageManager.Editor.Implementation.Remote.Requests;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using PackageConfig = HUFEXT.PackageManager.Editor.API.Data.PackageConfig;

namespace HUFEXT.PackageManager.Editor.Implementation.Remote.Services
{
    public class RemotePackageService : RemotePackageServiceBase, IPackageService
    {
        private readonly Token token;

        public RemotePackageService( Token token )
        {
            this.token = token;
        }
        
        public void RequestPackagesList( string channel, UnityAction<List<PackageManifest>> onComplete )
        {
            var route = RoutingScheme.CreateRoute( RoutingScheme.API.LatestPackagesList )
                                     .Set( RoutingScheme.Channel.Tag, channel )
                                     .Value;
            
            EnqueueRequest( new BaseRequest( route, ( response ) =>
            {
                if ( response.status == RequestStatus.Failure )
                {
                    Debug.LogError( "[PackageManager] Unable to fetch list of packages: " + response.handler.text );
                    return;
                }

                var latest = new ManifestList();
                var manifests = new List<PackageManifest>();

                latest.FromJson( response.Text );
                foreach ( var latestItem in latest.Items )
                {
                    latestItem.ParseVersion();
                    manifests.Add( latestItem );
                }
                onComplete?.Invoke( manifests );
            } ) );
        }

        public void RequestPackageConfig( PackageManifest manifest, UnityAction<API.Data.PackageConfig> onComplete )
        {
            if (token == null || !token.IsValid)
            {
                return;
            }

            var route = RoutingScheme.CreateRoute( RoutingScheme.API.PackageConfig )
                                         .Set( RoutingScheme.Scope.Tag, manifest.huf.scope )
                                         .Set( RoutingScheme.Channel.Tag, manifest.huf.channel )
                                         .Set( RoutingScheme.Package.Tag, manifest.name )
                                         .Value;

            var request = new BaseRequest( route, ( response ) =>
            {
                if (response.status == RequestStatus.Failure)
                {
                    Debug.LogError( $"[PackageManager] Unable to fetch package config: {manifest.name}" );
                    return;
                }

                var config = new API.Data.PackageConfig();
                EditorJsonUtility.FromJsonOverwrite( response.Text, config );
                onComplete?.Invoke( config );
            } );

            EnqueueRequest( request );
        }

        public void RequestPackageManifest( API.Data.PackageConfig config, UnityAction<PackageManifest> onComplete, string version = "", string scope = "" )
        {
            var route = RoutingScheme.CreateRoute( RoutingScheme.API.PackageManifest )
                                     .Set( RoutingScheme.Scope.Tag, scope.Length == 0 ? RoutingScheme.Scope.Public : scope )
                                     .Set( RoutingScheme.Channel.Tag, RoutingScheme.Channel.Preview )
                                     .Set( RoutingScheme.Package.Tag, config.packageName )
                                     .Set( RoutingScheme.Version.Tag, version.Length == 0 ? config.latestVersion : version )
                                     .Value;

            EnqueueRequest( new BaseRequest( route, ( response ) =>
            {
                if (response.status == RequestStatus.Failure)
                {
                    return;
                }

                var manifest = PackageManifest.ParseManifest( response.Text, true );
                onComplete?.Invoke( manifest );
            } ) );
        }

        public void RequestPackageDownload( PackageManifest manifest, UnityAction onComplete )
        {
            if (token == null || !token.IsValid)
            {
                return;
            }

            var route = RoutingScheme.CreateRoute( RoutingScheme.API.UnityPackageLink )
                                     .Set( RoutingScheme.Scope.Tag, manifest.huf.scope )
                                     .Set( RoutingScheme.Channel.Tag, manifest.huf.channel )
                                     .Set( RoutingScheme.Package.Tag, manifest.name )
                                     .Set( RoutingScheme.Version.Tag, manifest.huf.config.latestVersion )
                                     .Value;

            EnqueueRequest( new BaseRequest( route, ( response ) =>
            {
                if (response.status == RequestStatus.Failure)
                {
                    Debug.LogError( $"[PackageManager] Unable to download package {manifest.name}" );
                    return;
                }
                var link = new Link();
                EditorJsonUtility.FromJsonOverwrite( response.Text, link );
                DownloadPackage( manifest.name, link.url, onComplete );
            } ) );

            EditorUtility.DisplayProgressBar( "Downloading", "Downloading package " + manifest.name, 0f );
        }

        private void DownloadPackage( string name, string url, UnityAction onComplete )
        {
            EditorUtility.DisplayProgressBar( "Downloading", "Downloading package " + name, 0.5f );
            EnqueueRequest( new BaseRequest( url, ( packageResponse ) =>
            {
                if (!Directory.Exists( Registry.Cache.CACHE_DIRECTORY ))
                {
                    Directory.CreateDirectory( Registry.Cache.CACHE_DIRECTORY );
                }

                var filePath = Registry.Cache.CACHE_DIRECTORY + "/" + name + ".unitypackage";
                File.WriteAllBytes( filePath, packageResponse.Bytes );
                onComplete?.Invoke();
                EditorUtility.ClearProgressBar();
            } ) );
        }

        public void RequestPackageVersions( PackageManifest manifest, UnityAction<List<string>> onComplete, string channel = "" )
        {
            if( string.IsNullOrEmpty( channel ) )
            {
                channel = manifest.huf.channel;
            }

            string route = RoutingScheme.CreateRoute( RoutingScheme.API.PackageVersions )
                                    .Set( RoutingScheme.Scope.Tag, manifest.huf.scope )
                                    .Set( RoutingScheme.Channel.Tag, channel )
                                    .Set( RoutingScheme.Package.Tag, manifest.name )
                                    .Value;

            EnqueueRequest( new BaseRequest( route, ( response ) =>
            {
                if ( response.status == RequestStatus.Failure )
                { 
                    return;
                }
                var versions = new VersionList();
                versions.FromJson( response.Text );
                onComplete?.Invoke( versions.Items.Select( v => v.version).ToList() );
            } ) );
        }
    }
}
