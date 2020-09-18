using System;
using HUF.Ads.Runtime.API;
using HUF.Utils.Runtime.Logging;
using UnityEngine;

namespace HUF.Ads.Runtime.Implementation
{
    [Serializable]
    public struct ApplicationIdentifier
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( HAds.logPrefix, nameof(ApplicationIdentifier) );
#pragma warning disable 0649
        [SerializeField] string androidAppId;
        [SerializeField] string iOSAppId;
#pragma warning restore 0649

        public string Value
        {
            get
            {
#if UNITY_ANDROID
                return androidAppId;
#elif UNITY_IOS
                return iOSAppId;
#else
                HLog.LogWarning(logPrefix, $"Using unsupported platform for current provider configuration.");
                return "";
#endif
            }
        }
#if UNITY_EDITOR
        public string EditorAndroidAppId => androidAppId;
        public string EditoriOSAppId => iOSAppId;
#endif
    }
}