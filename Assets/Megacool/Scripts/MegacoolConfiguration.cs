using UnityEngine;
using System.Collections;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;

[UnityEditor.InitializeOnLoad]
#endif
[System.Serializable]

public class MegacoolConfiguration : ScriptableObject {

    private static MegacoolConfiguration instance;
    private static readonly string configurationAsset = "MegacoolConfiguration";

    // Configuration properties that should be exposed in the inspector. Ordering
    // and grouping is done by MegacoolEditor.
    public string appIdentifier;

    public string appConfigAndroid;

    public string appConfigIos;

    [Tooltip("Set the text to be shared on different channels.")]
    public string sharingText = "No way you can beat my score!";

    public bool universalLinksIOS = true;
    public bool universalLinksAndroid = true;
    public string sourceDomain = "mgcl.co";

    [Tooltip("Which URL scheme to respond to as fallback where normal links doesn't work")]
    public string schemeIOS;

    [Tooltip("Which URL scheme to respond to as fallback where normal links doesn't work")]
    public string schemeAndroid;

    [Tooltip("Whether to register as a referral receiver in the Android manifest. Needed for referrals to work on Android.")]
    public bool androidReferrals = true;

    [ContextMenuItem("Default", "defaultMaxFrames")]
    [Tooltip("Max number of frames in the buffer.")]
    public int maxFrames = 50;

    [ContextMenuItem("Default", "defaultPeakLocation")]
    [Tooltip("Set at what percentage of recording the maximum score should occur.")]
    public double peakLocation = 0.7;

    [ContextMenuItem("Default", "defaultFrameRate")]
    [Tooltip("Set numbers of frames per second to record.")]
    public float recordingFrameRate = 10.0f;

    [ContextMenuItem("Default", "defaultPlaybackFrameRate")]
    [Tooltip("Set numbers of frames per second to play.")]
    public float playbackFrameRate = 10.0f;

    [ContextMenuItem("Default", "defaultLastFrameDelay")]
    [Tooltip("Set a delay (in milliseconds) on the last frame in the animation.")]
    public int lastFrameDelay = 1000;

    [ContextMenuItem("Default", "defaultGifColorTable")]
    [Tooltip("Fixed: The colors in the GIF will be based on a broad spectrum of 256 fixed colors.\n" +
        "Analyze First: The colors in the GIF will be based on the first frame.")]
    public Megacool.GifColorTableType gifColorTable;

    [Tooltip("Turn on / off debug mode. In debug mode calls to the SDK are stored and can be submitted to the core developers using SubmitDebugData later.")]
    public bool debugMode = false;

    private void defaultMaxFrames() {
        maxFrames = 50;
    }

    private void defaultPeakLocation() {
        peakLocation = 0.7;
    }

    private void defaultFrameRate() {
        recordingFrameRate = 10.0f;
    }

    private void defaultPlaybackFrameRate() {
        playbackFrameRate = 10.0f;
    }

    private void defaultLastFrameDelay() {
        lastFrameDelay = 1;
    }

    public static MegacoolConfiguration Instance {
        get {
            if (instance == null) {
                LoadInstance();
            }
            return instance;
        }
    }


    private static void LoadInstance() {
        instance = Resources.Load(configurationAsset) as MegacoolConfiguration;
        if (instance == null) {
            instance = CreateInstance<MegacoolConfiguration>();

#if UNITY_EDITOR
            if (!AssetDatabase.IsValidFolder("Assets/Megacool")) {
                AssetDatabase.CreateFolder("Assets", "Megacool");
            }
            if (!AssetDatabase.IsValidFolder("Assets/Megacool/Resources")) {
                AssetDatabase.CreateFolder("Assets/Megacool", "Resources");
            }
            string configurationAssetPath = Path.Combine("Assets/Megacool/Resources", configurationAsset + ".asset");
            AssetDatabase.CreateAsset(instance, configurationAssetPath);
#endif
        }
    }
}
