using System;
using HUF.Ads.API;
using HUF.Utils.Extensions;
using HUF.Utils.Runtime.Logging;
using UnityEngine;

namespace HUF.Ads.Implementation
{
    [Serializable]
    public struct ApplicationIdentifier
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( HAds.logPrefix, nameof(ApplicationIdentifier));
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
                        HLog.LogWarning(logPrefix, $"Using unsupported platform for current provider configuration.");
                        return "";
                }
            }
        }
#if UNITY_EDITOR
        public string EditoriOSAppId => iOSAppId;
        public string EditorAndroidAppId => androidAppId;
#endif
    }
}