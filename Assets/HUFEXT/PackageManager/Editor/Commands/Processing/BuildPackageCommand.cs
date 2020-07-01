using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Commands.Processing
{
    public class BuildPackageCommand : Core.Command.Base
    {
        public Models.PackageManifest manifest;
        
        public override void Execute()
        {
            if ( !manifest.IsInstalled && !manifest.IsRepository )
            {
                Complete( false );
                return;
            }

            var temp = EditorUtility.OpenFolderPanel( 
                $"Select build directory for {manifest.name}-{manifest.version}", 
                Application.dataPath, 
                $"{manifest.name}-{manifest.version}.unitypackage" );

            if ( !string.IsNullOrEmpty( temp ) )
            {
                Debug.Log( "Exporting: " + manifest );
                AssetDatabase.ExportPackage( 
                    manifest.huf.path, 
                    $"{temp}/{manifest.name}-{manifest.version}.unitypackage", 
                    ExportPackageOptions.Recurse );
                Complete( true );
                return;
            }
            
            Complete( false );
        }
    }
}
