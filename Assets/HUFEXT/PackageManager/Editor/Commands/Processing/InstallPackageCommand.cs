using System.IO;
using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Commands.Processing
{
    [InitializeOnLoad]
    public class InstallPackageCommand : Core.Command.Base
    {
        public string packageName;

        static InstallPackageCommand()
        {
            AssetDatabase.importPackageStarted += OnImportStarted;
            AssetDatabase.importPackageCompleted += OnImportCompleted;
            AssetDatabase.importPackageCancelled += OnImportCancelled;
            AssetDatabase.importPackageFailed += OnImportFailed;
        }

        public override void Execute()
        {
            var path = Utils.Common.GetPackagePath( packageName );

            if ( !File.Exists( path ) )
            {
                Core.Registry.Push( Models.Keys.PACKAGE_MANAGER_LAST_IMPORT_FAILED );
                Complete( false, $"Unable to find package {path}." );
                return;
            }

            Core.Packages.Installing = true;

            // Special case. When package import starts it will reload scripts and run GC after complete.
            // All callbacks will be cleared.
            Complete( true );
            AssetDatabase.ImportPackage( path, false );
        }

        static void OnImportStarted( string packageName )
        {
            Core.Registry.Remove( Models.Keys.CACHE_LAST_IMPORTED_PACKAGE_NAME_KEY );
            Core.Registry.Remove( Models.Keys.PACKAGE_MANAGER_LAST_IMPORT_FAILED );
            Utils.Common.Log( $"Import started: {packageName}" );
        }

        static void OnImportCompleted( string packageName )
        {
            Core.Registry.Save( Models.Keys.CACHE_LAST_IMPORTED_PACKAGE_NAME_KEY, packageName );
            Utils.Common.Log( $"Import completed: {packageName}" );
            var path = Utils.Common.GetPackagePath( packageName );

            if ( File.Exists( path ) )
            {
                File.Delete( path );
            }

            Utils.Common.RebuildDefines();
            ResumePackageInstallationIfPackageManagerWasUpdated();
        }

        static void ResumePackageInstallationIfPackageManagerWasUpdated()
        {
            if ( PlayerPrefs.HasKey( PackageResolveCommand.HPM_PACKAGES_TO_INSTALL ) )
            {
                var packagesToInstall =
                    Utils.Common.FromJsonToArray<Models.PackageManifest>(
                        PlayerPrefs.GetString( PackageResolveCommand.HPM_PACKAGES_TO_INSTALL ) );
                PlayerPrefs.DeleteKey(  PackageResolveCommand.HPM_PACKAGES_TO_INSTALL );

                Core.Command.Execute( new Commands.Processing.RefreshPackagesCommand
                {
                    OnComplete = ( result, serializedData ) =>
                    {
                        Core.Command.BindAndExecute(
                            new Commands.Processing.PackageResolveCommand( packagesToInstall, true ),
                            new Commands.Processing.PackageLockCommand(),
                            new Commands.Processing.ProcessPackageLockCommand() );
                    }
                } );
            }
        }

        static void OnImportCancelled( string packageName )
        {
            Core.Registry.Push( Models.Keys.PACKAGE_MANAGER_LAST_IMPORT_FAILED );
            EditorUtility.ClearProgressBar();
            Utils.Common.Log( $"Import cancelled: {packageName}" );
        }

        static void OnImportFailed( string packageName, string message )
        {
            OnImportCancelled( packageName );
            Utils.Common.Log( $"Import failed: {message}" );
        }
    }
}