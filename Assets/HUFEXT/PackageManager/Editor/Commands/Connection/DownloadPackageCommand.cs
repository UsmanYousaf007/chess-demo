using System;
using System.IO;
using UnityEditor;

namespace HUFEXT.PackageManager.Editor.Commands.Connection
{
    [Serializable]
    public class Link
    {
        public string url = string.Empty;
    }
    
    public class DownloadPackageCommand : Core.Command.Base
    {
        public string scope;
        public string channel;
        public string packageName;
        public string version;

        public override void Execute()
        {
            if ( Core.Packages.Channel == Models.PackageChannel.Development )
            {
                CopyPackageFromDevelopmentEnvironment();
            }
            else
            {
                SendDownloadRequest();
            }
        }

        protected override void Complete( bool result, string serializedData = "" )
        {
            Utils.Common.Log( serializedData );
            base.Complete( result, serializedData );
        }

        void CopyPackageFromDevelopmentEnvironment()
        {
            Core.Registry.Load( Models.Keys.PACKAGE_MANAGER_DEV_ENVIRONMENT, out string devEnvPath );

            if ( devEnvPath == string.Empty )
            {
                Complete( false, "Environment path is empty." );
                return;
            }
            
            try
            {
                var packagePath = Path.Combine( devEnvPath, packageName ) + "/";
                var files = Directory.GetFiles( packagePath, "*.unitypackage", SearchOption.TopDirectoryOnly );
                if ( files.Length != 1 )
                {
                    Complete( false, $"Package {packageName} doesn't exists." );
                    return;
                }
                
                if ( !Directory.Exists( Models.Keys.CACHE_DIRECTORY ) )
                {
                    Directory.CreateDirectory( Models.Keys.CACHE_DIRECTORY );
                }

                File.Copy( files[0],
                    Path.Combine( Models.Keys.CACHE_DIRECTORY,
                        packageName + Models.Keys.Filesystem.UNITY_PACKAGE_EXTENSION ) );
                
                Complete( true );
            }
            catch ( Exception )
            {
                Complete( false, $"Package {packageName} doesn't exists." );
            }
        }
        
        void SendDownloadRequest()
        {
            var route = Core.Request.CreateRoute( Models.Keys.Routing.API.PACKAGE_LINK )
                            .Set( Models.Keys.Routing.Tag.SCOPE, scope )
                            .Set( Models.Keys.Routing.Tag.CHANNEL, channel )
                            .Set( Models.Keys.Routing.Tag.PACKAGE, packageName )
                            .Set( Models.Keys.Routing.Tag.VERSION, version )
                            .Value;

            Utils.Common.ShowDownloadProgress( $"{packageName}@{version}", 0.3f );
            
            var request = new Core.Request( route, ( response ) =>
            {
                if ( response.status == Core.RequestStatus.Failure )
                {
                    Complete( false, $"Unable to  download package {packageName}." );
                    EditorUtility.ClearProgressBar();
                    return;
                }
                var link = new Link();
                EditorJsonUtility.FromJsonOverwrite( response.text, link );
                DownloadPackage( packageName, link.url );
                Utils.Common.ShowDownloadProgress( $"{packageName}@{version}", 0.7f );
            } );

            request.Send();
        }
        
        void DownloadPackage( string name, string url )
        {
            var request = new Core.Request( url, ( packageResponse ) =>
            {
                if ( !Directory.Exists( Models.Keys.CACHE_DIRECTORY ) )
                {
                    Directory.CreateDirectory(Models.Keys.CACHE_DIRECTORY );
                }

                var filePath = Path.Combine( Models.Keys.CACHE_DIRECTORY,
                    $"{name}{Models.Keys.Filesystem.UNITY_PACKAGE_EXTENSION}" );
                
                File.WriteAllBytes( filePath, packageResponse.bytes );
                Complete( File.Exists( filePath ) );
                EditorUtility.ClearProgressBar();
            } );
            
            request.Send();
        }
    }
}
