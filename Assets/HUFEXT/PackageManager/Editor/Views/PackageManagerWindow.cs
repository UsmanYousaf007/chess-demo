using System.Collections.Generic;
using System.IO;
using System.Text;
using HUFEXT.PackageManager.Editor.Commands.Base;
using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Views
{
    public enum ViewEvent
    {
        Undefined,
        CopyDeveloperID,
        RefreshListView,
        RefreshPackages,
        ShowPreviewPackages,
        ShowUpdateWindow,
        ForceResolvePackages,
        AddScopedRegistry,
        AddDefaultRegistries,
        ClearCache,
        GenerateReportHUF,
        GenerateReportFull,
        ContactSupport,
        RevokeLicense,
        SelectPackage,
        InstallPackage,
        RemovePackage,
        ChangePackagesChannel,
        TogglePreviewOrStableChannel,
        ChangeDevelopmentEnvPath,
        DisableDeveloperMode,
    }
    
    public class PackageManagerWindow : EditorWindow, IHasCustomMenu
    {
        static bool isDirty = false;
        
        readonly Queue<ViewEvent> eventsQueue = new Queue<ViewEvent>();
        readonly Dictionary<ViewEvent, object> eventsData = new Dictionary<ViewEvent, object>();
        ViewEvent currentEvent = ViewEvent.Undefined;
        ViewEvent asyncEvent = ViewEvent.Undefined;

        readonly List<PackageManagerView> views = new List<PackageManagerView>();

        [SerializeField] 
        public Models.PackageManagerState state;
        
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
        
        private void OnEnable()
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

            isDirty = false;
        }

        public void AddItemsToMenu(GenericMenu menu)
        {
            if ( Core.Packages.Installing || Core.Packages.UpdateInProgress )
            {
                menu.AddItem( new GUIContent( "HUF/Fix Me" ), false,
                    () =>
                    {
                        Core.Packages.Installing = false;
                        Core.Packages.UpdateInProgress = false;
                    } );
            }
        }
        
        public static void RefreshViews()
        {
            isDirty = true;
        }
        
        public void Enqueue( ViewEvent ev, object data = null )
        {
            if ( eventsQueue.Contains( ev ) )
            {
                Debug.Log( $"Duplicated event: {ev.ToString()}" );
                return;
            }
            
            eventsQueue.Enqueue( ev );
            if ( data != null )
            {
                eventsData[ev] = data;
            }
            
            views.ForEach( view => view.OnEventEnter( ev ) );
        }

        private void OnDisable()
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
            currentEvent = eventsQueue.Count > 0 ? eventsQueue.Dequeue() : ViewEvent.Undefined;

            if ( currentEvent != ViewEvent.Undefined )
            {
                HandleEvents( currentEvent );
            }

            if ( isDirty )
            {
                if ( asyncEvent != ViewEvent.Undefined )
                {
                    views.ForEach( view => view.RefreshView( asyncEvent ) );
                    asyncEvent = ViewEvent.Undefined;
                }
                
                views.ForEach( view => view.RefreshView( currentEvent ) );
                isDirty = false;
                Repaint();
            }
            
            if ( Core.Registry.IsSet( Models.Keys.PACKAGE_MANAGER_LAST_FETCH_KEY ) )
            {
                Core.Registry.Load( Models.Keys.PACKAGE_MANAGER_LAST_FETCH_KEY, out state.lastFetchDate );
                Core.Registry.Remove( Models.Keys.PACKAGE_MANAGER_LAST_FETCH_KEY );
            }
            
            if( Core.Registry.Pop( Models.Keys.PACKAGE_MANAGER_DIRTY_FLAG ) )
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

        void HandleEvents( ViewEvent ev )
        {
            switch ( ev )
            {
                case ViewEvent.CopyDeveloperID:
                {
                    GUIUtility.systemCopyBuffer = state.developerId;
                    Debug.Log( $"Your developer ID was copied to clipboard: {GUIUtility.systemCopyBuffer}" );
                    break;
                }

                case ViewEvent.RefreshListView:
                {
                    isDirty = true;
                    break;
                }
                
                case ViewEvent.RefreshPackages:
                {
                    Core.Packages.Installing = true;
                    Repaint();
                    
                    Core.Command.Execute( new Commands.Processing.RefreshPackagesCommand
                    {
                        OnComplete = ( result, serializedData ) =>
                        {
                            Core.Packages.Installing = false;
                            asyncEvent = ViewEvent.RefreshPackages;
                            isDirty = true;
                        }
                    });
                    break;
                }

                case ViewEvent.AddScopedRegistry:
                {
                    // Show editor window.
                    break;
                }

                case ViewEvent.AddDefaultRegistries:
                {
                    Core.Command.Execute( new AddGoogleScopedRegistryCommand() );
                    break;
                }
                
                case ViewEvent.ShowPreviewPackages:
                {
                    state.showPreviewPackages = !state.showPreviewPackages;
                    isDirty = true;
                    break;
                }

                case ViewEvent.ShowUpdateWindow:
                {
                    Views.PackageUpdateWindow.Init();
                    break;
                }

                case ViewEvent.GenerateReportHUF:
                {
                    var builder = new StringBuilder();
                    foreach ( var package in Core.Packages.Local )
                    {
                        builder.Append( $"{package.displayName} {package.version}\n" );
                    }
                    var log = builder.ToString();
                    Debug.Log( $"HUF Packages installed in {PlayerSettings.applicationIdentifier}: \n" + log );
                    GUIUtility.systemCopyBuffer = log;
                    break;
                }

                case ViewEvent.GenerateReportFull:
                {
                    Utils.BuildReportPreprocess.GenerateBuildInfo( report =>
                    {
                        var serializedReport = Utils.BuildReportPreprocess.SerializeReport( report );
                        Debug.Log( "Build report: \n" + serializedReport );
                        GUIUtility.systemCopyBuffer = serializedReport;
                        Debug.Log( "Build report copied to clipboard." );
                    } );
                    break;
                }
                
                case ViewEvent.ContactSupport:
                {
                    Application.OpenURL( Models.Keys.HELPSHIFT_URL_KEY + state.developerId );
                    break;
                }

                case ViewEvent.SelectPackage:
                {
                    Core.Command.Execute( new Commands.Processing.SelectPackageCommand( this, eventsData[ev] as string )
                    {
                        OnComplete = ( result, data ) =>
                        {
                            if ( result )
                            {
                                RefreshViews();
                            }
                        }
                    } );
                    break;
                }

                case ViewEvent.InstallPackage:
                {
                    Core.Packages.Installing = true;
                    Repaint();
                    
                    var package = eventsData[ev] as Models.PackageManifest;
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
                    break;
                }

                case ViewEvent.RemovePackage:
                {
                    Core.Packages.Installing = true;
                    Repaint();
                    
                    var package = eventsData[ev] as Models.PackageManifest;
                    if ( package == null )
                    {
                        break;
                    }
                    
                    Core.Command.Enqueue( new Commands.Processing.RemovePackageCommand
                    {
                        path = package.huf.path,
                        OnComplete = ( result, data ) =>
                        {
                            Core.Packages.Installing = false;
                            EditorUtility.ClearProgressBar();
                            AssetDatabase.Refresh();
                            isDirty = true;
                        }
                    });
                    break;
                }

                case ViewEvent.ClearCache:
                {
                    if ( Directory.Exists( Models.Keys.CACHE_DIRECTORY ) )
                    {
                        Directory.Delete( Models.Keys.CACHE_DIRECTORY, true );
                    }
                    Core.Packages.RemoveLock();
                    break;
                }

                case ViewEvent.ChangePackagesChannel:
                {
                    Core.Packages.Channel = state.channel;
                    Enqueue( ViewEvent.RefreshPackages );
                    break;
                }

                case ViewEvent.TogglePreviewOrStableChannel:
                {
                    if ( state.channel == Models.PackageChannel.Stable )
                    {
                        state.channel = Models.PackageChannel.Preview;
                    }
                    else
                    {
                        state.channel = Models.PackageChannel.Stable;
                    }
                    Enqueue( ViewEvent.ChangePackagesChannel );
                    break;
                }
                
                case ViewEvent.ChangeDevelopmentEnvPath:
                {
                    var temp = EditorUtility.OpenFolderPanel( "Change development registry", eventsData[ev] as string, "" );
                    if ( temp != string.Empty )
                    {
                        Core.Registry.Save( Models.Keys.PACKAGE_MANAGER_DEV_ENVIRONMENT, temp );
                        Enqueue( ViewEvent.RefreshPackages );
                    }
                    break;
                }

                case ViewEvent.DisableDeveloperMode:
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
                        }
                    });
                    break;
                }

                case ViewEvent.RevokeLicense:
                {
                    if ( EditorUtility.DisplayDialogComplex( "Are you sure?", "Cache data and your HUF license will be removed from this machine.", "OK", "Cancel", "" ) == 0 )
                    {
                        Models.Token.Invalidate();
                    }
                    break;
                }
            }

            eventsData.Remove( ev );
        }
    }
}
