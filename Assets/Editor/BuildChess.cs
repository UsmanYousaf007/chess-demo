using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System;
using TurboLabz.TLUtils;

public class BuildChess : MonoBehaviour
{ 
    static string BUILD_OUTPUT_PATH = "/build";
    static string BUILD_OUTPUT_ANDROID_SUBPATH = "/Android";
    static string BUILD_OUTPUT_IOS_SUBPATH = "/iOS";

    static string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);

    static string androidAPK = "chessstar";
    static string bundleVersion = GetBundleVersion(); // PlayerSettings.bundleVersion;
    static string[] gameScenes = new string[] { "Game" };
    static string[] gameScenFiles = new string[] {
            "Assets/InstantFramework/Scenes/Splash.unity",
            "Assets/Game/Scenes/Game.unity"
            };

    static string bundleVersionCodeiOS = GetIOSBundleCode(); //PlayerSettings.iOS.buildNumber;
    static string bundleVersionCodeAndroid = GetAndroidBundleCode(); //PlayerSettings.Android.bundleVersionCode.ToString();

    static string GameAnalyticsInternalBuildName = "internal";

    private static void ProcessArgs()
    {
        LogUtil.Log("Process Args", "yellow");
        string[] args = Environment.GetCommandLineArgs();
        int i = 0;
        foreach (string a in args)
        {
            LogUtil.Log("Args: " + a);

            if (a == "-inBundleVersion")
            {
                bundleVersion = args[i + 1];
                LogUtil.Log("bundleVersion: " + bundleVersion);
            }
            else if (a == "-inBundleVersionCode")
            {
                bundleVersionCodeiOS = args[i + 1];
                bundleVersionCodeAndroid = args[i + 1];
                LogUtil.Log("bundleVersionCode: " + args[i + 1]);
            }
            i++;
        }
    }

    //This function set the backend envirnoment
    private static void SetGameEnvironment()
    {
        int envNumber = 0;

        string versionNumber = Environment.GetEnvironmentVariable("GS_BACKEND_ENV_NUM");

        if (versionNumber != null){
            envNumber = Int32.Parse(versionNumber);
        }

        LogUtil.Log("UNITY _____ envNumber : " + envNumber);

        if (envNumber == (int)GameSparksConfig.Environment.LivePreview)
        {
            ChessTools.SetGamesparksEnvLivePreview(); 
        }
        else if (envNumber == (int)GameSparksConfig.Environment.Live)
        {
            ChessTools.SetGamesparksEnvLive();
        }
        else if (envNumber == (int)GameSparksConfig.Environment.URLBased)
        {
            ChessTools.SetGamesparksEnvURLBased();
        }
        else if (envNumber == (int)GameSparksConfig.Environment.Sami)
        {
            ChessTools.SetGamesparksEnvSami();
        }
        else
        {
            ChessTools.SetGamesparksEnvDevelopment();
        }

    }

    private static void ProcessSkinLinks()
    {
        LogUtil.Log("Process skin links", "yellow");
        Scene scene = EditorSceneManager.GetSceneByName(gameScenes[0]);
        ClearSkinLinks.Init();
        EditorSceneManager.SaveScene(scene);
    }

    private static void ProcessBuild(BuildPlayerOptions buildPlayerOptions)
    {
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            LogUtil.Log("Build succeeded: " + summary.totalSize + " bytes", "cyan");
        }

        if (summary.result == BuildResult.Failed)
        {
            LogUtil.Log("Build failed", "red");
        }
    }

    public static void BuildPlayerSettingsiOS()
    {
        PlayerSettings.iOS.allowHTTPDownload = false;
        PlayerSettings.iOS.appInBackgroundBehavior = iOSAppInBackgroundBehavior.Suspend;
        PlayerSettings.iOS.appleDeveloperTeamID = "WAJ9ZPXALN";
        PlayerSettings.iOS.appleEnableAutomaticSigning = true;
        PlayerSettings.iOS.applicationDisplayName = "Chess Stars";
        PlayerSettings.iOS.backgroundModes = iOSBackgroundMode.None;
        PlayerSettings.iOS.cameraUsageDescription = "";
        PlayerSettings.iOS.deferSystemGesturesMode = UnityEngine.iOS.SystemGestureDeferMode.None;
        PlayerSettings.iOS.disableDepthAndStencilBuffers = false;
        // PlayerSettings.iOS.exitOnSuspend = false; // depricated
        PlayerSettings.iOS.forceHardShadowsOnMetal = false;
        PlayerSettings.iOS.hideHomeButton = false;
        PlayerSettings.iOS.iOSManualProvisioningProfileID = "";
        PlayerSettings.iOS.locationUsageDescription = "";
        PlayerSettings.iOS.microphoneUsageDescription = "";
        // PlayerSettings.iOS.overrideIPodMusic = false; // obsolete
        PlayerSettings.iOS.prerenderedIcon = true;
        PlayerSettings.iOS.requiresFullScreen = true;
        PlayerSettings.iOS.requiresPersistentWiFi = false;
        PlayerSettings.iOS.scriptCallOptimization = ScriptCallOptimizationLevel.FastButNoExceptions;
        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
        PlayerSettings.iOS.showActivityIndicatorOnLoading = iOSShowActivityIndicatorOnLoading.DontShow;
        PlayerSettings.iOS.statusBarStyle = iOSStatusBarStyle.Default;
        PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPhoneAndiPad;
        // PlayerSettings.iOS.targetOSVersion = iOSTargetOSVersion.Unknown; // obsolete
        PlayerSettings.iOS.targetOSVersionString = "11.0";
        // PlayerSettings.iOS.targetResolution = Native; // private
        PlayerSettings.iOS.tvOSManualProvisioningProfileID = "";
        PlayerSettings.iOS.tvOSManualProvisioningProfileType = ProvisioningProfileType.Automatic;
        PlayerSettings.iOS.useOnDemandResources = false;

        //-- Uncomment this to remove 32bit arhitecture from iOS build.
        //PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, 1); // 0 - None, 1 - ARM64, 2 - Universal.

        PlayerSettings.applicationIdentifier = "com.turbolabz.instantchess.ios";
        PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.iOS, ApiCompatibilityLevel.NET_4_6);
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "com.turbolabz.instantchess.ios");
        PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.iOS, ManagedStrippingLevel.Low);
    }

    public static void BuildPlayerSettingsAndroid()
    {
        PlayerSettings.Android.ARCoreEnabled = false;
        PlayerSettings.Android.androidIsGame = true;
        PlayerSettings.Android.androidTVCompatibility = false;
        PlayerSettings.Android.blitType = AndroidBlitType.Always;
        PlayerSettings.Android.buildApkPerCpuArchitecture = false;
        PlayerSettings.Android.disableDepthAndStencilBuffers = false;
        PlayerSettings.Android.forceInternetPermission = true;
        PlayerSettings.Android.forceSDCardPermission = false;
        //PlayerSettings.Android.licenseVerification = false; // readonly
        PlayerSettings.Android.maxAspectRatio = 3;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel21;
        PlayerSettings.Android.preferredInstallLocation = AndroidPreferredInstallLocation.Auto;
        PlayerSettings.Android.renderOutsideSafeArea = false;
        PlayerSettings.Android.showActivityIndicatorOnLoading = AndroidShowActivityIndicatorOnLoading.DontShow;
        PlayerSettings.Android.splashScreenScale = AndroidSplashScreenScale.ScaleToFill;
        PlayerSettings.Android.startInFullscreen = true;
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;
        // PlayerSettings.Android.targetDevice = AndroidTargetDevice.FAT; // obsolete
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
        // PlayerSettings.Android.use24BitDepthBuffer = true; // obsolete
        PlayerSettings.Android.useAPKExpansionFiles = false;
        PlayerSettings.Android.useCustomKeystore = true;

        PlayerSettings.applicationIdentifier = "com.turbolabz.instantchess.android.googleplay";
        PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Android, ApiCompatibilityLevel.NET_4_6);
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.turbolabz.instantchess.android.googleplay");
        PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.Low);
    }

    public static void GASettings(bool isInternal)
    {
        if (isInternal)
        {
            GameAnalyticsSDK.GameAnalytics.SettingsGA.Build.Add(GameAnalyticsInternalBuildName);
            GameAnalyticsSDK.GameAnalytics.SettingsGA.UsePlayerSettingsBuildNumber = false;
            LogUtil.Log("GASettings Version : " + GameAnalyticsInternalBuildName);
        }
        else
        {
            GameAnalyticsSDK.GameAnalytics.SettingsGA.Build.Add(bundleVersion);
            GameAnalyticsSDK.GameAnalytics.SettingsGA.UsePlayerSettingsBuildNumber = true;
            LogUtil.Log("GASettings Version : " + bundleVersion);
        }
    }

    public static void BuildPlayerSettings()
    {
        PlayerSettings.MTRendering = true;
        PlayerSettings.accelerometerFrequency = 0;
        PlayerSettings.actionOnDotNetUnhandledException = ActionOnDotNetUnhandledException.Crash;
        // PlayerSettings.advancedLicense = true; // readonly
        PlayerSettings.allowFullscreenSwitch = true;
        PlayerSettings.allowUnsafeCode = true;
        PlayerSettings.allowedAutorotateToLandscapeLeft = false;
        PlayerSettings.allowedAutorotateToLandscapeRight = false;
        PlayerSettings.allowedAutorotateToPortrait = true;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = true;
        // PlayerSettings.alwaysDisplayWatermark; // private
        PlayerSettings.aotOptions = "";

        //PlayerSettings.iPhoneBundleIdentifier = "com.turbolabz.instantchess.ios"; // obsolete
        //PlayerSettings.targetIOSGraphics = TargetIOSGraphics.Automatic; // obsolete
        //PlayerSettings.SetGraphicsAPIs(BuildTarget.iOS, UnityEngine.Rendering.GraphicsDeviceType.)  // This is set to Auto in UI but no option here

        PlayerSettings.bakeCollisionMeshes = false;
        PlayerSettings.captureSingleScreen = false;
        //PlayerSettings.cloudProjectId = "666aa149-b46-4ee8-9c85-9c55e162a2a2"; // readonly
        PlayerSettings.colorSpace = ColorSpace.Gamma;
        PlayerSettings.companyName = "Turbo Labz";
        PlayerSettings.cursorHotspot = new Vector2(0.0f, 0.0f);
        PlayerSettings.defaultCursor = null;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        PlayerSettings.defaultIsNativeResolution = true;
        PlayerSettings.defaultScreenHeight = 768;
        PlayerSettings.defaultScreenWidth = 1024;
        PlayerSettings.defaultWebScreenHeight = 600;
        PlayerSettings.defaultWebScreenWidth = 960;
        //PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Enabled; // depricated
        PlayerSettings.enable360StereoCapture = false;
        PlayerSettings.enableCrashReportAPI = true;
        PlayerSettings.enableFrameTimingStats = false;
        PlayerSettings.enableInternalProfiler = false;
        PlayerSettings.enableMetalAPIValidation = true;
        //PlayerSettings.firstStreamedLevelWithResources = 0; // private
        PlayerSettings.forceSingleInstance = false;
        PlayerSettings.fullScreenMode = FullScreenMode.FullScreenWindow;
        PlayerSettings.gcIncremental = false;
        PlayerSettings.gpuSkinning = false;
        PlayerSettings.graphicsJobMode = GraphicsJobMode.Native;
        // PlayerSettings.defaultIsFullScreen = true; // obsolete
        PlayerSettings.graphicsJobs = false;

        PlayerSettings.legacyClampBlendShapeWeights = true;
        //PlayerSettings.locationUsageDescription = ""; // obsolete
        PlayerSettings.logObjCUncaughtExceptions = true;
        //PlayerSettings.mobileRenderingPath = RenderingPath.Forward; // obsolete
        PlayerSettings.muteOtherAudioSources = false;
        PlayerSettings.preserveFramebufferAlpha = false;
        //PlayerSettings.productGUID = "??"; // readonly
        PlayerSettings.productName = "Chess Stars";
        PlayerSettings.protectGraphicsMemory = false;
        // PlayerSettings.renderingPath = RenderingPath.Forward; obsolete (used with editor)
        PlayerSettings.resizableWindow = false;
        //PlayerSettings.resolutionDialogBanner = null; // depricated
        PlayerSettings.runInBackground = true;
        PlayerSettings.scriptingRuntimeVersion = ScriptingRuntimeVersion.Latest;
        //PlayerSettings.showUnitySplashScreen = true; // obsolete
        PlayerSettings.SplashScreen.unityLogoStyle = PlayerSettings.SplashScreen.UnityLogoStyle.LightOnDark;
        PlayerSettings.SplashScreen.show = false;
        // PlayerSettings.singlePassStereoRendering = false; // depricated
        //PlayerSettings.splashScreenStyle = SplashScreenStyle.Dark; // obsolete
        PlayerSettings.statusBarHidden = true;
        PlayerSettings.stereoRenderingPath = StereoRenderingPath.MultiPass;
        //PlayerSettings.stereoscopic3D = false; // obsolete
        PlayerSettings.stripEngineCode = true;
        PlayerSettings.stripUnusedMeshComponents = true;

        //PlayerSettings.strippingLevel = StrippingLevel.StripAssemblies; // obsolete
        PlayerSettings.use32BitDisplayBuffer = true;
        PlayerSettings.useAnimatedAutorotation = true;
        PlayerSettings.useHDRDisplay = false;
        PlayerSettings.useMacAppStoreValidation = false;
        PlayerSettings.usePlayerLog = true;

        PlayerSettings.virtualRealitySplashScreen = null;
        PlayerSettings.virtualRealitySupported = false;
        PlayerSettings.visibleInBackground = false;

        PlayerSettings.vulkanEnableSetSRGBWrite = false;
        // PlayerSettings.vulkanUseSWCommandBuffers = false; // depricated
    }

    public static BuildPlayerOptions iOSSettings(BuildOptions buildOptions, string postfix)
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = gameScenFiles;
        buildPlayerOptions.locationPathName = desktopPath + BUILD_OUTPUT_PATH + BUILD_OUTPUT_IOS_SUBPATH + postfix;
        buildPlayerOptions.target = BuildTarget.iOS;
        buildPlayerOptions.options = buildOptions;
        buildPlayerOptions.targetGroup = BuildTargetGroup.iOS;

        return buildPlayerOptions;
    }

    public static BuildPlayerOptions AndroidSettings(BuildOptions buildOptions, string postfix)
    {
        PlayerSettings.bundleVersion = bundleVersion;
        PlayerSettings.Android.bundleVersionCode = Int32.Parse(bundleVersionCodeAndroid);

        LogUtil.Log("AndroidSettings _____ bundleVersion : " + PlayerSettings.bundleVersion);
        LogUtil.Log("AndroidSettings _____ bundleVersionCode : " + PlayerSettings.Android.bundleVersionCode);

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = gameScenFiles;
        buildPlayerOptions.locationPathName = desktopPath + BUILD_OUTPUT_PATH + BUILD_OUTPUT_ANDROID_SUBPATH + postfix + "/" + androidAPK + bundleVersionCodeAndroid + postfix + ".apk";
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = buildOptions;
        buildPlayerOptions.targetGroup = BuildTargetGroup.Android;

        EditorPrefs.SetString("AndroidSdkRoot", Environment.GetEnvironmentVariable("ANDROID_SDK_ROOT"));

#if !UNITY_CLOUD_BUILD
        PlayerSettings.Android.keystoreName = desktopPath + "/data/turbo-labz/projects/keystores/instant-chess/instant-chess-signing.keystore";
        PlayerSettings.Android.keystorePass = "0_turbolabzsignature-instant-chess_1";
        PlayerSettings.Android.keyaliasPass = "0_turbolabzsignature-instant-chess_1";
        PlayerSettings.Android.keyaliasName = "instant-chess-signing";
#endif
        PlayerSettings.Android.renderOutsideSafeArea = false;

        return buildPlayerOptions;
    }


    [MenuItem("Build/Build Chess iOS Release", false, 1)]
    public static void BuildiOS()
    {
        LogUtil.Log("Start Build iOS Release", "yellow");
        ProcessArgs();
        ProcessSkinLinks();
        BuildPlayerSettings();
        BuildPlayerSettingsiOS();
        GASettings(true);

#if UNITY_CLOUD_BUILD
        SetGameEnvironment();
#endif
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "CT_OC;SUBSCRIPTION_TEST");
        PlayerSettings.bundleVersion = bundleVersion;
        PlayerSettings.Android.bundleVersionCode = Int32.Parse(bundleVersionCodeiOS);
        BuildPlayerOptions buildPlayerOptions = iOSSettings(BuildOptions.CompressWithLz4HC, "_Release");

