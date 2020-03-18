using System;
using System.Collections.Generic;
using HUFEXT.PackageManager.Editor.API.Controllers;
using HUFEXT.PackageManager.Editor.API.Data;
using HUFEXT.PackageManager.Editor.Utils;
using HUFEXT.PackageManager.Editor.Utils.Helpers;
using UnityEditor;
using UnityEngine;
using Cache = HUFEXT.PackageManager.Editor.Utils.Cache;

namespace HUFEXT.PackageManager.Editor.Views
{
    [InitializeOnLoad]
    public class PackageUpdateWindow : EditorWindow
    {
        private const string windowTitle = "Huuuge Unity Framework - Update Packages";
        private const string forceUpdateLabel = "Select packages to update:";
        
        private readonly Vector2[] scroll = new Vector2[2];
        
        private bool[] itemShouldBeUpdated;
        private List<PackageManifest> packagesToUpdate;
        private bool forceUpdateDetected = false;
        
        private PackageController controller;
        public static bool IsOpened = false;
        
        static PackageUpdateWindow()
        {
            //EditorApplication.delayCall += Init;
        }

        public static void Init()
        {
            if ( PlayerPrefs.HasKey( Registry.Keys.PACKAGE_MANAGER_UPDATES ) )
            {
                Debug.LogError( "Update process is in progress..." );

                if ( !IsOpened )
                {
                    var controller = PackageController.CreateOrLoadFromCache();
                    controller?.UpdatePackagesRequest();
                }
                
                return;
            }

            if ( IsOpened )
            {
                return;
            }
            
            var window = CreateInstance( typeof( PackageUpdateWindow ) ) as PackageUpdateWindow;
            if ( window != null )
            {
                window.controller = PackageController.CreateOrLoadFromCache();
                if ( window.Initialize() )
                {
                    window.titleContent = new GUIContent( windowTitle );
                    window.minSize = new Vector2( 430f, 600f );
                    window.maxSize = window.minSize;
                    window.ShowUtility();
                    IsOpened = true;
                }
            }
        }

        private bool Initialize()
        {
            if ( controller == null || controller.Packages.Count == 0 )
            {
                return false;
            }

            packagesToUpdate = new List<PackageManifest>();

            foreach( var package in controller.Packages )
            {
                if( package.huf.status == PackageStatus.UpdateAvailable || 
                    package.huf.status == PackageStatus.ForceUpdate )
                {
                    packagesToUpdate.Add( package );
                }
            }
            
            if ( packagesToUpdate.Count == 0 )
            {
                Debug.Log( "[PackageManager] HUF packages are up to date." );
                return false;
            }
            
            itemShouldBeUpdated = new bool[packagesToUpdate.Count];

            for(int i = 0; i < packagesToUpdate.Count; ++i)
            {
                if ( packagesToUpdate[i].huf.status == PackageStatus.ForceUpdate )
                {
                    forceUpdateDetected = true;
                    itemShouldBeUpdated[i] = true;
                }
            }

            return true;
        }
        
        private void OnDisable()
        {
            if ( forceUpdateDetected )
            {
                EditorUtility.DisplayDialog( "Update required!", 
                                             "Some packages need to be updated immediately. Please click continue to install new versions of packages.", 
                                             "OK" );
                //Init();
            }
        }

        private void OnDestroy()
        {
            IsOpened = false;
        }

        private void OnGUI()
        {
            HCommonGUI.BannerWithLogo( position.width );

            using ( new GUILayout.AreaScope( new Rect( 0, 80, Screen.width, Screen.height - 80 ) ) )
            {
                EditorGUILayout.Space();
                PackagesToUpdatePanel();
                GUILayout.FlexibleSpace();
                HCommonGUI.HorizontalCentered( () =>
                {
                    if ( GUILayout.Button( "Continue", GUILayout.Width( 150f ), GUILayout.Height( 30f ) ) )
                    {
                        PrepareUpdateList();
                        controller.UpdatePackagesRequest();
                        forceUpdateDetected = false;
                        Close();
                    }
                });
                GUILayout.FlexibleSpace();
                EditorGUILayout.Space();
            }
        }

