using HUF.Utils.Runtime.Logging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HUF.Utils.Runtime.NativeFunctions
{
    public class HNativeFunctions : MonoBehaviour
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(NativeFunctions) );

        public static void OpenAppSettings()
        {
#if UNITY_IOS
            string settingsURL = iOSNativeBindings.GetSettingsURL();
            Application.OpenURL( settingsURL );
#else
            HLog.LogWarning( logPrefix, "Not supported on this platform!" );
#endif
        }
    }
}