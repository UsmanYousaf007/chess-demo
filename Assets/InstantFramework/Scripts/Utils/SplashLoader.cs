using UnityEngine;
using UnityEngine.UI;
using TurboLabz.InstantFramework;
using HUFEXT.GenericGDPR.Runtime.API;
using HUFEXT.ModuleStarter.Runtime.API;
using GameAnalyticsSDK;
using TurboLabz.TLUtils;

public class SplashLoader : MonoBehaviour {

    public static int launchCode = 1; // 1 = normal launch, 2 = resume, 3 = already launched
    public Text versionLabel;

    void Awake()
    {
        Debug.unityLogger.logEnabled = Debug.isDebugBuild;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Input.multiTouchEnabled = Settings.MULTI_TOUCH_ENABLED;
        Application.targetFrameRate = Settings.TARGET_FRAME_RATE;
        GameAnalytics.Initialize();
        GameAnalytics.OnRemoteConfigsUpdatedEvent += OnRemoteConfigsUpdated;
        launchCode = 1;
        versionLabel.text = Application.version;
    }

    void OnEnable()
    {
        HGenericGDPR.OnPolicyAccepted += OnPolicyAccepted;
    }

    void OnDisable()
    {
        HGenericGDPR.OnPolicyAccepted -= OnPolicyAccepted;
    }

    void Start() 
    {
        if (!HGenericGDPR.IsPolicyAccepted)
        {
            HGenericGDPR.Initialize();
            LogAnalytic(AnalyticsEventId.terms_and_conditions_shown);
        }
        else
        {
            RunInitPipiline();
        }
    }

    void OnPolicyAccepted()
    {
        LogAnalytic(AnalyticsEventId.terms_and_conditions_accepted);
        RunInitPipiline();
    }

    void RunInitPipiline()
    {
        HInitializationPipeline.RunPipeline();
    }

    void LogAnalytic(AnalyticsEventId evt)
    {
        GameAnalytics.NewDesignEvent(evt.ToString());

#if UNITY_EDITOR
        var prefix = "[EDITOR_ANALYTICS]";
#else
        var prefix = "[TLANALYTICS]";
#endif

        LogUtil.Log($"{prefix} {evt}", "yellow");
    }

    private static void OnRemoteConfigsUpdated()
    {
        Settings.ABTest.ADS_TEST_GROUP = GameAnalytics.GetRemoteConfigsValueAsString("ads_test", Settings.ABTest.ADS_TEST_GROUP_DEFAULT);
        GameAnalytics.SetCustomDimension01(Settings.ABTest.ADS_TEST_GROUP);

        Settings.ABTest.PROMOTION_TEST_GROUP = GameAnalytics.GetRemoteConfigsValueAsString("promotions", Settings.ABTest.PROMOTION_TEST_GROUP_DEFAULT);
        GameAnalytics.SetCustomDimension02(Settings.ABTest.PROMOTION_TEST_GROUP);

        LogUtil.Log($"GA test group {Settings.ABTest.PROMOTION_TEST_GROUP}", "red");
    }
}