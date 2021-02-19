using System.IO;
using System.Linq;
using HUFEXT.PackageManager.Editor.Commands.Data;
using HUFEXT.PackageManager.Editor.Commands.Processing;
using UnityEditor.Callbacks;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor
{
    public class PackageManagerOnScriptsReloaded
    {
        public const string CURRENT_PACKAGE_MANAGER_VERSION = "HPM_CurrentPackageManagerVersion";

        [DidReloadScripts]
        static void OnScriptsReloaded()
        {
            Core.Command.Execute( new GetLocalPackagesCommand()
            {
                OnComplete = ( result, serializedData ) =>
                {
                    var packageManagerPackage = Core.Packages.Local.FirstOrDefault( package =>
                        package.name == PackageResolveCommand.COM_HUUUGE_HUFEXT_PACKAGE_MANAGER );

                    if ( packageManagerPackage != null )
                        PlayerPrefs.SetString( CURRENT_PACKAGE_MANAGER_VERSION, packageManagerPackage.version );
                }
            } );
        }
    }
}