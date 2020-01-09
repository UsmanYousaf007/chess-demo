using UnityEngine;
using System.Collections;
using TurboLabz.InstantFramework;
using UnityEngine.SceneManagement;
using TurboLabz.TLUtils;

public class SplashLoader : MonoBehaviour {

    public static int launchCode = 1; // 1 = normal launch, 2 = resume, 3 = already launched

    void Awake()
    {
        Debug.unityLogger.logEnabled = Debug.isDebugBuild;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Input.multiTouchEnabled = Settings.MULTI_TOUCH_ENABLED;
        Application.targetFrameRate = Settings.TARGET_FRAME_RATE;
        launchCode = 1;
    }
        
    IEnumerator Start() 
    {
        AsyncOperation async = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
        yield return async;
    }
}