#if !UNITY_CLOUD_BUILD
        ProcessBuild(buildPlayerOptions);
#endif
        LogUtil.Log("End Build iOS Release", "yellow");
    }

    [MenuItem("Build/Build Chess iOS Development", false, 2)]
    public static void BuildiOSDevelopment()
    {
        LogUtil.Log("Start Build iOS Development", "yellow");
        ProcessArgs();
        ProcessSkinLinks();
        BuildPlayerSettings();
        BuildPlayerSettingsiOS();
        GASettings(true);

#if UNITY_CLOUD_BUILD
        SetGameEnvironment();
#endif
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "CT_OC;SUBSCRIPTION_TEST");
        PlayerSettings.bundleVersion = bundleVersion;
        PlayerSettings.Android.bundleVersionCode = Int32.Parse(bundleVersionCodeiOS);
        BuildPlayerOptions buildPlayerOptions = iOSSettings(BuildOptions.Development, "_Development");

#if !UNITY_CLOUD_BUILD
        ProcessBuild(buildPlayerOptions);
#endif

        LogUtil.Log("End Build iOS Development", "yellow");
    }

    [MenuItem("Build/Build Chess Andriod Release", false, 3)]
    public static void BuildAndroid()
    {
        LogUtil.Log("Start Build Android Release");
        ProcessArgs();
        ProcessSkinLinks();
        BuildPlayerSettings();
        BuildPlayerSettingsAndroid();
        GASettings(true);

#if UNITY_CLOUD_BUILD
        SetGameEnvironment();
#endif
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "CT_OC;SUBSCRIPTION_TEST");
        BuildPlayerOptions buildPlayerOptions = AndroidSettings(BuildOptions.None, "_Release");
