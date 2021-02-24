using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Views
{
    public class PackageManagerWindow : EditorWindow, IHasCustomMenu
    {
        readonly List<PackageManagerView> views = new List<PackageManagerView>();
        readonly Regex changelogVersionStartsWithRegex = new Regex( @"^##[\[ ]" );
        readonly Regex findVersionInChangelogRegex = new Regex( @"\[\d*[.]\d*[.]\d*\]" );

        static bool isDirty = false;

        [SerializeField] Models.PackageManagerQueue queue = new Models.PackageManagerQueue();
        [SerializeField] Models.PackageManagerViewEvent currentEvent = null;
        [SerializeField] int retryCount = 0;

        [SerializeField] public Models.PackageManagerState state;

        public static void RefreshPackages()
        {
            isDirty = true;
        }

        [MenuItem( Models.Keys.MENU_ITEM_OPEN_PACKAGE_MANAGER, false, 0 )]
        static void Init()
        {
            if ( Models.Token.IsValid )
            {
                Core.Registry.Pop( Models.Keys.PACKAGE_MANAGER_FORCE_CLOSE );
                var window = GetWindow<PackageManagerWindow>( false );

                if ( window == null )
                {
                    return;
                }

                window.titleContent = new GUIContent()
                {
                    text = " HUF Package Manager",
                    image = Utils.HGUI.Icons.WindowIcon
                };
                window.minSize = new Vector2( 900f, 300f );
                window.Show();
            }
            else
            {
                PolicyWindow.Init();
            }
        }

        public void AddItemsToMenu( GenericMenu menu )
        {
            if ( Core.Packages.Installing || Core.Packages.UpdateInProgress )
            {
                menu.AddItem( new GUIContent( "HUF/Fix Me" ),
                    false,
                    () =>
                    {
                        Core.Packages.Installing = false;
                        Core.Packages.UpdateInProgress = false;
                        Core.Registry.ClearCache();
                        Core.Command.FlushQueue();
                        currentEvent = null;
                        queue.events.Clear();
                    } );
            }

            menu.AddItem( new GUIContent( "HUF/Clear Window Queue" ),
                false,
                () =>
                {
                    currentEvent = null;
                    queue.events.Clear();
                } );
        }

        public void RegisterEvent( Models.PackageManagerViewEvent ev )
        {
            if ( queue.events.Find( e => e.eventType == ev.eventType ) != null )
            {
                return;
            }

            queue.events.Add( ev );
        }

        void OnEnable()
        {
            if ( state == null )
            {
                state = new Models.PackageManagerState();
                state.Load();
            }

            // Order is important, it determine how items will be repainted.
            if ( views.Count == 0 )
            {
                views.Add( new ToolbarView( this ) );
                views.Add( new DeveloperView( this ) );
                views.Add( new PackageListView( this ) );
                views.Add( new PackageView( this ) );
            }

            if ( queue.events == null )
            {
                queue.events = new List<Models.PackageManagerViewEvent>();
            }

            if ( !Core.Packages.Installing )
                UpdatePackagesInTheBackground();
        }

        void OnDisable()
        {
            state.Save();
        }

        void OnGUI()
        {
            if ( views == null || views.Count < 4 )
            {
                return;
            }

            using ( new EditorGUI.DisabledScope( Core.Packages.UpdateInProgress || Core.Packages.Installing ) )
            {
                views[0]?.Repaint();
                views[1]?.Repaint();

                using ( new GUILayout.HorizontalScope() )
                {
                    views[2]?.Repaint();
                    views[3]?.Repaint();
                }
            }
        }

        void OnInspectorUpdate()
        {
            if ( isDirty )
            {
                RegisterEvent( new Models.PackageManagerViewEvent
                {
                    owner = Models.PackageManagerViewType.Unknown,
                    eventType = Models.EventType.RefreshPackages
                } );
                isDirty = false;
            }

            // Fetch next event to handle.
            if ( currentEvent == null && queue.events.Count > 0 )
            {
                currentEvent = queue.events[0];
                queue.events.RemoveAt( 0 );
                retryCount = 0;
            }

            // Only if current event is completed.
            if ( ProcessEvent() )
            {
                for ( int i = 0; i < views.Count; ++i )
                {
                    if ( views[i].Type != currentEvent.owner )
                    {
                        views[i].OnEventCompleted( currentEvent );
                    }
                }

                currentEvent = null;
                Repaint();
            }

            if ( Core.Registry.IsSet( Models.Keys.PACKAGE_MANAGER_LAST_FETCH_KEY ) )
            {
                Core.Registry.Load( Models.Keys.PACKAGE_MANAGER_LAST_FETCH_KEY, out state.lastFetchDate );
                Core.Registry.Remove( Models.Keys.PACKAGE_MANAGER_LAST_FETCH_KEY );
            }

            if ( Core.Registry.Pop( Models.Keys.PACKAGE_MANAGER_DIRTY_FLAG ) )
            {
                Core.Registry.Load( Models.Keys.CACHE_LAST_FETCH_TIME_KEY, out state.lastFetchDate );
            }

            if ( !Models.Token.Exists )
            {
                Close();
            }

            if ( Core.Registry.Pop( Models.Keys.PACKAGE_MANAGER_FORCE_CLOSE ) )
            {
                Close();
            }
        }

        void UpdatePackagesInTheBackground()
        {
            Core.Command.Execute( new Commands.Processing.RefreshPackagesCommand
            {
                downloadInTheBackground = true,
                OnComplete = ( result, serializedData ) =>
                {
                    //RefreshPackages event is need to refresh the views
                    RegisterEvent( new Models.PackageManagerViewEvent
                    {
                        owner = Models.PackageManagerViewType.Unknown,
                        eventType = Models.EventType.RefreshPackages,
                        completed = true,
                        inProgress = true
                    } );
                }
            } );
        }

        // Returns true if event is completed.
        bool ProcessEvent()
        {
            if ( currentEvent == null )
            {
                return false;
            }

            if ( currentEvent.inProgress )
            {
                retryCount++;

                if ( retryCount > 30 )
                {
                    currentEvent.completed = true;
                    retryCount = 0;
                }

                return currentEvent.completed;
            }

            currentEvent.inProgress = true;
            HandleEvent();
            return currentEvent.completed;
        }

        void HandleEvent()
        {
            switch ( currentEvent.eventType )
            {
                case Models.EventType.CopyDeveloperID:
                {
                    GUIUtility.systemCopyBuffer = state.developerId;
                    Debug.Log( $"Your developer ID was copied to clipboard: {GUIUtility.systemCopyBuffer}" );
                    currentEvent.completed = true;
                    break;
                }
                case Models.EventType.RefreshListView:
                {
                    //isDirty = true;
                    currentEvent.completed = true;
                    break;
                }
                case Models.EventType.AddDefaultRegistries:
                {
                    Core.Command.Execute( new Commands.Base.AddGoogleScopedRegistryCommand() );
                    currentEvent.completed = true;
                    break;
                }
                case Models.EventType.AddScopedRegistry:
                {
                    CustomRegistryWindow.Init();
                    currentEvent.completed = true;
                    break;
                }
                case Models.EventType.ShowPreviewPackages:
                {
                    state.showPreviewPackages = !state.showPreviewPackages;
                    currentEvent.completed = true;
                    break;
                }
                case Models.EventType.ShowUnityPackages:
                {
                    state.showUnityPackages = !state.showUnityPackages;
                    currentEvent.completed = true;
                    break;
                }
                case Models.EventType.ShowUpdateWindow:
                {
                    PackageUpdateWindow.Init();
                    currentEvent.completed = true;
                    break;
                }
                case Models.EventType.GenerateReportHUF:
                {
                    GenerateHUFReport( false );
                    break;
                }
                case Models.EventType.GenerateReportSDKs:
                {
                    GenerateHUFReport( true );
                    break;
                }
                case Models.EventType.GenerateReportFull:
                {
                    Utils.BuildReportPreprocess.GenerateBuildInfo( report =>
                    {
                        GUIUtility.systemCopyBuffer = Utils.BuildReportPreprocess.SerializeReport( report );

                        Debug.Log(
                            $"Build report: \n{GUIUtility.systemCopyBuffer}\nBuild report copied to clipboard." );
                        currentEvent.completed = true;
                    } );
                    break;
                }
                case Models.EventType.ContactSupport:
                {
                    Application.OpenURL( Models.Keys.HELPSHIFT_URL_KEY + state.developerId );
                    currentEvent.completed = true;
                    break;
                }
                case Models.EventType.SelectPackage:
                {
                    Core.Command.Execute( new Commands.Processing.SelectPackageCommand( this, currentEvent.data )
                    {
                        OnComplete = ( result, data ) => { currentEvent.completed = true; }
                    } );
                    break;
                }
                case Models.EventType.RefreshPackages:
                {
                    Core.Command.Execute( new Commands.Processing.RefreshPackagesCommand
                    {
                        OnComplete = ( result, serializedData ) =>
                        {
                            if ( currentEvent != null )
                            {
                                currentEvent.completed = true;
                            }
                        }
                    } );
                    break;
                }
                case Models.EventType.ClearCache:
                {
                    Core.Registry.ClearCache();
                    Core.Packages.RemoveLock();
                    currentEvent.completed = true;
                    break;
                }
                case Models.EventType.ChangePackagesChannel:
                {
                    Core.Packages.Channel = state.channel;

                    RegisterEvent( new Models.PackageManagerViewEvent
                    {
                        owner = currentEvent.owner,
                        eventType = Models.EventType.RefreshPackages
                    } );
                    currentEvent.completed = true;
                    break;
                }
                case Models.EventType.TogglePreviewOrStableChannel:
                {
                    if ( state.channel == Models.PackageChannel.Stable )
                    {
                        state.channel = Models.PackageChannel.Preview;
                    }
                    else
                    {
                        state.channel = Models.PackageChannel.Stable;
                    }

                    Core.Packages.Channel = state.channel;

                    RegisterEvent( new Models.PackageManagerViewEvent
                    {
                        owner = currentEvent.owner,
                        eventType = Models.EventType.ChangePackagesChannel
                    } );
                    currentEvent.completed = true;
                    break;
                }
                case Models.EventType.ChangeDevelopmentEnvPath:
                {
                    var temp = EditorUtility.OpenFolderPanel( "Change development registry",
                        currentEvent.data,
                        "" );

                    if ( temp != string.Empty )
                    {
                        Core.Registry.Save( Models.Keys.PACKAGE_MANAGER_DEV_ENVIRONMENT, temp );

                        RegisterEvent( new Models.PackageManagerViewEvent
                        {
                            owner = currentEvent.owner,
                            eventType = Models.EventType.RefreshPackages
                        } );
                    }

                    currentEvent.completed = true;
                    break;
                }
                case Models.EventType.DisableDeveloperMode:
                {
                    Core.Registry.Pop( Models.Keys.PACKAGE_MANAGER_DEBUG_LOGS );
                    Core.Packages.Channel = Models.PackageChannel.Stable;
                    state.channel = Models.PackageChannel.Stable;

                    Core.Command.Execute( new Commands.Processing.RefreshPackagesCommand
                    {
                        downloadLatest = true,
                        OnComplete = ( result, serializedData ) =>
                        {
                            var group = BuildPipeline.GetBuildTargetGroup( EditorUserBuildSettings.activeBuildTarget );
                            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup( group );
                            var replace = defines.Replace( "HPM_DEV_MODE", "" );
                            PlayerSettings.SetScriptingDefineSymbolsForGroup( group, replace );
                            currentEvent.completed = true;
                        }
                    } );
                    break;
                }
                case Models.EventType.RevokeLicense:
                {
                    if ( EditorUtility.DisplayDialogComplex( "Are you sure?",
                        "Cache data and your HUF license will be removed from this machine.",
                        "OK",
                        "Cancel",
                        "" ) == 0 )
                    {
                        Models.Token.Invalidate();
                    }

                    currentEvent.completed = true;
                    break;
                }
                case Models.EventType.InstallPackage:
                {
                    Core.Packages.Installing = true;
                    Repaint();
                    var package = Models.PackageManifest.ParseManifest( currentEvent.data, true );

                    if ( package == null )
                    {
                        break;
                    }

                    var useLatestVersion = package.huf.status == Models.PackageStatus.UpdateAvailable ||
                                           package.huf.status == Models.PackageStatus.ForceUpdate ||
                                           package.huf.status == Models.PackageStatus.Migration;
                    Core.Command.Enqueue( new Commands.Processing.PackageResolveCommand( package, useLatestVersion ) );
                    Core.Command.Enqueue( new Commands.Processing.PackageLockCommand() );
                    Core.Command.Enqueue( new Commands.Processing.ProcessPackageLockCommand() );
                    currentEvent.completed = true;
                    break;
                }
                case Models.EventType.RemovePackage:
                {
                    Core.Registry.ClearCache();
                    var package = Models.PackageManifest.ParseManifest( currentEvent.data, true );

                    if ( package == null )
                    {
                        break;
                    }

                    if ( package.huf.isUnity )
                    {
                        UnityEditor.PackageManager.Client.Remove( package.name );
                        currentEvent.completed = true;
                        RefreshPackages();
                        break;
                    }

                    Core.Packages.Installing = true;
                    Repaint();

                    RegisterEvent( new Models.PackageManagerViewEvent
                    {
                        owner = Models.PackageManagerViewType.Unknown,
                        eventType = Models.EventType.RefreshPackages
                    } );

                    //currentEvent.completed = true;

                    Core.Command.Enqueue( new Commands.Processing.RemovePackageCommand
                    {
                        path = package.huf.path,
                        OnComplete = ( result, data ) =>
                        {
                            currentEvent.completed = true;
                            Utils.Common.RebuildDefines();
                        }
                    } );
                    break;
                }
                case Models.EventType.BuildSelectedPackage:
                {
                    Core.Command.Enqueue( new Commands.Processing.BuildPackageCommand
                    {
                        manifest = state.selectedPackage
                    } );
                    currentEvent.completed = true;
                    break;
                }
                default:
                {
                    currentEvent.completed = true;
                    break;
                }
            }
        }

        void GenerateHUFReport( bool withSDKs )
        {
            var builder = new StringBuilder();

            foreach ( var package in Core.Packages.Local )
            {
                builder.Append( $"{package.displayName} {package.version}\n" );

                if ( withSDKs )
                {
                    if ( package.TryGetChangelog( out string changelog ) )
                    {
                        var lines = changelog.Split( '\n' );

                        for ( int i = 0; i < lines.Length; i++ )
                        {
                            var line = lines[i];

                            if ( changelogVersionStartsWithRegex.IsMatch( line ) &&
                                 findVersionInChangelogRegex.IsMatch( line ) )
                                break;

                            line = line.Trim();
                            line = Regex.Replace( line, "^-", "" ).Trim();

                            if ( line.Length > 0 )
                            {
                                if ( line.StartsWith( "#" ) || line.StartsWith( "*" ) )
                                    builder.Append( "\t" );
                                builder.Append( $"\t{line}\n" );
                            }
                        }
                    }
                }

                GUIUtility.systemCopyBuffer = builder.ToString();

                Debug.Log(
                    $"HUF Packages installed in {PlayerSettings.applicationIdentifier}: \n{GUIUtility.systemCopyBuffer}" );
                currentEvent.completed = true;
            }
        }
    }
}