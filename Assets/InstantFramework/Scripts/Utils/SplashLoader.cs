using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TurboLabz.InstantFramework;
using UnityEngine.SceneManagement;
using TurboLabz.TLUtils;
using HUFEXT.GenericGDPR.Runtime.API;
using HUFEXT.ModuleStarter.Runtime.API;

public class SplashLoader : MonoBehaviour {

    public static int launchCode = 1; // 1 = normal launch, 2 = resume, 3 = already launched
    public Text versionLabel;

    void Awake()
    {
        Debug.unityLogger.logEnabled = Debug.isDebugBuild;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Input.multiTouchEnabled = Settings.MULTI_TOUCH_ENABLED;
        Application.targetFrameRate = Settings.TARGET_FRAME_RATE;
        launchCode = 1;
        versionLabel.text = Application.version;
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
}