#if !UNITY_CLOUD_BUILD
        ProcessBuild(buildPlayerOptions);
#endif
        LogUtil.Log("End Build Android Release");
    }

    [MenuItem("Build/Build Chess Andriod Development", false, 4)]
    public static void BuildAndroidDevelopment()
    {
        LogUtil.Log("Start Build Android Development");
        ProcessArgs();
        ProcessSkinLinks();
        BuildPlayerSettings();
        BuildPlayerSettingsAndroid();
        GASettings(true);

#if UNITY_CLOUD_BUILD
        SetGameEnvironment();
#endif

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "CT_OC;SUBSCRIPTION_TEST");
        BuildPlayerOptions buildPlayerOptions = AndroidSettings(BuildOptions.Development, "_Development");
#if !UNITY_CLOUD_BUILD
        ProcessBuild(buildPlayerOptions);
#endif
        LogUtil.Log("End Build Android Development");
    }

    [MenuItem("Build/Build Chess iOS for Store", false, 15)]
    public static void BuildiOSForStore()
    {
        LogUtil.Log("Start Build iOS for Store", "yellow");
        ProcessArgs();
        ProcessSkinLinks();
        BuildPlayerSettings();
        BuildPlayerSettingsiOS();
        GASettings(false);

#if UNITY_CLOUD_BUILD
        SetGameEnvironment();
#endif
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "CT_OC");
        PlayerSettings.bundleVersion = bundleVersion;
        PlayerSettings.Android.bundleVersionCode = Int32.Parse(bundleVersionCodeiOS);
        BuildPlayerOptions buildPlayerOptions = iOSSettings(BuildOptions.CompressWithLz4HC, "_ReleaseStore");
