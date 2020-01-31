using System;
using HUF.Utils.Extensions;
using UnityEngine;

namespace HUF.Ads.Implementation
{
    [Serializable]
    public struct ApplicationIdentifier
    {
        const string LOG_PREFIX = "ApplicationIdentifier";
        
        #pragma warning disable 0649
        [SerializeField] string iOSAppId;
        [SerializeField] string androidAppId;
        #pragma warning restore 0649

        public string Value
        {
            get
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.IPhonePlayer:
                        return iOSAppId;
                    case RuntimePlatform.Android:
                        return androidAppId;
                    case RuntimePlatform.OSXEditor:
                    case RuntimePlatform.WindowsEditor:
                    case RuntimePlatform.LinuxEditor:
                        return androidAppId.IsNullOrEmpty() ? iOSAppId : androidAppId;
                    default:
                        Debug.LogWarning(
                            $"[{LOG_PREFIX}] Using unsupported platform for current provider configuration.");
                        return "";
                }
            }
        }
    }
}