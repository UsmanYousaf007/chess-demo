using System.Collections.Generic;
using System.Linq;
using HUFEXT.PackageManager.Editor.Models;
using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Views
{
    [InitializeOnLoad]
    public class PackageUpdateWindow : EditorWindow
    {
        readonly Vector2[] scroll = new Vector2[2];

        bool[] itemShouldBeUpdated;
        List<Models.PackageManifest> packagesToUpdate;
        bool forceUpdateDetected = false;

        public static void Init()
        {
            var window = CreateInstance( typeof(PackageUpdateWindow) ) as PackageUpdateWindow;

            if ( window != null )
            {
                if ( window.Initialize() )
                {
                    window.titleContent = new GUIContent( Models.Keys.Views.Update.TITLE );
                    window.minSize = new Vector2( 430f, 400f );
                    window.ShowUtility();
                }
                else Debug.LogFormat( "<color=\"#2ECC40\"><b>All HUF packages are up to date.</b></color>" );
            }
        }

        bool Initialize()
        {
            packagesToUpdate = Core.Packages.Data
                .Where( ( p ) => p.IsHufPackage && ( p.huf.status == Models.PackageStatus.UpdateAvailable ||
                                                     p.huf.status == Models.PackageStatus.ForceUpdate ||
                                                     p.huf.status == Models.PackageStatus.Migration ) )
                .ToList();

            if ( packagesToUpdate.Count == 0 )
            {
                return false;
            }

            itemShouldBeUpdated = new bool[packagesToUpdate.Count];
            return true;
        }

        void OnDisable()
        {
            if ( forceUpdateDetected )
            {
                EditorUtility.DisplayDialog( "Update required!",
                    "Some packages need to be updated immediately. Please click continue to install new versions of packages.",
                    "OK" );
                //Init();
            }
        }

        void OnGUI()
        {
            Utils.HGUI.BannerWithLogo( position.width );

            using ( new GUILayout.AreaScope( new Rect( 0, 80, position.width, position.height - 80 ) ) )
            {
                EditorGUILayout.Space();
                PackagesToUpdatePanel();
                GUILayout.FlexibleSpace();

                using ( new EditorGUILayout.HorizontalScope() )
                {
                    GUILayout.FlexibleSpace();

                    if ( GUILayout.Button( "Continue", GUILayout.Width( 150f ), GUILayout.Height( 30f ) ) )
                    {
                        List<Models.PackageManifest> chosenPackages = new List<PackageManifest>();

                        for ( int i = 0; i < packagesToUpdate.Count; ++i )
                        {
                            if ( itemShouldBeUpdated[i] )
                            {
                                chosenPackages.Add( packagesToUpdate[i] );
                            }
                        }

                        Core.Command.BindAndExecute(
                            new Commands.Processing.PackageResolveCommand( chosenPackages, true ),
                            new Commands.Processing.PackageLockCommand(),
                            new Commands.Processing.ProcessPackageLockCommand() );
                        Close();
                    }

                    GUILayout.FlexibleSpace();
                }

                GUILayout.FlexibleSpace();
            }
        }

        void PackagesToUpdatePanel()
        {
            using ( new GUILayout.HorizontalScope() )
            {
                EditorGUILayout.LabelField( Models.Keys.Views.Update.SELECT, EditorStyles.boldLabel );
                GUILayout.FlexibleSpace();

                if ( GUILayout.Button( "All" ) )
                {
                    for ( int i = 0; i < packagesToUpdate.Count; ++i )
                    {
                        itemShouldBeUpdated[i] = true;
                    }
                }

                if ( GUILayout.Button( "None" ) )
                {
                    for ( int i = 0; i < packagesToUpdate.Count; ++i )
                    {
                        itemShouldBeUpdated[i] = packagesToUpdate[i].huf.status == Models.PackageStatus.ForceUpdate;
                    }
                }
            }

            EditorGUILayout.Space();

            using ( var v = new EditorGUILayout.VerticalScope() )
            {
                Utils.HGUI.HorizontalSeparator();
                EditorGUI.DrawRect( v.rect, new Color( 0f, 0f, 0f, 0.3f ) );

                using ( var scope =
                    new GUILayout.ScrollViewScope( scroll[0], GUILayout.Height( position.height - 165f ) ) )
                {
                    scroll[0] = scope.scrollPosition;

                    for ( int i = 0; i < packagesToUpdate.Count; ++i )
                    {
                        DrawItem( i );
                    }
                }

                Utils.HGUI.HorizontalSeparator();
            }
        }

        void DrawItem( int i )
        {
            var myStyle = new GUIStyle { margin = new RectOffset( 0, 0, 0, 0 ) };
            var rect = EditorGUILayout.BeginVertical( myStyle, GUILayout.Height( 30f ) );

            {
                GUILayout.FlexibleSpace();

                using ( new GUILayout.HorizontalScope() )
                {
                    var package = packagesToUpdate[i];
                    var isMigration = package.huf.status == Models.PackageStatus.Migration;
                    var isForceUpdate = package.huf.status == Models.PackageStatus.ForceUpdate;

                    var remotePackageSupportsCurrentUnityVersion =
                        package.LatestPackageVersion().SupportsCurrentUnityVersion;

                    using ( new EditorGUI.DisabledScope(
                        isForceUpdate || !remotePackageSupportsCurrentUnityVersion ) )
                    {
                        if ( !remotePackageSupportsCurrentUnityVersion )
                        {
                            itemShouldBeUpdated[i] = false;
                        }
                        else if ( isForceUpdate && !itemShouldBeUpdated[i] )
                        {
                            itemShouldBeUpdated[i] = true;
                        }

                        if ( itemShouldBeUpdated[i] )
                        {
                            EditorGUI.DrawRect( rect, new Color( .5f, .5f, .5f, 0.1f ) );
                            GUI.contentColor = Color.white;
                        }

                        if ( GUI.Button( rect, GUIContent.none, EditorStyles.label ) )
                        {
                            itemShouldBeUpdated[i] = !itemShouldBeUpdated[i];
                        }

                        GUILayout.Label( GetIconForStatus( package.huf.status ),
                            GUILayout.Width( 16f ) );

                        if ( !remotePackageSupportsCurrentUnityVersion )
                        {
                            GUILayout.Label( package.displayName );

                            GUILayout.Label( $"Package only supports Unity: {package.unity}",
                                EditorStyles.whiteMiniLabel );
                        }
                        else if ( isMigration )
                        {
                            GUILayout.Label( package.displayName );

                            GUILayout.Label( $"Migrate to {package.huf.config.latestVersion}",
                                EditorStyles.whiteMiniLabel );
                        }
                        else
                        {
                            GUILayout.Label( $"{package.displayName} {package.version}" );

                            GUILayout.Label( $"Update to {package.huf.config.latestVersion}",
                                EditorStyles.centeredGreyMiniLabel );
                        }

                        if ( package.huf.status == Models.PackageStatus.ForceUpdate )
                        {
                            GUILayout.Label( "- Update required.", EditorStyles.centeredGreyMiniLabel );
                        }

                        GUILayout.FlexibleSpace();
                        itemShouldBeUpdated[i] = GUILayout.Toggle( itemShouldBeUpdated[i], "" );
                    }
                }

                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndVertical();
            Utils.HGUI.HorizontalSeparator();
        }

        Texture2D GetIconForStatus( Models.PackageStatus status )
        {
            switch ( status )
            {
                case Models.PackageStatus.Installed: return Utils.HGUI.Icons.PackageInstalledIcon;
                case Models.PackageStatus.UpdateAvailable: return Utils.HGUI.Icons.PackageUpdateIcon;
                case Models.PackageStatus.ForceUpdate: return Utils.HGUI.Icons.PackageForceUpdateIcon;
                case Models.PackageStatus.Migration: return Utils.HGUI.Icons.PackageMigrationIcon;
                default: return Utils.HGUI.Icons.PackageErrorIcon;
            }
        }
    }
}