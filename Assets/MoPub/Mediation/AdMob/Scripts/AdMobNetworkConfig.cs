﻿#if mopub_manager
using UnityEngine;

public class AdMobNetworkConfig : MoPubNetworkConfig
{
    public override string AdapterConfigurationClassName
    {
        get { return Application.platform == RuntimePlatform.Android
                  ? "com.mopub.mobileads.GooglePlayServicesAdapterConfiguration"
                  : "GoogleAdMobAdapterConfiguration"; }
    }

    public override string MediationSettingsClassName
    {
        get { return Application.platform == RuntimePlatform.Android
                  ? "com.mopub.mobileads.GooglePlayServicesRewardedVideo$GooglePlayServicesMediationSettings"
                  : "MPGoogleGlobalMediationSettings"; }
    }

    [Header("Network Configuration")]

    [Tooltip("Enter your app ID that the Mobile Ads SDK can use to initialize.")]
    [Config.Optional]
    public PlatformSpecificString appid;

    [Tooltip("Check to request non-personalized ads. Clear to request personalized ads.")]
    [Config.Optional]
    public bool npa;

    [Header("Mediation Settings (Global)")]

    [Tooltip("Enter a web URL corresponding to the content shown in app to utilize AdMob's contextual targeting.")]
    [Mediation.Optional]
    public string contentUrl;

    [Tooltip("Enter a test device ID to request for test ads.")]
    [Mediation.Optional]
    public string testDeviceId;

    [Tooltip("Check if you want your content to be treated as child-directed for purposes of COPPA.")]
    [Mediation.Optional]
    public bool taggedForChildDirectedTreatment;

    [Tooltip("Check if you want your ad request to be handled suitable for users under the age of consent.")]
    [Mediation.Optional]
    public bool taggedForUnderAgeOfConsent;

    public override MoPubBase.MediatedNetwork NetworkOptions
    {
        get {
            var options = base.NetworkOptions;
            if (enabledOptions.Contains("npa"))
                // AdMob adapter wants "1" and "0" for true and false.
                options.NetworkConfiguration["npa"] = npa ? "1" : "0";
            return options;
        }
    }
}
#endif
