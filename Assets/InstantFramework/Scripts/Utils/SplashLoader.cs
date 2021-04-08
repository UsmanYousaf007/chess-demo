using UnityEngine;
using UnityEngine.UI;
using TurboLabz.InstantFramework;
using HUF.PolicyGuard.Runtime.API;
using HUFEXT.ModuleStarter.Runtime.API;
using GameAnalyticsSDK;
using TurboLabz.TLUtils;
using HUF.Analytics.Runtime.API;
using HUF.Utils.Runtime.Configs.API;
using HUF.PolicyGuard.Runtime.Configs;
using HUF.Ads.Runtime.API;
using HUF.Notifications.Runtime.API;
using HUF.Notifications.Runtime.Data.Structs;

#if UNITY_IOS
using HUF.Utils.Runtime.PlayerPrefs;
using HUF.PolicyGuard.Runtime.Implementations;
#endif

public class SplashLoader : MonoBehaviour {

    public static int launchCode = 1; // 1 = normal launch, 2 = resume, 3 = already launched
    public Text versionLabel;
    public GameObject ATT_BG;

    private bool termsShown;

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

    void Start()
    {
        if (HAnalytics.GetGDPRConsent() == null)
        {
            HNotifications.Local.AskForPermission(true);
            SetupPolicyGuardConfig(firstSession: true);
            HPolicyGuard.Initialize();
            LogAnalytic(AnalyticsEventId.terms_and_conditions_shown_2);
            termsShown = true;
        }
        else if (CheckPersonalizedAdsStatus() == false)
        {
            SetupPolicyGuardConfig(firstSession: false);
            HPolicyGuard.Initialize();
        }
        else
        {
            RunInitPipiline();
        }
    }

    void RunInitPipiline()
    {
        if (termsShown)
        {
            LogAnalytic(AnalyticsEventId.terms_and_conditions_accepted_2);
        }

        HInitializationPipeline.RunPipeline();
    }

    private static void OnRemoteConfigsUpdated()
    {
        Settings.ABTest.COINS_TEST_GROUP = GameAnalytics.GetRemoteConfigsValueAsString("coins_test", Settings.ABTest.COINS_TEST_GROUP_DEFAULT);
        GameAnalytics.SetCustomDimension01(Settings.ABTest.COINS_TEST_GROUP);
    }

    private void SetupPolicyGuardConfig(bool firstSession)
    {
        var config = HConfigs.GetConfig<PolicyGuardConfig>();
        config.ShowATTPreOptInPopup = !firstSession;
        config.ShowNativeATT = !firstSession;
        config.ShowAdsConsent = !firstSession;
    }

    void OnEnable()
    {
        HPolicyGuard.OnEndCheckingPolicy += RunInitPipiline;
        HPolicyGuard.OnGDPRPopupShowed += OnTermsAndConditionShown;
        HPolicyGuard.OnGDPRPopupClosed += OnTermsAndConditionClosed;
        HPolicyGuard.OnPersonalizedAdsPopupShowed += OnGDPRShown;
        HPolicyGuard.OnPersonalizedAdsPopupClosed += OnGDPRClosed;
        HPolicyGuard.OnATTPopupShowed += OnPreATTShown;
        HPolicyGuard.OnATTPopupClosed += OnPreATTClosed;
        HPolicyGuard.OnATTNativePopupShowed += OnATTShown;
        HPolicyGuard.OnATTNativePopupClosed += OnATTClosed;
        HNotifications.Local.OnAskForPermissionComplete += OnAskForNotificationPermissionComplete;
    }

    void OnDisable()
    {
        HPolicyGuard.OnEndCheckingPolicy -= RunInitPipiline;
        HPolicyGuard.OnGDPRPopupShowed -= OnTermsAndConditionShown;
        HPolicyGuard.OnGDPRPopupClosed -= OnTermsAndConditionClosed;
        HPolicyGuard.OnPersonalizedAdsPopupShowed -= OnGDPRShown;
        HPolicyGuard.OnPersonalizedAdsPopupClosed -= OnGDPRClosed;
        HPolicyGuard.OnATTPopupShowed -= OnPreATTShown;
        HPolicyGuard.OnATTPopupClosed -= OnPreATTClosed;
        HPolicyGuard.OnATTNativePopupShowed -= OnATTShown;
        HPolicyGuard.OnATTNativePopupClosed -= OnATTClosed;
        HNotifications.Local.OnAskForPermissionComplete -= OnAskForNotificationPermissionComplete;
    }

    private void OnTermsAndConditionShown()
    {
        LogAnalytic(AnalyticsEventId.terms_and_conditions_shown);
        HAnalytics.LogEvent(new AnalyticsEvent(AnalyticsEventId.terms_and_conditions_shown.ToString()), AnalyticsServiceName.HBI);
    }

    private void OnTermsAndConditionClosed(bool status)
    {
        LogAnalytic(AnalyticsEventId.terms_and_conditions_accepted);
        HAnalytics.LogEvent(new AnalyticsEvent(AnalyticsEventId.terms_and_conditions_accepted.ToString()), AnalyticsServiceName.HBI);
    }

    private void OnGDPRShown()
    {
        LogAnalytic(AnalyticsEventId.gdpr);
    }

    private void OnGDPRClosed(bool status)
    {
        LogAnalytic(AnalyticsEventId.gdpr_player_interaction, status ? AnalyticsContext.accepted : AnalyticsContext.rejected);
    }

    private void OnPreATTShown()
    {
        LogAnalytic(AnalyticsEventId.pre_permission);
    }

    private void OnPreATTClosed(bool status)
    {
        LogAnalytic(AnalyticsEventId.pre_permission_interaction, status ? AnalyticsContext.accepted : AnalyticsContext.rejected);
    }

    private void OnATTShown()
    {
        ATT_BG.SetActive(true);
        LogAnalytic(AnalyticsEventId.ATT_shown);
    }

    private void OnATTClosed(bool status)
    {
        ATT_BG.SetActive(false);
        LogAnalytic(AnalyticsEventId.ATT_interaction, status ? AnalyticsContext.accepted : AnalyticsContext.rejected);
    }

    private void OnAskForNotificationPermissionComplete(ConsentStatus status)
    {
        LogAnalytic(AnalyticsEventId.notification_permission_dialogue, status == ConsentStatus.Granted ? AnalyticsContext.accepted : AnalyticsContext.rejected);
    }

    void LogAnalytic(AnalyticsEventId evt)
    {
        var evtStr = evt.ToString();
        GameAnalytics.NewDesignEvent(evtStr);
        PrintAnalytic(evtStr);
    }

    void LogAnalytic(AnalyticsEventId evt, AnalyticsContext context)
    {
        var evtStr = $"{evt}:{context}";
        GameAnalytics.NewDesignEvent(evtStr);
        PrintAnalytic(evtStr);
    }

    void PrintAnalytic(string evt)
    {
#if UNITY_EDITOR
        var prefix = "[EDITOR_ANALYTICS]";
#else
        var prefix = "[TLANALYTICS]";
#endif
        LogUtil.Log($"{prefix} {evt}", "yellow");
    }

    public static bool CheckPersonalizedAdsStatus()
    {
#if UNITY_ANDROID
        return HAds.HasConsent() != null;
#elif UNITY_IOS
        return HAds.HasConsent() != null && !HPlayerPrefs.GetBool(PolicyGuardService.ATT_POSTPONED_KEY, false) && !HPolicyGuard.WasATTPopupDisplayed();
#endif
    }
}