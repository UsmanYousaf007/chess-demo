using System.Collections.Generic;
using HUFEXT.PackageManager.Editor.Models;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using PackageStatus = HUFEXT.PackageManager.Editor.Models.PackageStatus;

namespace HUFEXT.PackageManager.Editor.Commands.Data
{
    public class GetUnityPackagesCommand : Core.Command.Base
    {
        readonly List<Models.PackageManifest> packages = new List<Models.PackageManifest>();
        UnityEditor.PackageManager.Requests.ListRequest unityPackagesRequest;

        public override void Execute()
        {
            unityPackagesRequest = UnityEditor.PackageManager.Client.List( false );
            EditorApplication.update += WaitForUnityResponse;
        }

        protected override void Complete( bool result, string serializedData = "" )
        {
            if ( result )
            {
                Core.Packages.Unity = packages;
            }
            else
            {
                Core.Packages.ClearUnityData();
            }

            EditorApplication.update -= WaitForUnityResponse;

            base.Complete( result,
                PlayerPrefs.GetString( Models.Keys.CACHE_PACKAGE_UNITY_REGISTRY_KEY ) ); // serializedData
        }

        void WaitForUnityResponse()
        {
            if ( unityPackagesRequest == null )
            {
                Complete( false );
                return;
            }

            if ( unityPackagesRequest.IsCompleted )
            {
                switch ( unityPackagesRequest.Status )
                {
                    case StatusCode.Success:
                        ConvertUnityPackageInfo( unityPackagesRequest.Result );
                        Complete( true );
                        break;
                    default:
                        Complete( false );
                        break;
                }
            }
        }

        void ConvertUnityPackageInfo( PackageCollection collection )
        {
            foreach ( var unityPackage in collection )
            {
                var package = new PackageManifest
                {
                    name = unityPackage.name,
                    displayName = unityPackage.displayName,
                    version = unityPackage.version,
                    description = unityPackage.description,
                    huf = new PackageManifest.Metadata
                    {
                        status = PackageStatus.Installed,
                        rollout = Models.Rollout.UNITY_LABEL,
                        isUnity = true,
                    }
                };
                package.ParseVersion();
                packages.Add( package );
            }
        }
    }
}