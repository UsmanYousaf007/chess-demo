using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSparksConfig : MonoBehaviour {

    public GameObject Live;
    public GameObject Preview;
    public GameObject Dev;

    public string configURL = "";
    public bool checkStagingURL = false;

    // URL such as https://turbolabz.com/wp-content/uploads/2018/09/chessstar-3-3-2-1.odt;

	// Use this for initialization
	void Awake () 
    {
        Live.SetActive(false);
        Preview.SetActive(false);
        Dev.SetActive(false);

        #if UNITY_EDITOR
        Dev.SetActive(true);
        return;
        #else

        // This code activates when a debug build is made for device
        if (UnityEngine.Debug.isDebugBuild)
        {
            Dev.SetActive(true);
            return;
        }

        if (checkStagingURL)
        {
            StartCoroutine(CheckStageURL(configURL));
        }
        else
        {
            Live.SetActive(true);
        }
        #endif
	}

    IEnumerator CheckStageURL(string url)
    {
        bool isLive = true;

        WWW www = new WWW(url);
        yield return www;

        if (string.IsNullOrEmpty(www.error)) // Success
        {
            if (www.text == "preview")
            {
                isLive = false;
            }
        }

        Live.SetActive(isLive);
        Preview.SetActive(!isLive);
    }
}
