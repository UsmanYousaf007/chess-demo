using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Views.Items
{
    public class HPackageListItem : ListItem
    {
        static readonly Dictionary<Models.PackageStatus, GUIContent> statusContent = new Dictionary<Models.PackageStatus, GUIContent>
        {
            {
                Models.PackageStatus.NotInstalled, new GUIContent()
                {
                    image = Utils.HGUI.Icons.PackageNotInstalledIcon,
                    tooltip = "This package is not installed."
                }
            },
            {
                Models.PackageStatus.Installed, new GUIContent()
                {
                    image = Utils.HGUI.Icons.PackageInstalledIcon,
                    tooltip = "This package is installed."
                }
            },
            {
                Models.PackageStatus.Migration, new GUIContent()
                {
                    image = Utils.HGUI.Icons.PackageMigrationIcon,
                    tooltip = "This package has an old format. Migration will update package to the latest version."
                }
            },
            {
                Models.PackageStatus.UpdateAvailable, new GUIContent()
                {
                    image = Utils.HGUI.Icons.PackageUpdateIcon,
                    tooltip = "New version of this package is available."
                }
            },
            {
                Models.PackageStatus.ForceUpdate, new GUIContent()
                {
                    image = Utils.HGUI.Icons.PackageForceUpdateIcon,
                    tooltip = "New mandatory version of this package is available."
                }
            },
            {
                Models.PackageStatus.Unknown, new GUIContent()
                {
                    image = Utils.HGUI.Icons.PackageErrorIcon,
                    tooltip = "There is an error with this package. Try to reimport package."
                }
            },
            {
                Models.PackageStatus.Unavailable, new GUIContent()
                {
                    image = Utils.HGUI.Icons.PackageErrorIcon,
                    tooltip = "This package is unavailable. Please, contact HUF support."
                }
            },
            {
                Models.PackageStatus.Conflict, new GUIContent()
                {
                    image = Utils.HGUI.Icons.PackageConflictIcon,
                    tooltip = "There are different packages with same path. Please, contact HUF support."
                }
            },
            {
                Models.PackageStatus.Development, new GUIContent()
                {
                    image   = Utils.HGUI.Icons.PackageConflictIcon,
                    tooltip = "This package is still in development."
                }
            },
            {
                Models.PackageStatus.Embedded, new GUIContent()
                {
                    text = "embedded",
                    tooltip = "Package manifest not found or package is part of other package."
                }
            },
            {
                Models.PackageStatus.Git, new GUIContent()
                {
                    image = Utils.HGUI.Icons.PackageGitIcon,
                    tooltip = "This package contains GIT repository."
                }
            },
            {
                Models.PackageStatus.GitError, new GUIContent()
                {
                    image   = Utils.HGUI.Icons.PackageGitErrorIcon,
                    tooltip = "This package contains GIT repository but package version is older than remote's package."
                }
            },
            {
                Models.PackageStatus.GitUpdate, new GUIContent()
                {
                    image   = Utils.HGUI.Icons.PackageGitUpdateIcon,
                    tooltip = "This package contains GIT repository but package version is equal to remote's package."
                }
            }
        };
        
        static GUIStyle labelStyle = new GUIStyle( EditorStyles.label )
        {
            fontSize  = 11,
            fontStyle = FontStyle.Normal,
            richText  = true
        };
        
        static GUIStyle boldStyle = new GUIStyle( EditorStyles.label )
        {
            fontSize  = 11,
            fontStyle = FontStyle.Bold,
            richText  = true
        };
        
        public override PackageListView.ItemType Type => PackageListView.ItemType.PackageHUF;

        public readonly Models.PackageManifest manifest;
        public bool isActive = false;

        public HPackageListItem( PackageManagerWindow window, Models.PackageManifest manifest ) : base ( window )
        {
            this.manifest = manifest;
        }

        public override void DrawContent( Rect rect )
        {
            if ( manifest == null )
            {
                GUILayout.Label( "<color=red>Null manifest reference.</color>", labelStyle );
                return;
            }
            
            isActive = Window != null &&
                       Window.state.selectedPackage != null &&
                       Window.state.selectedPackage.name == manifest.name;
            
            GUILayout.FlexibleSpace();
            using ( new GUILayout.HorizontalScope() )
            {
                if ( isActive )
                {
                    EditorGUI.DrawRect( rect, new Color( 0f, 0f, 0f, 0.2f ) );
                    GUI.contentColor = Color.white;
                }

                if ( GUI.Button( rect, GUIContent.none, EditorStyles.label ) && !isActive )
                {
                    Window.RegisterEvent( new Models.PackageManagerViewEvent
                    {
                        owner = Models.PackageManagerViewType.PackageListView,
                        eventType = Models.EventType.SelectPackage,
                        data = manifest.name
                    });
                }

                var displayName = manifest.displayName.Length > 25
                    ? manifest.displayName.Substring( 0, 22 ) + "..."
                    : manifest.displayName;

                if ( manifest.huf.status == Models.PackageStatus.UpdateAvailable )
                {
                    displayName = $"<color=orange>{displayName}</color>";
                }

                if ( manifest.huf.status == Models.PackageStatus.ForceUpdate || 
                     manifest.huf.status == Models.PackageStatus.GitError )
                {
                    displayName = $"<color=red>{displayName}</color>";
                }

                GUILayout.Label( displayName, isActive ? boldStyle : labelStyle );

                if ( manifest.IsRepository )
                {
                    MiniLabel( "git" );
                }
                else
                {
                    if ( manifest.huf.isLocal )
                    {
                        MiniLabel( "local" );
                    }

                    if ( manifest.huf.isPreview )
                    {
                        MiniLabel( "preview" );
                    }
                }
                GUILayout.FlexibleSpace();
                
                var width = GUILayout.Width( 16f );

                switch ( manifest.huf.status )
                {
                    case Models.PackageStatus.NotInstalled:
                    case Models.PackageStatus.Installed:
                    case Models.PackageStatus.UpdateAvailable:
                    case Models.PackageStatus.ForceUpdate:
                    case Models.PackageStatus.Migration:
                    case Models.PackageStatus.Development:
                    {
                        GUILayout.Label( manifest.huf.version );
                        GUILayout.Label( statusContent[manifest.huf.status], width );
                        break;
                    }

                    case Models.PackageStatus.Unknown:
                    case Models.PackageStatus.Unavailable:
                    case Models.PackageStatus.Conflict:
                    {
                        GUILayout.Label( statusContent[manifest.huf.status], width );
                        break;
                    }

                    case Models.PackageStatus.Embedded:
                    {
                        GUILayout.Label( statusContent[Models.PackageStatus.Embedded],
                            new GUIStyle( EditorStyles.centeredGreyMiniLabel )
                            {
                                normal = { textColor = isActive ? Color.white : Color.grey }
                            } );
                        break;
                    }
                    
                    case Models.PackageStatus.Git:
                    case Models.PackageStatus.GitUpdate:
                    case Models.PackageStatus.GitError:
                    {
                        if ( !manifest.huf.version.Contains( "0.0.0" ) )
                        {
                            GUILayout.Label( manifest.huf.version );
                        }

                        GUILayout.Label( statusContent[manifest.huf.status],
                            new GUIStyle( EditorStyles.centeredGreyMiniLabel )
                            {
                                normal = { textColor = isActive ? Color.white : Color.grey }
                            } );
                        break;
                    }
                }
            }

            GUILayout.FlexibleSpace();
        }

        void MiniLabel( string text )
        {
            GUILayout.Label( text, new GUIStyle( EditorStyles.centeredGreyMiniLabel )
            {
                normal = { textColor = isActive ? Color.white : Color.gray }
            } );
        }
    }
}
