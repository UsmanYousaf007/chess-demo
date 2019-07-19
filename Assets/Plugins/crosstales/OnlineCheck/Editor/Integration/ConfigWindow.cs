using UnityEditor;
using UnityEngine;
using Crosstales.OnlineCheck.EditorUtil;

namespace Crosstales.OnlineCheck.EditorIntegration
{
    /// <summary>Editor window extension.</summary>
    [InitializeOnLoad]
    public class ConfigWindow : ConfigBase
    {

        #region Variables

        private int tab = 0;
        private int lastTab = 0;

        private Vector2 scrollPosPrefabs;
        private Vector2 scrollPosTD;

        #endregion


        #region EditorWindow methods

        [MenuItem("Tools/" + Util.Constants.ASSET_NAME + "/Configuration...", false, EditorHelper.MENU_ID + 1)]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(ConfigWindow));
        }

        public static void ShowWindow(int tab)
        {
            ConfigWindow window = EditorWindow.GetWindow(typeof(ConfigWindow)) as ConfigWindow;
            window.tab = tab;
        }

        public void OnEnable()
        {
            titleContent = new GUIContent(Util.Constants.ASSET_NAME_SHORT, EditorHelper.Logo_Asset_Small);
        }

        public void OnGUI()
        {
            tab = GUILayout.Toolbar(tab, new string[] { "Config", "Prefabs", "TD", "Help", "About" });

            if (tab != lastTab)
            {
                lastTab = tab;
                GUI.FocusControl(null);
            }

            if (tab == 0)
            {
                showConfiguration();

                EditorHelper.SeparatorUI();

                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button(new GUIContent(" Save", EditorHelper.Icon_Save, "Saves the configuration settings for this project")))
                    {
                        save();
                    }

                    if (GUILayout.Button(new GUIContent(" Reset", EditorHelper.Icon_Reset, "Resets the configuration settings for this project.")))
                    {
                        if (EditorUtility.DisplayDialog("Reset configuration?", "Reset the configuration of " + Util.Constants.ASSET_NAME + "?", "Yes", "No"))
                        {
                            Util.Config.Reset();
                            EditorConfig.Reset();
                            save();
                        }
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(6);
            }
            else if (tab == 1)
            {
                showPrefabs();
            }
            else if (tab == 2)
            {
                showTestDrive();
            }
            else if (tab == 3)
            {
                showHelp();
            }
            else
            {
                showAbout();
            }
        }

        public void OnInspectorUpdate()
        {
            Repaint();
        }

        #endregion


        #region Private methods

        private void showPrefabs()
        {
            scrollPosPrefabs = EditorGUILayout.BeginScrollView(scrollPosPrefabs, false, false);
            {
                GUILayout.Label("Available Prefabs", EditorStyles.boldLabel);

                GUILayout.Space(6);

                if (!EditorHelper.isOnlineCheckInScene || !EditorHelper.isProxyInScene)
                {
                    if (!EditorHelper.isOnlineCheckInScene)
                    {

                        GUILayout.Label(Util.Constants.ASSET_NAME);

                        if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds an " + Util.Constants.ONLINECHECK_SCENE_OBJECT_NAME + "-prefab to the scene.")))
                        {
                            EditorHelper.InstantiatePrefab(Util.Constants.ONLINECHECK_SCENE_OBJECT_NAME);
                        }
                    }

                    if (!EditorHelper.isOnlineCheckInScene && !EditorHelper.isProxyInScene)
                    {
                        EditorHelper.SeparatorUI();
                    }

                    if (!EditorHelper.isProxyInScene)
                    {

                        GUILayout.Label(Util.Constants.PROXY_SCENE_OBJECT_NAME);
                        if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds a 'Proxy'-prefab to the scene.")))
                        {
                            EditorHelper.InstantiatePrefab(Util.Constants.PROXY_SCENE_OBJECT_NAME);
                        }
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("All available prefabs are already in the scene.", MessageType.Info);
                }

                GUILayout.Space(6);
            }
            EditorGUILayout.EndScrollView();
        }

        private void showTestDrive()
        {
            GUILayout.Space(3);
            GUILayout.Label("Test-Drive", EditorStyles.boldLabel);

            if (Util.Helper.isEditorMode)
            {
                if (EditorHelper.isOnlineCheckInScene)
                {
                    scrollPosTD = EditorGUILayout.BeginScrollView(scrollPosTD, false, false);
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("Internet available:");
                            GUI.enabled = false;
                            EditorGUILayout.Toggle(new GUIContent(string.Empty, "Is Internet currently available?"), OnlineCheck.isInternetAvailable);
                            GUI.enabled = true;
                        }
                        GUILayout.EndHorizontal();

                        EditorHelper.SeparatorUI();

                        GUILayout.Label("Checks", EditorStyles.boldLabel);
                        GUILayout.Label("Last checked:\t" + OnlineCheck.LastCheck.ToString());
                        GUILayout.Label("Total:\t\t" + Util.Context.NumberOfChecks.ToString());


                    }
                    if (!Util.Helper.isEditorMode)
                    {
                        GUILayout.Label("Per Minute:\t" + Util.Context.ChecksPerMinute.ToString("#0.0"));
                        GUILayout.Label("Data downloaded:\t" + Util.Helper.FormatBytesToHRF(OnlineCheck.DataDownloaded).ToString());

                        EditorHelper.SeparatorUI();

                        GUILayout.Label("Timers", EditorStyles.boldLabel);
                        GUILayout.Label("Runtime:\t\t" + Util.Helper.FormatSecondsToHourMinSec(Util.Context.Runtime));
                        GUILayout.Label("Uptime:\t\t" + Util.Helper.FormatSecondsToHourMinSec(Util.Context.Uptime));
                        GUILayout.Label("Downtime:\t\t" + Util.Helper.FormatSecondsToHourMinSec(Util.Context.Downtime));
                    }

                    EditorHelper.SeparatorUI();

                    EditorGUILayout.EndScrollView();

                    EditorHelper.SeparatorUI();

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
                    EditorHelper.OCUnavailable();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Disabled in Play-mode!", MessageType.Info);
            }
        }

        #endregion
    }
}
// © 2017-2019 crosstales LLC (https://www.crosstales.com)