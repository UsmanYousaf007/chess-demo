using System;
using HUFEXT.GenericGDPR.Runtime.Utils;
using HUFEXT.GenericGDPR.Runtime.Views;
using UnityEditor;
using UnityEngine;

namespace HUFEXT.GenericGDPR.Editor
{
    [CustomEditor(typeof(GDPRView))]
    class GDPRViewCustomEditor : UnityEditor.Editor
    {
        private GUIContent refreshContent;
        private string forceLang = string.Empty;
        private GDPRView view;
        public string[] options;
        public int index = 0;

        private void OnEnable()
        {
            refreshContent = new GUIContent
            {
                text  = " Refresh View",
                image = EditorGUIUtility.IconContent( "d_Refresh" ).image
            };

            view = ( GDPRView ) target;

            var translations = GDPRTranslationsProvider.Translations;
            options = new string[translations.Count + 1];
            options[0] = "None";
            for ( int i = 1; i < translations.Count; ++i )
            {
                options[i] = translations[i - 1].lang;
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if ( view == null )
            {
                return;
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField( "Debug", EditorStyles.boldLabel );
            index = EditorGUILayout.Popup( "Preview Language", index, options );
            if ( GUILayout.Button( refreshContent ) )
            {
                view.Refresh( index != 0 ? options[index] : "" );
                EditorUtility.SetDirty( target );
            }
            EditorGUILayout.Space();
        }
    }
}