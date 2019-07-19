using UnityEngine;
using UnityEditor;
using Crosstales.OnlineCheck.EditorUtil;

namespace Crosstales.OnlineCheck.EditorExtension
{
    /// <summary>Custom editor for the 'OnlineCheck'-class.</summary>
    [InitializeOnLoad]
    [CustomEditor(typeof(OnlineCheck))]
    public class OnlineCheckEditor : Editor
    {
        #region Variables

        private OnlineCheck script;

        #endregion


        #region Static constructor

        static OnlineCheckEditor()
        {
            EditorApplication.hierarchyWindowItemOnGUI += hierarchyItemCB;
        }

        #endregion


        #region Editor methods

        public void OnEnable()
        {
            script = (OnlineCheck)target;
            onUpdate();
        }

        public void OnDisable()
        {
            EditorApplication.update -= onUpdate;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorHelper.SeparatorUI();

            if (script.isActiveAndEnabled)
            {
                if (script.CustomCheck != null && !string.IsNullOrEmpty(script.CustomCheck.URL) && (script.CustomCheck.URL.Contains("crosstales.com") || script.CustomCheck.URL.Contains("207.154.226.218")))
                {
                    EditorGUILayout.HelpBox("'Custom Check' uses 'crosstales.com' for detection: this is only allowed for test-builds and the check interval will be limited!" + System.Environment.NewLine + "Please use your own URL for detection.", MessageType.Warning);
                    EditorHelper.SeparatorUI();
                }

                onUpdate();
                GUILayout.Label("Internet Status", EditorStyles.boldLabel);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Available:");
                    GUI.enabled = false;
                    EditorGUILayout.Toggle(new GUIContent(string.Empty, "Is Internet currently available?"), OnlineCheck.isInternetAvailable);
                    GUI.enabled = true;
                }
                GUILayout.EndHorizontal();

                EditorHelper.SeparatorUI();

                GUILayout.Label("Checks", EditorStyles.boldLabel);
                GUILayout.Label("Last checked:\t" + OnlineCheck.LastCheck.ToString());
                GUILayout.Label("Total:\t\t" + Util.Context.NumberOfChecks.ToString());

                if (!Util.Helper.isEditorMode)
                {
                    GUILayout.Label("Per Minute:\t" + Util.Context.ChecksPerMinute.ToString("#0.0"));
                    GUILayout.Label("Data Downloaded:\t" + Util.Helper.FormatBytesToHRF(OnlineCheck.DataDownloaded).ToString());
                    EditorHelper.SeparatorUI();

                    GUILayout.Label("Timers", EditorStyles.boldLabel);
                    GUILayout.Label("Runtime:\t\t" + Util.Helper.FormatSecondsToHourMinSec(Util.Context.Runtime));
                    GUILayout.Label("Uptime:\t\t" + Util.Helper.FormatSecondsToHourMinSec(Util.Context.Uptime));
                    GUILayout.Label("Downtime:\t\t" + Util.Helper.FormatSecondsToHourMinSec(Util.Context.Downtime));
                }

                if (Util.Helper.isEditorMode)
                {
                    if (GUILayout.Button(new GUIContent(" Refresh", EditorHelper.Icon_Reset, "Restart the Internet availability check.")))
                    {
                        OnlineCheck.Refresh();
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Script is disabled!", MessageType.Info);
            }
        }
#endregion


#region Private methods

        private void onUpdate()
        {
            Repaint();
        }

        private static void hierarchyItemCB(int instanceID, Rect selectionRect)
        {
            if (EditorConfig.HIERARCHY_ICON)
            {
                GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

                if (go != null && go.GetComponent<OnlineCheck>())
                {
                    Rect r = new Rect(selectionRect);
                    r.x = r.width - 4;

                    //Debug.Log("HierarchyItemCB: " + r);

                    GUI.Label(r, EditorHelper.Logo_Asset_Small);
                }
            }
        }

#endregion

    }
}
// © 2017-2019 crosstales LLC (https://www.crosstales.com)