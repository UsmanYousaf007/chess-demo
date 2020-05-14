using UnityEditor;

namespace HUFEXT.PackageManager.Editor.Core
{
    [InitializeOnLoad]
    public class Bootstrap
    {
        const int LOW_LIMIT = 50;
        const int HIGH_LIMIT = 300;
        
        static int lowFrame = 0;
        static int highFrame = 0;
            
        static Bootstrap()
        {
            EditorApplication.delayCall += () => Command.Execute( new Commands.Base.BootstrapCommand() );
            EditorApplication.update += PackageController;
        }

        static void PackageController()
        {
            if ( ++lowFrame > LOW_LIMIT )
            {
                if ( !Command.QueueIsEmpty )
                {
                    Command.FlushQueue();
                }

                Utils.Common.Log( "Low frame limiter." );
                lowFrame = 0;
            }
            
            if ( ++highFrame > HIGH_LIMIT )
            {
                if ( Packages.Locked )
                {
                    Command.Execute( new Commands.Processing.ProcessPackageLockCommand() );
                }
                
                Utils.Common.Log( "High frame limiter." );
                highFrame = 0;
            }
        }
    }
}
