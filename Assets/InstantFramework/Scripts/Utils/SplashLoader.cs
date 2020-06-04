using UnityEngine;
using System.Collections;
using TurboLabz.InstantFramework;
using UnityEngine.SceneManagement;
using TurboLabz.TLUtils;
using HUFEXT.GenericGDPR.Runtime.API;
using HUFEXT.ModuleStarter.Runtime.API;

public class SplashLoader : MonoBehaviour {

    public static int launchCode = 0; // 1 = normal launch, 2 = resume, 3 = already launched

    void Awake()
    {
        Debug.unityLogger.logEnabled = Debug.isDebugBuild;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Input.multiTouchEnabled = Settings.MULTI_TOUCH_ENABLED;
        Application.targetFrameRate = Settings.TARGET_FRAME_RATE;
        launchCode = 1;
    }

    void OnEnable()
    {
        HGenericGDPR.OnPolicyAccepted += LoadGameScene;
    }

    void OnDisable()
    {
        HGenericGDPR.OnPolicyAccepted -= LoadGameScene;
    }

    void Start() 
    {
        if (!HGenericGDPR.IsPolicyAccepted)
        {
            HGenericGDPR.Initialize();
        }
        else
        {
            LoadGameScene();
        }
    }

    void LoadGameScene()
    {
        HInitializationPipeline.RunPipeline();
    }
}