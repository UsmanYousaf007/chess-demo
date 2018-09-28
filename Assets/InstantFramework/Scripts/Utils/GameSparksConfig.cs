using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSparksConfig : MonoBehaviour {

    public GameObject Live;
    public GameObject Preview;
    public string configURL = "";

    // URL such as https://turbolabz.com/wp-content/uploads/2018/09/chessstar-3-3-2-1.odt;

	// Use this for initialization
	void Awake () 
    {
        StartCoroutine(CheckStageURL(configURL));
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
