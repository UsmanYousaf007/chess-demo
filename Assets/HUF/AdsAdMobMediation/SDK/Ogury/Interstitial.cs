using System;
using UnityEngine;

public class Interstitial : MonoBehaviour
{

    public void OnAdAvailable()
    {
        Debug.Log("OnAdAvailable");
    }

    public void OnAdClosed()
    {
        Debug.Log("OnAdClosed");
    }

    public void OnAdDisplayed()
    {
        Debug.Log("OnAdDisplayed");
    }

    public void OnAdLoaded()
    {
        Debug.Log("OnAdLoaded");
    }

    public void OnAdNotAvailable()
    {
        Debug.Log("OnAdNotAvailable");
    }

    public void OnAdNotLoaded()
    {
        Debug.Log("OnAdNotLoaded");
    }

    public void OnAdError(string error)
    {
        Debug.Log("OnAdError " + error);
    }

}
