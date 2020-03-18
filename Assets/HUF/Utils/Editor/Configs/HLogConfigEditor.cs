using HUF.Utils.Runtime.Logging;
using UnityEditor;
using UnityEngine;

namespace HUF.Utils.Configs.API.Editor
{
    [CustomEditor( typeof(HLogConfig), true )]
    public class HLogConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            if ( GUILayout.Button( "Refresh Config" ) )
            {
                HLog.RefreshConfig();
            }
        }
    }
}