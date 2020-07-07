﻿using UnityEngine;
using TurboLabz.InstantFramework;
using HUFEXT.GenericGDPR.Runtime.API;
using HUFEXT.ModuleStarter.Runtime.API;
using GameAnalyticsSDK;
using TurboLabz.TLUtils;

public class SplashLoader : MonoBehaviour {

    const string FTUE_KEY = "ftue";

    public static int launchCode = 1; // 1 = normal launch, 2 = resume, 3 = already launched

    public static bool FTUE
    {
        get
        {
            if (!PlayerPrefs.HasKey(FTUE_KEY))
            {
                PlayerPrefs.SetInt(FTUE_KEY, 1);
            }

            return PlayerPrefs.GetInt(FTUE_KEY) == 1;
        }

        set
        {
            PlayerPrefs.SetInt(FTUE_KEY, value ? 1 : 0);
        }
    }

    void Awake()
    {
        Debug.unityLogger.logEnabled = Debug.isDebugBuild;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Input.multiTouchEnabled = Settings.MULTI_TOUCH_ENABLED;
        Application.targetFrameRate = Settings.TARGET_FRAME_RATE;
        GameAnalytics.Initialize();
        launchCode = 1;

        if (FTUE)
        {
            LogAnalytic(AnalyticsEventId.ftue_app_launch);
        }
    }

    void OnEnable()
    {
        HGenericGDPR.OnPolicyAccepted += RunInitPipiline;
    }

    void OnDisable()
    {
        HGenericGDPR.OnPolicyAccepted -= RunInitPipiline;
    }

    void Start() 
    {
        if (!HGenericGDPR.IsPolicyAccepted)
        {
            HGenericGDPR.Initialize();
            LogAnalytic(AnalyticsEventId.ftue_gdpr);
        }
        else
        {
            RunInitPipiline();
        }
    }

    void RunInitPipiline()
    {
        HInitializationPipeline.RunPipeline();
    }

    void LogAnalytic(AnalyticsEventId evt)
    {
        GameAnalytics.NewDesignEvent(evt.ToString());

#if UNITY_EDITOR
        var suffix = "[EDITOR_ANALYTICS]";
#else
        var suffix = "[TLANALYTICS]";
#endif

        LogUtil.Log($"{suffix} {evt}", "yellow");
    }
}