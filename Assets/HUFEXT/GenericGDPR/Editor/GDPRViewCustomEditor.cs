using HUFEXT.GenericGDPR.Runtime.Views;
using UnityEditor;
using UnityEngine;

namespace HUFEXT.GenericGDPR.Editor
{
    [CustomEditor(typeof(GDPRView))]
    class GDPRViewCustomEditor : UnityEditor.Editor 
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if ( GUILayout.Button( "Reload Colors" ) )
            {
                ( target as GDPRView )?.Refresh();
                EditorUtility.SetDirty( target );
            }
        }
    }
}