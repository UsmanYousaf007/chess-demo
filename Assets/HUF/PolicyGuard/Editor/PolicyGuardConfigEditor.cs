using System;
using System.Collections.Generic;
using HUF.GenericDialog.Runtime.Configs;
using HUF.PolicyGuard.Runtime.Configs;
using HUF.PolicyGuard.Runtime.Configs.Data;
using HUF.Utils.Editor.Configs;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace HUF.PolicyGuard.Editor
{
    [CustomEditor( typeof(PolicyGuardConfig), true )]
    public class PolicyGuardConfigEditor : AbstractConfigEditor
    {
        const float LIST_HEADER_OFFSET = 13;
        const float CATEGORY_OFFSET = 5;
        const float FIELD_WIDTH = 150;
        const float WIDTH_CONFIG = 150;
        const float WIDTH_TYPE_AND_NAME = 150;
        const float WIDTH_PREF_KEY = 150;
        const float WIDTH_AUTO_SET_PREF = 50;

        ReorderableList androidReorderableList = null;
        ReorderableList iOSReorderableList = null;
        PolicyGuardConfig config;
        Platform currentPlatform;

        enum Platform
        {
            Android,
            iOS
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();


            config.UseCustomAutomatedPopupsFlow =
                EditorGUILayout.Toggle( "Use custom automated pop-ups flow per platform",
                    config.UseCustomAutomatedPopupsFlow );

            if ( !config.UseCustomAutomatedPopupsFlow )
                return;

            foreach ( Platform platform in Enum.GetValues( typeof(Platform) ) )
            {
                currentPlatform = platform;
                var popupsFlow = GetListForPlatform( platform, config );

                if ( popupsFlow.Count == 0 )
                    GenerateDefaultFlow( popupsFlow, platform );
                DrawPlatformFlow( GetReorderableListForPlatform( platform, config ), platform );
            }
        }

        static List<PolicyPopup> GetListForPlatform( Platform platform, PolicyGuardConfig config )
        {
            switch ( platform )
            {
                case Platform.Android:
                    return config.AndroidPopupsFlow;
                case Platform.iOS:
                    return config.IOSPopupsFlow;
                default:
                    return null;
            }
        }

        static void GenerateDefaultFlow( [NotNull] ICollection<PolicyPopup> flow, Platform platform )
        {
            if ( platform == Platform.Android )
            {
                flow.Add( new PolicyPopup( PolicyPopup.PopupType.GDPRWithAds ) );
                return;
            }

            flow.Add( new PolicyPopup( PolicyPopup.PopupType.GDPR ) );
            flow.Add( new PolicyPopup( PolicyPopup.PopupType.ATT ) );
            flow.Add( new PolicyPopup( PolicyPopup.PopupType.PersonalizedAds ) );
        }

        static void DrawPlatformFlow( ReorderableList listToDraw, Platform platform )
        {
            using ( new GUILayout.VerticalScope( EditorStyles.helpBox ) )
            {
                using ( new GUILayout.HorizontalScope() )
                {
                    GUILayout.Label( platform.ToString() );
                }

                listToDraw.DoLayoutList();
            }
        }

        static void SplitRect( Rect rect,
            out Rect nameRect,
            out Rect configRect,
            out Rect prefsRect,
            out Rect typeRect,
            out Rect setPrefsRect )
        {
            nameRect = new Rect( rect.x, rect.y, WIDTH_TYPE_AND_NAME, rect.height );
            float offset = CATEGORY_OFFSET + WIDTH_TYPE_AND_NAME;
            typeRect = new Rect( rect.x + offset, rect.y, FIELD_WIDTH, rect.height );
            offset += CATEGORY_OFFSET + FIELD_WIDTH;
            configRect = new Rect( rect.x + offset, rect.y, WIDTH_CONFIG, rect.height );
            offset += CATEGORY_OFFSET + WIDTH_CONFIG;
            prefsRect = new Rect( rect.x + offset, rect.y, WIDTH_PREF_KEY, rect.height );
            offset += CATEGORY_OFFSET + WIDTH_PREF_KEY;
            setPrefsRect = new Rect( rect.x + offset, rect.y, WIDTH_AUTO_SET_PREF, rect.height );
        }

        static void DrawListHeader( Rect rect )
        {
            var offset = new Rect(
                rect.x + LIST_HEADER_OFFSET,
                rect.y,
                rect.width - LIST_HEADER_OFFSET,
                rect.height
            );

            SplitRect( offset,
                out var nameRect,
                out var configRect,
                out var prefsRect,
                out var typeRect,
                out var setPrefsRect );
            EditorGUI.LabelField( nameRect, "Name" );
            EditorGUI.LabelField( typeRect, "Type", EditorStyles.miniLabel );
            EditorGUI.LabelField( configRect, "Config", EditorStyles.miniLabel );
            EditorGUI.LabelField( prefsRect, "Player Prefs Key", EditorStyles.miniLabel );
            EditorGUI.LabelField( setPrefsRect, "Set Prefs", EditorStyles.miniLabel );
        }

        void OnEnable()
        {
            config = (PolicyGuardConfig)serializedObject.targetObject;
        }

        void OnDisable()
        {
            EditorUtility.SetDirty( config );
        }

        ReorderableList GetReorderableListForPlatform( Platform platform, PolicyGuardConfig config )
        {
            ReorderableList reorderableList = null;

            switch ( platform )
            {
                case Platform.Android:
                    reorderableList = androidReorderableList;
                    break;
                case Platform.iOS:
                    reorderableList = iOSReorderableList;
                    break;
            }

            if ( reorderableList != null )
                return reorderableList;

            reorderableList = new ReorderableList( GetListForPlatform( platform, config ), typeof(string) )
            {
                draggable = true,
                displayAdd = true,
                displayRemove = true,
                drawHeaderCallback = DrawListHeader,
                drawElementCallback = DrawEntry
            };

            switch ( platform )
            {
                case Platform.Android:
                    androidReorderableList = reorderableList;
                    break;
                case Platform.iOS:
                    iOSReorderableList = reorderableList;
                    break;
            }

            return reorderableList;
        }

        void DrawEntry( Rect rect, int index, bool isActive, bool isFocused )
        {
            SplitRect( rect,
                out var nameRect,
                out var configRect,
                out var prefsRect,
                out var typeRect,
                out var setPrefsRect );
            var listForPlatform = GetListForPlatform( currentPlatform, config );
            var popup = listForPlatform[index];

            if ( popup.type == PolicyPopup.PopupType.Custom )
                popup.name = GUI.TextField( nameRect, popup.name );
            else
                GUI.Label( nameRect, popup.type.ToString() );
            popup.type = (PolicyPopup.PopupType)EditorGUI.EnumPopup( typeRect, popup.type );

            using ( new EditorGUI.DisabledScope( popup.type != PolicyPopup.PopupType.Custom ) )
            {
                popup.popupConfig = (HGenericDialogConfig)EditorGUI.ObjectField( configRect,
                    popup.popupConfig,
                    typeof(HGenericDialogConfig),
                    false );
                popup.playerPrefsKey = GUI.TextField( prefsRect, popup.playerPrefsKey );
                popup.setKeyAutomatically = GUI.Toggle( setPrefsRect, popup.setKeyAutomatically, string.Empty );
            }
        }
    }
}