        private void PackagesToUpdatePanel()
        {
            using( new GUILayout.HorizontalScope() )
            {
                EditorGUILayout.LabelField( forceUpdateLabel, EditorStyles.boldLabel );
            
                GUILayout.FlexibleSpace();
                
                if ( GUILayout.Button( "All" ) )
                {
                    for(int i = 0; i < packagesToUpdate.Count; ++i)
                    {
                        itemShouldBeUpdated[i] = true;
                    }
                }
                if ( GUILayout.Button( "None" ) )
                {
                    for(int i = 0; i < packagesToUpdate.Count; ++i)
                    {
                        itemShouldBeUpdated[i] = packagesToUpdate[i].huf.status == PackageStatus.ForceUpdate;
                    }
                }
            }

            EditorGUILayout.Space();

            var r = EditorGUILayout.BeginVertical();
            {
                HCommonGUI.HorizontalSeparator();
                EditorGUI.DrawRect( r, new Color( 0f, 0f, 0f, 0.3f ) );
                using ( var scope = new GUILayout.ScrollViewScope( scroll[0], GUILayout.Height( 430f ) ) )
                {
                    scroll[0] = scope.scrollPosition;
                    for ( int i = 0; i < packagesToUpdate.Count; ++i )
                    {
                        DrawItem( i );
                    }
                }
                HCommonGUI.HorizontalSeparator();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawItem( int i )
        {
            var myStyle = new GUIStyle { margin = new RectOffset( 0, 0, 0, 0 ) };
            var rect = EditorGUILayout.BeginVertical( myStyle, GUILayout.Height( 30f ) );
            {
                GUILayout.FlexibleSpace();
                using( new GUILayout.HorizontalScope() )
                {
                    using( new EditorGUI.DisabledScope( packagesToUpdate[i].huf.status == PackageStatus.ForceUpdate ) )
                    {
                        if ( itemShouldBeUpdated[i] )
                        {
                            EditorGUI.DrawRect( rect, new Color( .5f, .5f, .5f, 0.1f ) );
                            GUI.contentColor = Color.white;
                        }
                        
                        if ( GUI.Button( rect, GUIContent.none, EditorStyles.label ) )
                        {
                            itemShouldBeUpdated[i] = !itemShouldBeUpdated[i];
                        }

                        GUILayout.Label( GetIconForStatus( packagesToUpdate[i].huf.status ), 
                                         GUILayout.Width( 16f ) );
                        GUILayout.Label( packagesToUpdate[i].displayName + " " + packagesToUpdate[i].version);
                        GUILayout.Label( "Update to " + packagesToUpdate[i].huf.config.latestVersion, 
                                         EditorStyles.centeredGreyMiniLabel );
                        
                        if ( packagesToUpdate[i].huf.status == PackageStatus.ForceUpdate )
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
            HCommonGUI.HorizontalSeparator();
        }
        
        private Texture2D GetIconForStatus( PackageStatus status )
        {
            switch ( status )
            {
                case PackageStatus.Installed: return HCommonGUI.Icons.PackageInstalledIcon;
                case PackageStatus.UpdateAvailable: return HCommonGUI.Icons.PackageUpdateIcon;
                case PackageStatus.ForceUpdate: return HCommonGUI.Icons.PackageForceUpdateIcon;
                default: return HCommonGUI.Icons.PackageErrorIcon;
            }
        }

        private void PrepareUpdateList()
        {
            var toUpdate = new PackageUpdateList();
            for ( int i = 0; i < itemShouldBeUpdated.Length; ++i )
            {
                if ( itemShouldBeUpdated[i] )
                {
                    toUpdate.Items.Add( packagesToUpdate[i] );
                }
            }

            var data = EditorJsonUtility.ToJson( toUpdate );
            if ( !string.IsNullOrEmpty( data ) )
            {
                PlayerPrefs.SetString( Registry.Keys.PACKAGE_MANAGER_UPDATES, data );
            }
        }
    }
}