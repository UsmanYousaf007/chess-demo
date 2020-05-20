#if UNITY_ANDROID
using UnityEngine;

namespace HUFEXT.CrossPromo.Runtime.AppLauncher
{
    public class AndroidAppLauncherService : IAppLauncher
    {
        const string NATIVE_APP_LAUNCHER = "hufext.NativeGameLauncher.AppLauncher";
        const string UNITY_PLAYER_CLASS = "com.unity3d.player.UnityPlayer";
        const string CURRENT_ACTIVITY_OBJECT = "currentActivity";
        const string IS_APP_INSTALLED_METHOD_NAME = "isAppInstalled";
        const string LAUNCH_APP_METHOD_NAME = "launchApp";
        static AndroidJavaObject unityActivity;
        static AndroidJavaObject appLauncher;
        
        public AndroidAppLauncherService()
        {
            var unityPlayerClass = new AndroidJavaClass(UNITY_PLAYER_CLASS);
            unityActivity = unityPlayerClass.GetStatic<AndroidJavaObject>(CURRENT_ACTIVITY_OBJECT);
            appLauncher = new AndroidJavaClass(NATIVE_APP_LAUNCHER);
        }
        
        public bool IsAppInstalled(string packageName)
        {
            var result = appLauncher.CallStatic<bool>(IS_APP_INSTALLED_METHOD_NAME, unityActivity, packageName);
            return result;
        }

        public bool LaunchApp(string packageName)
        {
            var result = appLauncher.CallStatic<bool>(LAUNCH_APP_METHOD_NAME, unityActivity, packageName);
            return result;
        }
    }
}
#endif