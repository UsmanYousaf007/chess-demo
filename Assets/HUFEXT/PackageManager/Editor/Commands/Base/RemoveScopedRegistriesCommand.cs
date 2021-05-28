using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HUFEXT.PackageManager.Editor.Models;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Commands.Base
{
    public class RemoveScopedRegistriesCommand : Core.Command.Base
    {
        const string DEPENDENCIES_PATTERN = @"""dependencies"":";
        
        public override void Execute()
        {
            var path = Path.Combine( Directory.GetParent( Application.dataPath ).FullName,
                Models.Keys.Filesystem.UNITY_PACKAGES_MANIFEST_FILE );

            if ( !File.Exists( path ) )
            {
                Complete( false );
            }

            var rawManifest = File.ReadAllText( path );
            var start = rawManifest.IndexOf( DEPENDENCIES_PATTERN, StringComparison.Ordinal );
            var end = rawManifest.IndexOf( '}', start );
            var length = end - start + 1;

            if ( length <= 0 || length >= rawManifest.Length )
            {
                Complete( false );
            }
            
            var manifest = new ManifestWrapper();
            JsonUtility.FromJsonOverwrite( rawManifest, manifest );
            
            var dependencies = rawManifest.Substring( start, length );
            var dependenciesArray = dependencies.Split( '\n' );
            dependencies = string.Join("\n",  dependenciesArray.Where( d => !d.Contains( "\"com.google." ) ));

            int index = manifest.scopedRegistries.FindIndex( r => r.url == Keys.GOOGLE_SCOPED_REGISTRY_KEY );
            if ( index >= 0 )
            {
               manifest.scopedRegistries.RemoveAt( index );
            }

            var json = JsonUtility.ToJson( manifest, true );
            File.WriteAllText( path, json.Replace( $"{DEPENDENCIES_PATTERN} \"\"", dependencies ) );

            Complete( true );
        }

        protected override void Complete( bool result, string serializedData = "" )
        {
            if ( result )
            {
                Debug.Log( "Scoped registries removed." );
            }

            base.Complete( result, serializedData );
        }

        [Serializable]
        internal class ManifestWrapper
        {
            public string dependencies = "";
            public List<Models.ScopedRegistryWrapper> scopedRegistries = new List<Models.ScopedRegistryWrapper>();
        }
    }
}