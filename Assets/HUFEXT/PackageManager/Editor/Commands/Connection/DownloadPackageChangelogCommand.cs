using System;
using HUFEXT.PackageManager.Editor.Models;
using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Commands.Connection
{
    public class DownloadPackageChangelogCommand : Core.Command.Base
    {
        const string NOT_FOUND_RESPONSE = "Not found";

        public string scope = string.Empty;
        public string channel = string.Empty;
        public string packageName = string.Empty;
        public string version = string.Empty;
        public PackageManifest package = null;
        public bool silent = false;

        public override void Execute()
        {
            if ( package != null )
            {
                if ( string.IsNullOrEmpty( scope ) )
                    scope = package.huf.scope;

                if ( string.IsNullOrEmpty( channel ) )
                    channel = package.huf.channel;

                if ( string.IsNullOrEmpty( packageName ) )
                    packageName = package.name;

                if ( string.IsNullOrEmpty( version ) )
                    version = package.version;
            }

            if ( !silent )
                Utils.Common.ShowDownloadProgress( $"changelog for {packageName}" );

            var route = Core.Request.CreateRoute( Models.Keys.Routing.API.PACKAGE_CHANGELOG )
                .Set( Models.Keys.Routing.Tag.SCOPE, scope )
                .Set( Models.Keys.Routing.Tag.CHANNEL, channel )
                .Set( Models.Keys.Routing.Tag.PACKAGE, packageName )
                .Set( Models.Keys.Routing.Tag.VERSION, version )
                .Value;
            var request = new Core.Request( route, OnPackageChangelogDownloaded );
            request.Send();
        }

        void OnPackageChangelogDownloaded( Core.WebResponse response )
        {
            if ( !silent )
                EditorUtility.ClearProgressBar();

            if ( response.status != Core.RequestStatus.Success )
            {
                if ( response.text == NOT_FOUND_RESPONSE )
                    Complete( true, string.Empty );
                else
                    Complete( false, $"Unable to download changelog for {packageName}. Response is failure." );
                return;
            }

            Complete( true, response.text );
        }
    }
}