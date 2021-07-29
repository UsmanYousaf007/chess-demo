using UnityEditor;

namespace HUFEXT.PackageManager.Editor.Commands.Processing
{
    // This method will add resolved dependencies to package-lock file.
    public class PackageLockCommand : Core.Command.Base
    {
        public override void Execute()
        {
            if ( data == string.Empty || !lastResult )
            {
                Complete( false, "Unable to find resolved dependencies." );
                return;
            }

            var locked = Core.Registry.Get<Models.Dependencies>( Models.Keys.FILE_PACKAGE_LOCK, Core.CachePolicy.File );
            var dependencies = Utils.Common.FromJsonToArray<Models.Dependency>( data );

            foreach ( var dependency in dependencies )
            {
                var index = locked.Items.FindIndex( ( d ) => d.name == dependency.name );

                if ( index == -1 )
                {
                    locked.Items.Add( dependency );
                    continue;
                }

                if ( dependency.IsVersionHigherTo( locked.Items[index] ) )
                {
                    locked.Items[index] = dependency;
                }
            }

            Core.Registry.Save( Models.Keys.FILE_PACKAGE_LOCK, locked, Core.CachePolicy.File );
            Utils.Common.Log( $"Packages locked: {EditorJsonUtility.ToJson( locked, true )}" );
            Complete( true );
        }
    }
}