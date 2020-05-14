using HUF.Utils.Runtime.BuildSupport.ConfigPreset;
using UnityEditor;
using UnityEngine;

namespace HUF.Utils.Editor.BuildSupport.ConfigPreset
{
    [CustomEditor(typeof(ConfigPresets))]
    public class ConfigPresetsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Apply Preset"))
            {
                ConfigPresets.ApplyConfigPresets(Debug.isDebugBuild);
            }
        }
    }
}