#if !UNITY_CLOUD_BUILD
        ProcessBuild(buildPlayerOptions);
#endif

        LogUtil.Log("End Build iOS for Store", "yellow");

#if SUBSCIPTION_TEST
        LogUtil.Log("SUBSCIPTION_TEST are ON please disable it for store builds");
#else
        LogUtil.Log("SUBSCIPTION_TEST are OFF for store builds");
#endif

    }

    [MenuItem("Build/Build Chess Andriod for Store", false, 15)]
    public static void BuildAndroidForStore()
    {
        LogUtil.Log("Start Build Android for Store");
        ProcessArgs();
        ProcessSkinLinks();
        BuildPlayerSettings();
        BuildPlayerSettingsAndroid();
        GASettings(false);

#if UNITY_CLOUD_BUILD
        SetGameEnvironment();
#endif
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "CT_OC");
        BuildPlayerOptions buildPlayerOptions = AndroidSettings(BuildOptions.None, "_ReleaseStore");
        EditorUserBuildSettings.buildAppBundle = true;

#if !UNITY_CLOUD_BUILD
        ProcessBuild(buildPlayerOptions);
#endif
        LogUtil.Log("End Build Android for Store");

#if SUBSCIPTION_TEST
        LogUtil.Log("SUBSCIPTION_TEST are ON please disable it for store builds");
