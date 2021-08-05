using System;
using System.Collections.Generic;
using HUFEXT.PackageManager.Editor.Models;
using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Views.Items
{
    public class HPackageListItem : ListItem
    {
        static readonly Color lightGrey = new Color( 0.8f, 0.8f, 0.8f );

        static readonly Dictionary<Models.PackageStatus, GUIContent> statusContent =
            new Dictionary<Models.PackageStatus, GUIContent>
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
                        image = Utils.HGUI.Icons.PackageGitErrorIcon,
                        tooltip =
                            "This package contains GIT repository but package version is older than remote's package."
                    }
                },
                {
                    Models.PackageStatus.GitUpdate, new GUIContent()
                    {
                        image = Utils.HGUI.Icons.PackageGitUpdateIcon,
                        tooltip =
                            "This package contains GIT repository but package version is equal to remote's package."
                    }
                }
            };

        static readonly Dictionary<Models.PackageStatus, GUIContent> statusContentWhenUnityNotSupported =
            new Dictionary<Models.PackageStatus, GUIContent>
            {
                {
                    Models.PackageStatus.NotInstalled, new GUIContent()
                    {
                        image = Utils.HGUI.Icons.PackageNotInstalledIcon,
                        tooltip =
                            "This package is not available for current Unity version."
                    }
                },
                {
                    Models.PackageStatus.Installed, new GUIContent()
                    {
                        image = Utils.HGUI.Icons.PackageErrorIcon,
                        tooltip =
                            "This package cannot be used in current Unity version."
                    }
                },
                {
                    Models.PackageStatus.ForceUpdate, new GUIContent()
                    {
                        image = Utils.HGUI.Icons.PackageForceUpdateIcon,
                        tooltip =
                            "This package cannot be used in used current Unity version and must be updated."
                    }
                }
            };

        static readonly GUIStyle labelStyle = new GUIStyle( EditorStyles.label )
        {
            fontSize = 11,
            fontStyle = FontStyle.Normal,
            richText = true
        };

        static readonly GUIStyle boldStyle = new GUIStyle( EditorStyles.label )
        {
            fontSize = 11,
            fontStyle = FontStyle.Bold,
            richText = true
        };

        public readonly Models.PackageManifest package;

        public bool isActive = false;

        public HPackageListItem( PackageManagerWindow window, Models.PackageManifest package ) : base( window )
        {
            this.package = package;
        }

        public override PackageListView.ItemType Type => PackageListView.ItemType.PackageHUF;

        public override void DrawContent( Rect rect )
        {
            if ( package == null )
            {
                GUILayout.Label( "<color=red>Null package reference.</color>", labelStyle );
                return;
            }

            isActive = Window != null &&
                       Window.state.selectedPackage != null &&
                       Window.state.selectedPackage.name == package.name;
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
                        data = package.name
                    } );
                }

                var displayName = package.displayName.Length > 25
                    ? $"{package.displayName.Substring( 0, 22 )}..."
                    : package.displayName;

                switch ( package.huf.status )
                {
                    case Models.PackageStatus.GitError:
                    case Models.PackageStatus.ForceUpdate:
                        displayName = $"<color=red>{displayName}</color>";
                        break;
                    case Models.PackageStatus.UpdateAvailable:
                        displayName = $"<color=orange>{displayName}</color>";
                        break;
                    default:
                        if ( !package.SupportsCurrentUnityVersion )
                            displayName = $"<color=red>{displayName}</color>";
                        break;
                }

                GUILayout.Label( displayName, isActive ? boldStyle : labelStyle );

                if ( package.IsRepository )
                {
                    MiniLabel( "git" );
                }
                else
                {
                    if ( package.huf.isLocal )
                        MiniLabel( "local" );
                    if ( !package.IsStable )
                        MiniLabel( package.huf.prerelease );
                }

                GUILayout.FlexibleSpace();
                var width = GUILayout.Width( 16f );

                switch ( package.huf.status )
                {
                    case Models.PackageStatus.NotInstalled:
                    case Models.PackageStatus.Installed:
                    case Models.PackageStatus.UpdateAvailable:
                    case Models.PackageStatus.ForceUpdate:
                    case Models.PackageStatus.Migration:
                    {
                        GUILayout.Label( package.huf.version );

                        if ( package.SupportsCurrentUnityVersion ||
                             !statusContentWhenUnityNotSupported.ContainsKey( package.huf.status ) )
                            GUILayout.Label( statusContent[package.huf.status], width );
                        else
                            GUILayout.Label( statusContentWhenUnityNotSupported[package.huf.status], width );
                        break;
                    }
                    case Models.PackageStatus.Unknown:
                    case Models.PackageStatus.Unavailable:
                    case Models.PackageStatus.Conflict:
                    {
                        GUILayout.Label( statusContent[package.huf.status], width );
                        break;
                    }
                    case Models.PackageStatus.Embedded:
                    {
                        GUILayout.Label( statusContent[Models.PackageStatus.Embedded],
                            new GUIStyle( EditorStyles.centeredGreyMiniLabel )
                            {
                                normal = { textColor = isActive ? Color.white : lightGrey }
                            } );
                        break;
                    }
                    case Models.PackageStatus.Git:
                    case Models.PackageStatus.GitUpdate:
                    case Models.PackageStatus.GitError:
                    {
                        if ( !package.huf.version.Contains( "0.0.0" ) )
                        {
                            GUILayout.Label( package.huf.version );
                        }

                        GUILayout.Label( statusContent[package.huf.status],
                            new GUIStyle( EditorStyles.centeredGreyMiniLabel )
                            {
                                normal = { textColor = isActive ? Color.white : lightGrey }
                            } );
                        break;
                    }
                }
            }

            GUILayout.FlexibleSpace();
        }

        void MiniLabel( string text )
        {
            GUILayout.Label( text,
                new GUIStyle( EditorStyles.centeredGreyMiniLabel )
                {
                    normal = { textColor = isActive ? Color.white : lightGrey }
                } );
        }
    }
}