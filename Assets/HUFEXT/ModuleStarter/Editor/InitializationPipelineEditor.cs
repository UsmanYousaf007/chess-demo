using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HUF.Utils.Editor.Configs;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Logging;
using HUFEXT.ModuleStarter.Runtime.API;
using HUFEXT.ModuleStarter.Runtime.Config;
using HUFEXT.ModuleStarter.Runtime.Data;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static HUFEXT.ModuleStarter.Runtime.Config.ModuleInitializerConfig;

namespace HUFEXT.ModuleStarter.Editor
{
    [CustomEditor(typeof(HInitializationPipelineConfig))]
    public class InitializationPipelineEditor : UnityEditor.Editor
    {
        const string PATH_RESOURCES = "Assets/Resources";
        const string PATH_DEFAULT = "HUFEXT/ModuleStarter/Editor/defaultOrder.dat";
        const string PATH_ORDER = "StreamingAssets/initOrder.dat";

        const string SYNC_ICON = "d_PlayButtonProfile";
        const string ASYNC_ICON = "d_PlayButton";
        const string SKIP_ICON = "d_LookDevClose@2x";
        const string NO_SYNC_ICON = "Toolbar Minus";

        const float WIDTH_CHECKBOX = 30;
        const float WIDTH_STATUS = 35;
        const float LIST_HEADER_OFFSET = 13;


        static readonly HLogPrefix logPrefix = new HLogPrefix(nameof(InitializationPipelineEditor));

        ReorderableList entriesList;

        List<ReorderableEntry> entries;

        [MenuItem("HUF/Configs/Initialization Pipeline")]
        public static void OpenEditor()
        {
            if ( HConfigs.HasConfig<HInitializationPipelineConfig>() )
            {
                var config = Resources.LoadAll<HInitializationPipelineConfig>( HConfigs.CONFIGS_FOLDER )[0];
                Selection.activeObject = config;
                return;
            }

            CopyDefaults();
            HInitializationPipelineConfig newConfig = PrepareConfig();
            Selection.activeObject = newConfig;
        }

        static void CopyDefaults()
        {
            File.Copy(
                Path.Combine( Application.dataPath, PATH_DEFAULT ),
                Path.Combine( Application.dataPath, PATH_ORDER ),
                true );
        }

        static void DisableAutoinits( bool exceptHips )
        {
            var field = typeof(FeatureConfigBase).GetField( "autoInit",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance );

            if ( field == null )
            {
                HLog.LogError( logPrefix, $"{nameof(FeatureConfigBase)} structure is unexpected. Contact HUF support." );
                return;
            }

            var paths = new List<string>();

            foreach ( var config in Resources.LoadAll<FeatureConfigBase>( HConfigs.CONFIGS_FOLDER ) )
            {
                paths.Add( AssetDatabase.GetAssetPath( config ) );
                if ( config is HInitializationPipelineConfig )
                {
                    field.SetValue( config, exceptHips );
                    continue;
                }
                field.SetValue( config, false );
            }

            AssetDatabase.ForceReserializeAssets( paths );
            AssetDatabase.ForceReserializeAssets( paths.Take( 1 ) );
            AssetDatabase.Refresh( ImportAssetOptions.DontDownloadFromCacheServer );
        }

        static HInitializationPipelineConfig PrepareConfig()
        {
            var newConfig = CreateInstance<HInitializationPipelineConfig>();

            AssetDatabase.CreateAsset(
                newConfig,
                Path.Combine( PATH_RESOURCES, $"{HConfigs.CONFIGS_FOLDER}/{nameof(HInitializationPipelineConfig)}.asset" ) );
            AssetDatabase.SaveAssets();
            return newConfig;
        }

        public override void OnInspectorGUI()
        {
#if !HUFEXT_MODULE_STARTER
            DrawDefineWarning();
            return;
#endif

            DrawHUFHeader();
            GUILayout.FlexibleSpace();
            base.OnInspectorGUI();
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField( "Initialization Pipeline order", EditorStyles.boldLabel );
            entriesList.DoLayoutList();
            DrawInfoBox();
            EditorGUILayout.Separator();
            DrawRefreshButton();
            DrawSaveButton();
            EditorGUILayout.Separator();
            DrawPipelineButton();
            DrawDefaultsButton();

            //DrawDebugButton(); // Uncomment for development
        }

        static void DrawInfoBox()
        {
            const string INFO = "If you want a feature initialization handled by HIP disable it's AutoInit first. Remember to save project so changes are serialized and visible to HIP.";
            EditorGUILayout.HelpBox( INFO, MessageType.Info );
        }

        void OnEnable()
        {
            BuildList();
            BuildOrderableList();
        }

        void BuildOrderableList()
        {
            entriesList = new ReorderableList( entries, typeof(string) )
            {
                displayAdd = false,
                displayRemove = false,
                drawHeaderCallback = DrawListHeader,
                drawElementCallback = DrawEntry
            };
        }

        void BuildList()
        {
            entries = new List<ReorderableEntry>();
            SortEntries();
            ModuleInitializer.ReloadAll();
            foreach ( var entry in Entries )
            {
                var initializer = ModuleInitializer.Get( entry.id );

                if ( initializer == null )
                    continue;

                entries.Add( new ReorderableEntry()
                {
                    name = entry.id,
                    isAsync = entry.isAsync,
                    isAsyncOnly = initializer.asyncOnly,
                    isSkipped = entry.isSkipped
                } );
            }

            if ( entriesList == null )
                BuildOrderableList();

            entriesList.list = entries;
        }