#else
        LogUtil.Log("SUBSCIPTION_TEST are OFF for store builds");
#endif
    }

    [MenuItem("Build/Apply Build Settings (iOS & Android)", false, 0)]
    public static void ApplyBuildSettingsiOSAndroid()
    {
        LogUtil.Log("Applying project settings for iOS and Android");
        BuildPlayerSettings();
        BuildPlayerSettingsAndroid();
        BuildPlayerSettingsiOS();
        LogUtil.Log("End Apply Settings");
    }

    public static void BuildiOSCloud(string xcodeproj)
    {
        BuildiOS();
    }

    public static void BuildAndroidloud(string player)
    {
        BuildAndroid();
    }

    public static void BuildiOSCloudDevelopment(string xcodeproj)
    {
        BuildiOSDevelopment();
    }

    public static void BuildAndroidloudDevelopment(string player)
    {
        BuildAndroidDevelopment();
    }

    public static string GetBundleVersion()
    {
      string bundleVersion = PlayerSettings.bundleVersion;

#if UNITY_CLOUD_BUILD
     string versionString = Environment.GetEnvironmentVariable("BUNDLE_VERSION");
     if (versionString != null){
            bundleVersion = versionString;
        }
#endif

        LogUtil.Log("UNITY _____ GetBundleVersion : " + bundleVersion);

        return bundleVersion;
    }

    public static string GetAndroidBundleCode()
    {
        string bundleVersionCode = PlayerSettings.Android.bundleVersionCode.ToString();

#if UNITY_CLOUD_BUILD
     string bundleVersionCodeString = Environment.GetEnvironmentVariable("ANDROID_BUNDLE_CODE");
     if (bundleVersionCodeString != null){
            bundleVersionCode = bundleVersionCodeString;
        }
#endif

        LogUtil.Log("UNITY _____ GetAndroidBundleCode : " + bundleVersionCode);

        return bundleVersionCode;
    }

    public static string GetIOSBundleCode()
    {
        string buildNumber = PlayerSettings.iOS.buildNumber;

#if UNITY_CLOUD_BUILD
     string buildNumberString = Environment.GetEnvironmentVariable("IOS_BUNDLE_CODE");
     if (buildNumberString != null){
            buildNumber = buildNumberString;
        }
#endif

        LogUtil.Log("UNITY _____ GetIOSBundleCode : " + buildNumber);

        return buildNumber;
    }

}
