using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Commands.Base
{
    public class AddScopedRegistriesCommand : Core.Command.Base
    {
        const string DEPENDENCIES_PATTERN = @"""dependencies"":";

        public List<Models.ScopedRegistryWrapper> registries;

        public override void Execute()
        {
            if ( registries == null )
            {
                Complete( false );
                return;
            }

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

            var dependencies = rawManifest.Substring( start, length );
            var manifest = new ManifestWrapper();
            JsonUtility.FromJsonOverwrite( rawManifest, manifest );

            foreach ( var reg in registries )
            {
                if ( !manifest.scopedRegistries.Exists( r => r.url == reg.url ) )
                {
                    manifest.scopedRegistries.Add( reg );
                    var json = JsonUtility.ToJson( manifest, true );
                    File.WriteAllText( path, json.Replace( $"{DEPENDENCIES_PATTERN} \"\"", dependencies ) );
                }
            }

            Complete( true );
        }

        protected override void Complete( bool result, string serializedData = "" )
        {
            if ( result )
            {
                Debug.Log( "Scoped registries added." );
            }

            base.Complete( result, serializedData );
        }

        [Serializable]
        internal class ManifestWrapper
        {
            public string dependencies = string.Empty;
            public List<Models.ScopedRegistryWrapper> scopedRegistries = new List<Models.ScopedRegistryWrapper>();
        }
    }
}