using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Commands.Connection
{
    //CACHE_PACKAGE_KEY
    public class DownloadPackageManifestCommand : Core.Command.Base
    {
        public string scope = string.Empty;
        public string channel = string.Empty;
        public string packageName = string.Empty;
        public string version = string.Empty;

        Models.PackageManifest manifest;

        public override void Execute()
        {
            Utils.Common.ShowDownloadProgress( $"manifest for {packageName}" );

            var route = Core.Request.CreateRoute( Models.Keys.Routing.API.PACKAGE_MANIFEST )
                .Set( Models.Keys.Routing.Tag.SCOPE, scope )
                .Set( Models.Keys.Routing.Tag.CHANNEL, channel )
                .Set( Models.Keys.Routing.Tag.PACKAGE, packageName )
                .Set( Models.Keys.Routing.Tag.VERSION, version )
                .Value;
            var request = new Core.Request( route, OnPackageManifestDownloaded );
            request.Send();
        }

        void OnPackageManifestDownloaded( Core.WebResponse response )
        {
            if ( response.status != Core.RequestStatus.Success )
            {
                Complete( false, $"Unable to download manifest for {packageName}. Response is failure." );
                EditorUtility.ClearProgressBar();
                return;
            }

            Utils.Common.ShowDownloadProgress( $"manifest for {packageName}", 0.5f );
            manifest = Models.PackageManifest.ParseManifest( response.text, true );
            manifest.huf.status = Models.PackageStatus.NotInstalled;
            manifest.huf.config.latestVersion = version;
            Utils.Common.ShowDownloadProgress( $"config for {packageName}", 0.75f );

            var route = Core.Request.CreateRoute( Models.Keys.Routing.API.PACKAGE_CONFIG )
                .Set( Models.Keys.Routing.Tag.SCOPE, scope )
                .Set( Models.Keys.Routing.Tag.CHANNEL, channel )
                .Set( Models.Keys.Routing.Tag.PACKAGE, packageName )
                .Value;
            var request = new Core.Request( route, OnPackageConfigDownloaded );
            request.Send();
        }

        void OnPackageConfigDownloaded( Core.WebResponse response )
        {
            if ( response.status != Core.RequestStatus.Success )
            {
                Complete( false, $"Unable to download config for {packageName}. Response is failure." );
                EditorUtility.ClearProgressBar();
                return;
            }

            EditorJsonUtility.FromJsonOverwrite( response.text, manifest.huf.config );
            Complete( true, manifest.ToString() );
            EditorUtility.ClearProgressBar();
        }
    }
}