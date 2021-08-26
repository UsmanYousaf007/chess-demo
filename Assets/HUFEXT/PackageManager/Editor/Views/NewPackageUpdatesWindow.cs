using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HUFEXT.PackageManager.Editor.Models;
using HUFEXT.PackageManager.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Views
{
    [InitializeOnLoad]
    public class NewPackageUpdatesWindow : EditorWindow
    {
        const string NEW_PACKAGE_UPDATES_WINDOW_LAST_CHECK_TIME = "HUFNewPackageUpdatesWindowLastCheckTime";
        const string NEW_PACKAGE_UPDATES_WINDOW_LAST_SESSION_ID = "HUFNewPackageUpdatesWindowLastSessionID";

        static readonly Regex lineVersionRegex = new Regex( @"\[\d*\.\d*\.\d*\]" );
        readonly Vector2[] scroll = new Vector2[2];

        static GUIStyle redBoldLabel;
        static GUIStyle yellowBoldLabel;

        static List<Models.PackageManifest> packagesForceUpdates;
        static List<Models.PackageManifest> packagesUpdates;
        static List<Models.PackageManifest> packagesToGetChangelog = new List<PackageManifest>();
        static Dictionary<string, string> packageChangelogChanges = new Dictionary<string, string>();
        static NewPackageUpdatesWindow window;

        static NewPackageUpdatesWindow()
        {
            InitAndShow();
        }

        [MenuItem( "HUF/Check for new package updates" )]
        static void InitAndShow()
        {
            if ( window != null )
                return;

            packageChangelogChanges.Clear();
            packagesToGetChangelog.Clear();
            RefreshPackages( CheckIfShouldBeShown );
        }

        static void RefreshPackages( Action OnComplete )
        {
            Common.Log( "[NewPackageUpdatesWindow] Refreshing packages" );

            Core.Command.Execute( new Commands.Processing.RefreshPackagesCommand
            {
                downloadInTheBackground = true,
                OnComplete = ( result, serializedData ) =>
                {
                    packagesForceUpdates = Core.Packages.Data
                        .Where( ( p ) => p.IsHufPackage && ( p.huf.status == Models.PackageStatus.ForceUpdate ||
                                                             p.huf.status == Models.PackageStatus.Migration ) )
                        .ToList();

                    packagesUpdates = Core.Packages.Data
                        .Where( ( p ) => p.IsHufPackage && p.huf.status == Models.PackageStatus.UpdateAvailable )
                        .ToList();
                    packagesToGetChangelog.AddRange( packagesForceUpdates );
                    packagesToGetChangelog.AddRange( packagesUpdates );
                    OnComplete?.Invoke();
                }
            } );
        }

        static void CheckIfShouldBeShown()
        {
            if ( packagesForceUpdates.Count == 0 && packagesUpdates.Count == 0 )
            {
                Common.LogAlways( Models.Keys.Views.Update.FORMATTED_ALL_HUF_PACKAGES_ARE_UP_TO_DATE );
                return;
            }

            if ( packagesForceUpdates.Count > 0 )
            {
                if ( PlayerPrefs.HasKey( NEW_PACKAGE_UPDATES_WINDOW_LAST_SESSION_ID ) &&
                     EditorAnalyticsSessionInfo.id ==
                     long.Parse( PlayerPrefs.GetString( NEW_PACKAGE_UPDATES_WINDOW_LAST_SESSION_ID ) ) )
                {
                    Common.Log(
                        $"[NewPackageUpdatesWindow] Window will be shown in the next session: {EditorAnalyticsSessionInfo.id}" );
                    return;
                }
            }
            else if ( PlayerPrefs.HasKey( NEW_PACKAGE_UPDATES_WINDOW_LAST_CHECK_TIME ) )
            {
                var checkDate =
                    DateTime.Parse( PlayerPrefs.GetString( NEW_PACKAGE_UPDATES_WINDOW_LAST_CHECK_TIME ),
                        CultureInfo.InvariantCulture );

                if ( checkDate > DateTime.UtcNow )
                {
                    Common.Log( $"[NewPackageUpdatesWindow] Next check on: {checkDate}" );
                    return;
                }
            }

            PlayerPrefs.SetString( NEW_PACKAGE_UPDATES_WINDOW_LAST_SESSION_ID,
                EditorAnalyticsSessionInfo.id.ToString() );
            var nextCheckDate = DateTime.UtcNow.AddDays( 7 );

            PlayerPrefs.SetString( NEW_PACKAGE_UPDATES_WINDOW_LAST_CHECK_TIME,
                nextCheckDate.ToString( CultureInfo.InvariantCulture ) );
            DownloadPackageChangelogs( ShowIfThereAreNewUpdates );
        }

        static void ShowIfThereAreNewUpdates()
        {
            if ( window == null )
            {
                window = CreateInstance( typeof(NewPackageUpdatesWindow) ) as NewPackageUpdatesWindow;

                if ( window != null )
                {
                    window.titleContent = new GUIContent( Models.Keys.Views.NewPackageUpdates.TITLE );
                    window.minSize = new Vector2( 830f, 800f );
                    window.ShowUtility();
                }
            }
            else
                Common.LogAlways(
                    "[NewPackageUpdatesWindow] No new HUF packages updates." );
        }

        static void DownloadPackageChangelogs( Action OnComplete )
        {
            Common.Log( $"[NewPackageUpdatesWindow] Downloading Package Changelogs: {packagesToGetChangelog.Count}" );

            if ( packagesToGetChangelog.Count > 0 )
            {
                var package = packagesToGetChangelog[0];

                Core.Command.Execute( new Commands.Connection.DownloadPackageChangelogCommand()
                {
                    package = package.LatestPackageVersion(),
                    silent = true,
                    OnComplete = ( success, serializedData ) =>
                    {
                        if ( success )
                            packagesToGetChangelog.RemoveAt( 0 );

                        if ( !string.IsNullOrEmpty( serializedData ) )
                            packageChangelogChanges[package.name] = serializedData;
                        GetChangelogDifference( package, serializedData );
                        DownloadPackageChangelogs( OnComplete );
                    }
                } );
            }
            else
                OnComplete?.Invoke();
        }

        static void GetChangelogDifference( PackageManifest package, string newChangelog )
        {
            if ( string.IsNullOrEmpty( newChangelog ) )
                return;

            string oldVersion = package.version;
            string currentVersion = "";
            string[] lines = newChangelog.Split( '\n' );
            StringBuilder changelogDiffBuilder = new StringBuilder();

            foreach ( var line in lines )
            {
                if ( line.StartsWith( "## " ) || line.StartsWith( "##[" ) )
                {
                    var match = lineVersionRegex.Match( line );

                    if ( match.Success )
                        currentVersion = match.Value.Substring( 1, match.Value.Length - 2 );
                }

                if ( currentVersion == oldVersion )
                    break;

                if ( !string.IsNullOrEmpty( currentVersion ) )
                {
                    if ( changelogDiffBuilder.Length > 0 )
                        changelogDiffBuilder.Append( '\n' );
                    changelogDiffBuilder.Append( line );
                }
            }

            packageChangelogChanges[package.name] = changelogDiffBuilder.ToString().Trim();
        }

        void OnGUI()
        {
            if ( redBoldLabel == null )
                redBoldLabel = new GUIStyle( EditorStyles.whiteLargeLabel )
                {
                    normal = new GUIStyleState()
                    {
                        textColor = new Color( 1, 0.1f, 0.1f ),
                    },
                    fontStyle = EditorStyles.whiteBoldLabel.fontStyle
                };

            if ( yellowBoldLabel == null )
                yellowBoldLabel = new GUIStyle( redBoldLabel )
                {
                    normal = new GUIStyleState()
                    {
                        textColor = new Color( 1, 0.6f, 0.1f ),
                    },
                };
            Utils.HGUI.BannerWithLogo( position.width );

            using ( new GUILayout.AreaScope( new Rect( 0, 80, position.width, position.height - 84 ) ) )
            {
                EditorGUILayout.Space();
                PackagesToUpdatePanel();
                GUILayout.FlexibleSpace();

                using ( new EditorGUILayout.HorizontalScope() )
                {
                    GUILayout.FlexibleSpace();

                    if ( GUILayout.Button( "Open updates window", Models.Keys.GUI.mediumButtonSize ) )
                    {
                        PackageUpdateWindow.Init();
                        Close();
                    }

                    GUILayout.Space( 6 );

                    if ( GUILayout.Button( "Dismiss", Models.Keys.GUI.mediumButtonSize ) )
                    {
                        List<Models.PackageManifest> chosenPackages = new List<PackageManifest>();

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
            using ( var v = new EditorGUILayout.VerticalScope( GUILayout.ExpandWidth( true ) ) )
            {
                Utils.HGUI.HorizontalSeparator();
                EditorGUI.DrawRect( v.rect, new Color( 0f, 0f, 0f, 0.3f ) );

                using ( var scope =
                    new GUILayout.ScrollViewScope( scroll[0], GUILayout.Height( position.height - 130f ) ) )
                {
                    scroll[0] = scope.scrollPosition;

                    foreach ( var package in packagesForceUpdates )
                    {
                        DrawItem( package );
                    }

                    foreach ( var package in packagesUpdates )
                    {
                        DrawItem( package );
                    }
                }

                Utils.HGUI.HorizontalSeparator();
            }
        }

        void DrawItem( PackageManifest package )
        {
            var myStyle = new GUIStyle { margin = new RectOffset( 3, 3, 3, 5 ) };
            var rect = EditorGUILayout.BeginVertical( myStyle, GUILayout.Height( 40f ) );

            {
                GUILayout.FlexibleSpace();

                using ( new GUILayout.HorizontalScope() )
                {
                    var remotePackageSupportsCurrentUnityVersion =
                        package.LatestPackageVersion().SupportsCurrentUnityVersion;

                    GUILayout.Label( GetIconForStatus( package.huf.status ),
                        GUILayout.Width( 16f ),
                        GUILayout.Height( 24f ) );

                    if ( !remotePackageSupportsCurrentUnityVersion )
                    {
                        GUILayout.Label( package.displayName );

                        GUILayout.Label( $"Update only supports Unity: {package.unity}",
                            redBoldLabel );
                    }
                    else if ( package.huf.status == Models.PackageStatus.Migration )
                    {
                        GUILayout.Label( package.displayName );

                        GUILayout.Label( $"Migrate to {package.huf.config.latestVersion}",
                            new GUIStyle( redBoldLabel ) );
                    }
                    else
                    {
                        GUILayout.Label( $"{package.displayName} {package.version}", EditorStyles.whiteLargeLabel );

                        if ( package.huf.status == Models.PackageStatus.ForceUpdate )
                            GUILayout.Label( $"Update to {package.huf.config.minimumVersion} is required",
                                redBoldLabel );
                        else
                            GUILayout.Label( $"New version available {package.huf.config.latestVersion}",
                                yellowBoldLabel );
                    }

                    GUILayout.FlexibleSpace();
                }

                if ( packageChangelogChanges.ContainsKey( package.name ) )
                {
                    GUILayout.Label( packageChangelogChanges[package.name] );
                }
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