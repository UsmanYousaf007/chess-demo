using UnityEngine;
using System.Collections;
using TurboLabz.InstantFramework;
using UnityEngine.SceneManagement;

public class SplashLoader : MonoBehaviour {

    SplashView inGameSplashView;
    bool notifyPostSceneLoad;

    void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Input.multiTouchEnabled = Settings.MULTI_TOUCH_ENABLED;
        Application.targetFrameRate = Settings.TARGET_FRAME_RATE;
    }
        
    IEnumerator Start() 
    {
        AsyncOperation async = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
        yield return async;
       
        inGameSplashView = GameObject.FindWithTag("SplashConnection").GetComponent<SplashView>();;

        // If the animation had finished running before the scene had loaded it will inform
        // us via this bool that we need to notify the game after the scene has loaded
        if (notifyPostSceneLoad)
        {
            Notify();
        }
    }

    public void OnSplashAnimationComplete()
    {
        if (inGameSplashView != null)
        {
            Notify();
        }
        else
        {
            notifyPostSceneLoad = true;
        }
    }

    private void Notify()
    {
        inGameSplashView.originalSplash = gameObject.transform.parent.gameObject;
        inGameSplashView.OnSplashAnimationComplete();
    }
}