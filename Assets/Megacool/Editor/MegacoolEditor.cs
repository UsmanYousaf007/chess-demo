using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Collections;


[InitializeOnLoad]
class Startup {
    static Startup() {
#if UNITY_ANDROID
        AndroidSdkVersions minSupportedApiVersion = AndroidSdkVersions.AndroidApiLevel18;
        AndroidSdkVersions apiVersion = PlayerSettings.Android.minSdkVersion;
        if (apiVersion < minSupportedApiVersion) {
            string minApiError = "The application's minimum API version (" + apiVersion + ") is less than the required API version by Megacool (" +
                minSupportedApiVersion + ")";

            if (EditorUtility.DisplayDialog("Megacool API", minApiError,
                "Change to " + minSupportedApiVersion, "Cancel")) {
                    PlayerSettings.Android.minSdkVersion = minSupportedApiVersion;
            }
        }
#endif
    }
}

[CustomEditor(typeof(MegacoolConfiguration))]
public class MegacoolEditor : Editor {
    private SerializedObject serializedConfiguration;
    private SerializedProperty serializedAppIdentifier;
    private SerializedProperty serializedAppConfigAndroid;
    private SerializedProperty serializedAppConfigIos;
    private SerializedProperty serializedSharingText;
    private SerializedProperty serializedSchemeIOS;
    private SerializedProperty serializedSchemeAndroid;
    private SerializedProperty serializedMaxFrames;
    private SerializedProperty serializedPeakLocation;
    private SerializedProperty serializedRecordingFrameRate;
    private SerializedProperty serializedPlaybackFrameRate;
    private SerializedProperty serializedLastFrameDelay;
    private SerializedProperty serializedGifColorTable;
    private SerializedProperty serializedUniversalLinksIOS;
    private SerializedProperty serializedUniversalLinksAndroid;
    private SerializedProperty serializedAndroidReferrals;

    void OnEnable () {
        serializedConfiguration = new SerializedObject(MegacoolConfiguration.Instance);
        serializedAppIdentifier = serializedConfiguration.FindProperty("appIdentifier");
        serializedAppConfigAndroid = serializedConfiguration.FindProperty("appConfigAndroid");
        serializedAppConfigIos = serializedConfiguration.FindProperty("appConfigIos");
        serializedSharingText = serializedConfiguration.FindProperty("sharingText");
        serializedSchemeIOS = serializedConfiguration.FindProperty("schemeIOS");
        serializedSchemeAndroid = serializedConfiguration.FindProperty("schemeAndroid");
        serializedMaxFrames = serializedConfiguration.FindProperty("maxFrames");
        serializedPeakLocation = serializedConfiguration.FindProperty("peakLocation");
        serializedRecordingFrameRate = serializedConfiguration.FindProperty("recordingFrameRate");
        serializedPlaybackFrameRate = serializedConfiguration.FindProperty("playbackFrameRate");
        serializedLastFrameDelay = serializedConfiguration.FindProperty("lastFrameDelay");
        serializedGifColorTable = serializedConfiguration.FindProperty("gifColorTable");
        serializedUniversalLinksIOS = serializedConfiguration.FindProperty("universalLinksIOS");
        serializedUniversalLinksAndroid = serializedConfiguration.FindProperty("universalLinksAndroid");
        serializedAndroidReferrals = serializedConfiguration.FindProperty("androidReferrals");
    }

    public override void OnInspectorGUI() {
        serializedConfiguration.Update();

        addHeader("Core config");
        addProperty(serializedAppIdentifier, "Megacool identifier");
        addHelpBox("This is the identifier you picked for your app. It must match what " +
            "you've entered on the dashboard.");

        addProperty(serializedAppConfigAndroid, "Android key");
        addProperty(serializedAppConfigIos, "iOS key");
        addHelpBox("The properties above are required, you can find them on your dashboard at " +
            "https://dashboard.megacool.co");

        addHeader("Linking");
        addLabel("Scheme");
        addProperty(serializedSchemeIOS, "    iOS");
        addProperty(serializedSchemeAndroid, "    Android");
        addProperty(serializedAndroidReferrals, "Android referrals");
        addLabel("Universal linking");
        addProperty(serializedUniversalLinksIOS, "    iOS");
        addProperty(serializedUniversalLinksAndroid, "    Android");

        addHelpBox("The properties below can also be set programmatically, see the documentation for details");
        addHeader("Recording");
        addProperty(serializedLastFrameDelay, "Last frame delay (ms)");
        addProperty(serializedRecordingFrameRate, "Recording frame rate");
        addProperty(serializedPlaybackFrameRate, "Playback frame rate");
        addProperty(serializedMaxFrames, "Max frames");
        addProperty(serializedPeakLocation, "Peak location");
        addProperty(serializedGifColorTable, "GIF color table");

        addHeader("Shares");
        addProperty(serializedSharingText, "Default share text");

        if (GUILayout.Button("Save changes")) {
            serializedConfiguration.ApplyModifiedProperties();

            if (MegacoolAndroidManifestEditor.IsDefaultApplicationIdentifer()) {
                // Fail hard if this is unset since otherwise it'll build successfully but only fail once a share is
                // attempted, making it easy to ship something broken.
                Debug.LogError("The Android Bundle Identifier seems to be unset, ensure it's set in the " +
                    "Android Player Settings. This is necessary for GIF sharing to work.");
                return;
            }

			MegacoolAndroidManifestEditor.ReplaceMegacoolManifestApplicationId();

			MegacoolAndroidManifestEditor.WriteStringsDotXML(
				serializedAppIdentifier.stringValue,
				serializedUniversalLinksAndroid.boolValue,
				serializedSchemeAndroid.stringValue
			);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Megacool configuration saved! Remember to commit the resulting AndroidManifest.xml, " +
                "strings.xml and the MegacoolConfiguration.asset!");
        }

        if (GUI.changed) {
            serializedConfiguration.ApplyModifiedProperties();
        }
    }

    [MenuItem("Megacool/Configuration")]
    public static void GenerateMegacoolConfiguration() {
        MegacoolConfiguration configuration = MegacoolConfiguration.Instance;
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = configuration;
    }

    private void addProperty(SerializedProperty serializedProperty, string label) {
        EditorGUILayout.PropertyField(serializedProperty, new GUIContent(label), new GUILayoutOption[]{});
    }

    private void addHeader(string header) {
        EditorGUILayout.LabelField(header, EditorStyles.boldLabel, new GUILayoutOption[]{});
    }

    private void addLabel(string label) {
        EditorGUILayout.LabelField(label, new GUILayoutOption[]{});
    }

    private void addHelpBox(string helpText) {
        EditorGUILayout.HelpBox(helpText, MessageType.Info);
    }

    private void addWarning(string warning) {
        EditorGUILayout.HelpBox(warning, MessageType.Warning);
    }
}
