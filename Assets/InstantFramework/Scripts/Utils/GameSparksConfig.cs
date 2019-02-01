using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSparksConfig : MonoBehaviour {

    public GameObject Live;
    public GameObject Preview;
    public GameObject Dev;
    public GameObject Sandbox;

    public Environment environment;
    public enum Environment
    {
        Development,
        LivePreview,
        Live,
        URLBased,
        Sandbox
    }

    public string configURL = "";

    // URL such as https://turbolabz.com/wp-content/uploads/2018/09/chessstar-3-3-2-1.odt;

	// Use this for initialization
	void Awake () 
    {
        Live.SetActive(false);
        Preview.SetActive(false);
        Dev.SetActive(false);
        Sandbox.SetActive(false);

        if (environment == Environment.URLBased)
        {
            StartCoroutine(CheckStageURL(configURL));
        }
        else if (environment == Environment.Live)
        {
            Live.SetActive(true);
        }
        else if (environment == Environment.LivePreview)
        {
            Preview.SetActive(true);
        }
        else if (environment == Environment.Development)
        {
            Dev.SetActive(true);
        }
        else if (environment == Environment.Sandbox)
        {
            Sandbox.SetActive(true);
        }
    }

    IEnumerator CheckStageURL(string url)
    {
        bool isLive = true;

        WWW www = new WWW(url);
        yield return www;

        if (string.IsNullOrEmpty(www.error)) // Success
        {
            isLive = false;
        }

        Live.SetActive(isLive);
        Preview.SetActive(!isLive);
    }
}
