using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Commands.Base
{
    public class AddGoogleScopedRegistryCommand : Core.Command.Base
    {
        [Serializable]
        internal class ScopedRegistryWrapper
        {
            public string name;
            public string url;
            public List<string> scopes = new List<string>();
        }
        
        [Serializable]
        internal class ManifestWrapper
        {
            public List<ScopedRegistryWrapper> scopedRegistries = new List<ScopedRegistryWrapper>();
        }

        private UnityEditor.PackageManager.Requests.SearchRequest request;
        
        public override void Execute()
        {
            var path = Path.Combine( Directory.GetParent( Application.dataPath ).FullName,
                Models.Keys.Filesystem.UNITY_PACKAGES_MANIFEST_FILE );
            
            if ( !File.Exists( path ) )
            {
                Complete( false );
            }

            var googleRegistry = new ScopedRegistryWrapper
            {
                name = "Game Package Registry by Google",
                url = Models.Keys.GOOGLE_SCOPED_REGISTRY_KEY,
                scopes = new List<string>
                {
                    "com.google"
                }
            };

            var rawManifest = File.ReadAllText( path );
            var start = rawManifest.IndexOf( @"""dependencies"":", StringComparison.Ordinal );
            var end = rawManifest.IndexOf( '}', start );
            var length = end - start + 1;

            if ( length <= 0 || length >= rawManifest.Length )
            {
                Complete( false );
            }
            
            var dependencies = rawManifest.Substring( start, length );
            
            var manifest = new ManifestWrapper();
            JsonUtility.FromJsonOverwrite( rawManifest, manifest );

            if ( !manifest.scopedRegistries.Exists( (r) => r.url == googleRegistry.url ) )
            {
                manifest.scopedRegistries.Add( googleRegistry );
                var json = JsonUtility.ToJson( manifest, true );
                File.WriteAllText( path, json.Replace( "\"dependencies\": \"\"", dependencies ) );
            }
            
            Complete( true );
        }
    }
}
