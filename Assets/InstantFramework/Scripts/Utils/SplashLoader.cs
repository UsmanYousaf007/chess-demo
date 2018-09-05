using UnityEngine;
using System.Collections;
using TurboLabz.InstantFramework;
using UnityEngine.SceneManagement;
using TurboLabz.TLUtils;

public class SplashLoader : MonoBehaviour {

    void Awake()
    {
        Debug.unityLogger.logEnabled = Debug.isDebugBuild;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Input.multiTouchEnabled = Settings.MULTI_TOUCH_ENABLED;
        Application.targetFrameRate = Settings.TARGET_FRAME_RATE;
    }
        
    IEnumerator Start() 
    {
        AsyncOperation async = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
        yield return async;
    }
}