        void DrawEntry( Rect rect, int index, bool isActive, bool isFocused )
        {
            SplitRect( rect, out Rect statusRect, out Rect syncRect, out Rect skipRect, out Rect entryRect );
            ReorderableEntry entry = entries[index];

            GUI.Label( statusRect, GetStatusIcon(entry) );

            using( new EditorGUI.DisabledScope( entry.isSkipped ) )
            {
                if ( entry.isAsyncOnly )
                    GUI.Label( syncRect, EditorGUIUtility.IconContent( NO_SYNC_ICON ) );
                else
                    entry.isAsync = GUI.Toggle( syncRect, entry.isAsync || entry.isAsyncOnly, string.Empty );
            }

            entry.isSkipped = GUI.Toggle( skipRect, entry.isSkipped, string.Empty );

            GUI.Label( entryRect, entry.name );
        }

        GUIContent GetStatusIcon( ReorderableEntry entry )
        {
            if ( entry.isSkipped )
                return EditorGUIUtility.IconContent( SKIP_ICON );

            return EditorGUIUtility.IconContent( entry.isAsync ? ASYNC_ICON : SYNC_ICON );
        }

        void DrawListHeader( Rect rect )
        {
            Rect offsetted = new Rect(
                rect.x + LIST_HEADER_OFFSET,
                rect.y,
                rect.width - LIST_HEADER_OFFSET,
                rect.height
            );
            SplitRect( offsetted, out Rect statusRect, out Rect syncRect, out Rect skipRect, out Rect entryRect );

            EditorGUI.LabelField( statusRect, "Status", EditorStyles.miniLabel );
            EditorGUI.LabelField( syncRect, "Async", EditorStyles.miniLabel);
            EditorGUI.LabelField( skipRect, "Skip", EditorStyles.miniLabel );
            EditorGUI.LabelField( entryRect, "Name" );
        }

        void SplitRect( Rect rect, out Rect statusRect, out Rect syncRect, out Rect skipRect, out Rect entryRect )
        {
            const float ENTRY_OFFSET = 2 * WIDTH_CHECKBOX + WIDTH_STATUS;

            syncRect = new Rect( rect.x, rect.y, WIDTH_CHECKBOX, rect.height );
            skipRect = new Rect( rect.x + WIDTH_CHECKBOX, rect.y, WIDTH_CHECKBOX, rect.height );
            statusRect = new Rect( rect.x + WIDTH_CHECKBOX * 2, rect.y, WIDTH_STATUS, rect.height );
            entryRect = new Rect(
                rect.x + ENTRY_OFFSET,
                rect.y,
                rect.width - ENTRY_OFFSET,
                rect.height );
        }

        void DrawHUFHeader()
        {
            using ( new GUILayout.HorizontalScope() )
            {
                EditorGUILayout.LabelField( AbstractConfigEditor.HeaderContent, EditorStyles.boldLabel );
            }
        }

        void DrawRefreshButton()
        {
            if ( !GUILayout.Button( "Refresh / Revert" ) )
                return;

            BuildList();
        }

        void DrawSaveButton()
        {
            if ( !GUILayout.Button( "Save" ) )
                return;

            try
            {
                UpdateEntries( GetOrderedList() );
                HLog.LogImportant( logPrefix, "Order saved successfully" );
            }
            catch ( Exception exception )
            {
                HLog.LogError( logPrefix, $"Error saving order: {exception.Message}" );
            }
        }

        void DrawDefaultsButton()
        {
            if ( !GUILayout.Button( "Restore defaults" ) )
                return;

            if ( !EditorUtility.DisplayDialog(
                "Restoring defaults",
                "Are you sure you want to restore the default values? This will wipe order of all custom initializers.",
                "YES",
                "CANCEL" ) )
                return;

            CopyDefaults();
            Load();
            BuildList();
        }

        void DrawPipelineButton()
        {
            if ( !GUILayout.Button( "Disable all autoinits" ) )
                return;

            if ( !EditorUtility.DisplayDialog(
                "Switching to HUF Initialization Pipeline",
                $"Are you sure you want to disable auto-init in all configs? Only configs inheriting from {nameof(FeatureConfigBase)} will be affected.",
                "YES",
                "CANCEL" ) )
                return;

            bool exceptHips = EditorUtility.DisplayDialog(
                "Switching to HUF Initialization Pipeline",
                $"Do you want the pipeline to run automatically or manually?",
                "AUTOMATICALLY",
                "MANUALLY" );

            DisableAutoinits( exceptHips );
            BuildList();
        }

        [UsedImplicitly]
        void DrawDefineWarning()
        {
            const string WARNING = "You need to have HUFEXT_MODULE_STARTER defined in your project. Use Menu \"HUF/Utils/Rebuild Defines\" for best results.";

            EditorGUILayout.HelpBox( WARNING, MessageType.Warning );
        }

        // ReSharper disable once UnusedMember.Local // For development
        void DrawDebugButton()
        {
            if ( !GUILayout.Button( "Debug" ) )
                return;

            HInitializationPipeline.RunPipeline();
        }

        List<OrderEntry> GetOrderedList()
        {
            var list = new List<OrderEntry>(entries.Count);

            foreach ( var entry in entries )
            {
                list.Add( new OrderEntry
                {
                    id = entry.name,
                    isAsync = entry.isAsync,
                    isSkipped = entry.isSkipped
                } );
            }

            return list;
        }

        class ReorderableEntry
        {
            public string name;
            public bool isAsync;
            public bool isAsyncOnly;
            public bool isSkipped;
        }
    }
}