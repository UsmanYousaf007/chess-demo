using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptinVideo : MonoBehaviour {

    public void OnAdAvailable()
    {
        Debug.Log("Video OnAdAvailable");
    }

    public void OnAdClosed()
    {
        Debug.Log("Video OnAdClosed");
    }

    public void OnAdDisplayed()
    {
        Debug.Log("Video OnAdDisplayed");
    }

    public void OnAdLoaded()
    {
        Debug.Log("Video OnAdLoaded");
    }

    public void OnAdNotAvailable()
    {
        Debug.Log("Video OnAdNotAvailable");
    }

    public void OnAdNotLoaded()
    {
        Debug.Log("Video OnAdNotLoaded");
    }

    public void OnAdError(string error)
    {
        Debug.Log("OptinAd OnAdError " + error);
    }

    public void OnOptinVideoRewarded(String reward)
    {
        Debug.Log("OptinAd OnOptinVideoRewarded reward" + reward);
    }
}
