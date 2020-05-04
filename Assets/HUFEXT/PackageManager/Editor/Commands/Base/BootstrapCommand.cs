using UnityEditor;

namespace HUFEXT.PackageManager.Editor.Commands.Base
{
    class BootstrapCommand : Core.Command.Base
    {
        public override void Execute()
        {
            // Process packages only if signed authorization token exist.
            if ( !Models.Token.Exists )
            {
                Complete( false );
                return;
            }
            
            Core.Registry.Load( Models.Keys.PACKAGE_MANAGER_NEXT_AUTO_FETCH, out int next );
            if ( Core.Packages.Empty || Utils.Common.GetTimestamp() >= next )
            {
                Core.Command.Execute( new Processing.RefreshPackagesCommand() );
            }

            Complete( true );
        }
    }
}