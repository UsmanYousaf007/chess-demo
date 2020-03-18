using UnityEditor;
using UnityEngine;

namespace HUF.Utils.Wrappers.BuildSupport.ConfigPreset.Editor
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