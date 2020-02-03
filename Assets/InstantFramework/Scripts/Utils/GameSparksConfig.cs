using System.Collections;
using System.Collections.Generic;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;
using UnityEngine;

public class GameSparksConfig : MonoBehaviour {

    public GameObject Live;
    public GameObject Preview;
    public GameObject Dev;
    public GameObject Sandbox;
    public GameObject HuuugeDev;

    public Environment environment;
    public enum Environment
    {
        Development,
        LivePreview,
        Live,
        URLBased,
        Sandbox,
        HuuugeDev
    }

    public string configURL = "";

    /// <summary>
    /// Returns saved s3 url ping version.
    /// </summary>
    public static string configURLSavedVersion
    {
        get
        {
            return PlayerPrefs.GetString(PrefKeys.S3_URL_PING_VERSION, "0");
        }

        set
        {
            PlayerPrefs.SetString(PrefKeys.S3_URL_PING_VERSION, value);
        }
    }

    // URL such as https://turbolabz.com/wp-content/uploads/2018/09/chessstar-3-3-2-1.odt;

    // Use this for initialization
    void Awake () 
    {
        Live.SetActive(false);
        Preview.SetActive(false);
        Dev.SetActive(false);
        Sandbox.SetActive(false);
        HuuugeDev.SetActive(false);

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
        else if (environment == Environment.HuuugeDev)
        {
            HuuugeDev.SetActive(true);
        }
    }

    IEnumerator CheckStageURL(string url)
    {
        bool isLive = true;

        string configURLVersion = "";
        string[] words = url.Split('/');

        if(words.Length > 0)
        {
            configURLVersion = words[words.Length - 1];
        }

        if (configURLVersion != configURLSavedVersion)
        {
            WWW www = new WWW(url);
            yield return www;

            if (string.IsNullOrEmpty(www.error)) // Success
            {
                isLive = false;
            }
            else
            {
                configURLSavedVersion = configURLVersion;
            }
        }

        Live.SetActive(isLive);
        Preview.SetActive(!isLive);
    }
}
