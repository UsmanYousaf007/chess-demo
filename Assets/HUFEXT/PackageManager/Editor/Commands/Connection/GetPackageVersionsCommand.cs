using System;
using System.Linq;
using UnityEditor;

namespace HUFEXT.PackageManager.Editor.Commands.Connection
{
    [Serializable]
    public class Version
    {
        public string version = string.Empty;
    }
    
    public class GetPackageVersionsCommand : Core.Command.Base
    {
        public Models.PackageManifest manifest;
        
        bool previewCompleted = false;
        bool stableCompleted = false;
        bool experimentalCompleted = false;
        
        public override void Execute()
        {
            if ( Core.Packages.Channel == Models.PackageChannel.Development )
            {
                Complete( false, "Unable to fetch versions from development channel." );
                return;
            }
            
            EditorApplication.update += WaitForFinish;
            FetchPackageVersions( Models.Keys.Routing.PREVIEW_CHANNEL );
            FetchPackageVersions( Models.Keys.Routing.EXPERIMENTAL_CHANNEL );
            FetchPackageVersions( Models.Keys.Routing.STABLE_CHANNEL );
        }

        void FetchPackageVersions( string channel )
        {
            string route = Core.Request.CreateRoute( Models.Keys.Routing.API.PACKAGE_VERSIONS )
                                    .Set( Models.Keys.Routing.Tag.SCOPE, manifest.huf.scope )
                                    .Set( Models.Keys.Routing.Tag.CHANNEL, channel )
                                    .Set( Models.Keys.Routing.Tag.PACKAGE, manifest.name )
                                    .Value;

            var request = new Core.Request( route, ( response ) =>
            {
                if ( channel == Models.Keys.Routing.PREVIEW_CHANNEL )
                {
                    previewCompleted = true;
                }
                else if ( channel == Models.Keys.Routing.EXPERIMENTAL_CHANNEL )
                {
                    experimentalCompleted = true;
                }
                else
                {
                    stableCompleted = true;
                }
                
                if ( response.status == Core.RequestStatus.Failure )
                { 
                    EditorApplication.update -= WaitForFinish;
                    Complete( false, $"Unable to fetch package version from {channel}. Response is failure." );
                    return;
                }
                
                var versions = Utils.Common.FromJsonToArray<Version>( response.text );

                if ( channel == Models.Keys.Routing.PREVIEW_CHANNEL )
                {
                    manifest.huf.config.previewVersions = versions.Select( v => v.version ).ToList();
                }
                else if ( channel == Models.Keys.Routing.EXPERIMENTAL_CHANNEL )
                {
                    manifest.huf.config.experimentalVersions = versions.Select( v => v.version ).ToList();
                }
                else
                {
                    manifest.huf.config.stableVersions = versions.Select( v => v.version ).ToList();
                }
            } );
            
            request.Send();
        }
        
        void WaitForFinish()
        {
            if ( previewCompleted && stableCompleted )
            {
                EditorApplication.update -= WaitForFinish;
                Complete( true, manifest.ToString() );
            }
        }
    }
}
