using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace HUFEXT.PackageManager.Editor.Commands.Connection
{
    public class GetPackageVersionsCommand : Core.Command.Base
    {
        public Models.PackageManifest package;
        int fetchesFinished = 0;
        int fetchesCount = 0;

        public override void Execute()
        {
            if ( Core.Packages.Channel == Models.PackageChannel.Development )
            {
                Complete( false, "Unable to fetch versions from development channel." );
                return;
            }
            EditorApplication.update += WaitForFinish;
            fetchesFinished = 0;
            fetchesCount = package.huf.scopes.Count * 3;

            foreach ( var scope in package.huf.scopes )
            {
                FetchPackageVersions( Models.Keys.Routing.PREVIEW_CHANNEL, scope );
                FetchPackageVersions( Models.Keys.Routing.EXPERIMENTAL_CHANNEL, scope );
                FetchPackageVersions( Models.Keys.Routing.STABLE_CHANNEL, scope );
            }
        }

        void FetchPackageVersions( string channel, string scope )
        {
            string route = Core.Request.CreateRoute( Models.Keys.Routing.API.PACKAGE_VERSIONS )
                .Set( Models.Keys.Routing.Tag.SCOPE, scope )
                .Set( Models.Keys.Routing.Tag.CHANNEL, channel )
                .Set( Models.Keys.Routing.Tag.PACKAGE, package.name )
                .Value;

            var request = new Core.Request( route,
                ( response ) =>
                {
                    fetchesFinished++;

                    if ( response.status != Core.RequestStatus.Success )
                    {
                        Utils.Common.Log( $"Unable to fetch {package.name} version from {channel} on {scope}." );
                        return;
                    }

                    var versions = Utils.Common.FromJsonToArray<Models.Version>( response.text );

                    foreach ( var version in versions )
                        version.scope = scope;

                    if ( channel == Models.Keys.Routing.PREVIEW_CHANNEL )
                        AddToVersionsList( package.huf.config.previewVersions, versions );
                    else if ( channel == Models.Keys.Routing.EXPERIMENTAL_CHANNEL )
                        AddToVersionsList( package.huf.config.experimentalVersions, versions );
                    else
                        AddToVersionsList( package.huf.config.stableVersions, versions );
                } );
            request.Send();

            void AddToVersionsList( List<Models.Version> configList, List<Models.Version> versions )
            {
                configList.Clear();
                foreach ( var version in versions )
                {
                    if ( !configList.Exists( v => v.version == version.version ) )
                        configList.Add( version );
                }
            }
        }

        void WaitForFinish()
        {
            if ( fetchesFinished >= fetchesCount )
            {
                EditorApplication.update -= WaitForFinish;
                Complete( true, package.ToString() );
            }
        }
    }
}