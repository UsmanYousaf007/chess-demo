using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace HUFEXT.PackageManager.Editor.Commands.Processing
{
    public class RemovePackageCommand : Core.Command.Base
    {
        public string path;
        
        public override void Execute()
        {
            if ( !Directory.Exists( path ) )
            {
                Complete( false, $"Unable to find directory {path}." );
                return;
            }

            UpdateProgress( path, 0f );
            RemovePackageDataInternal();
        }

        protected override void Complete( bool result, string serializedData = "" )
        {
            EditorUtility.ClearProgressBar();
            Core.Packages.Installing = false;
            base.Complete( result, serializedData );
        }

        void RemovePackageDataInternal()
        {
            var dependencies = new List<string>();
            
            foreach ( var file in Directory.GetFiles( path, "dependencies.*" ) )
            {
                if ( !file.Contains( Models.Keys.Filesystem.META_EXTENSION ) )
                {
                    dependencies.AddRange( File.ReadAllLines( file ) );
                }
            }
            
            var currentIndex = 0f;
            var totalCount = ( float ) dependencies.Count;
            
            dependencies.ForEach( filename =>
            {
                UpdateProgress( filename, ++currentIndex / totalCount );
                if ( FileUtil.DeleteFileOrDirectory( filename ) )
                {
                    FileUtil.DeleteFileOrDirectory( Utils.Common.GetMetaPath( filename ) );
                }
            });
            
            if ( path != "Assets" && Directory.Exists( path ) )
            {
                Directory.Delete( path, true );
                FileUtil.DeleteFileOrDirectory( Utils.Common.GetMetaPath( path ) );
            }
            
            Complete( true );
        }

        void UpdateProgress( string file, float progress )
        {
            EditorUtility.DisplayProgressBar( $"HUF", $"Removing file {file}", progress );
        }
    }
}
