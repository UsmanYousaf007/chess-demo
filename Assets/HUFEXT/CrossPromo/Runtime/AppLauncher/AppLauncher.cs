#if UNITY_ANDROID && !UNITY_EDITOR
using PlatformAppLauncher = HUFEXT.CrossPromo.Runtime.AppLauncher.AndroidAppLauncherService;
#elif UNITY_IPHONE && !UNITY_EDITOR
using PlatformAppLauncher = HUFEXT.CrossPromo.Runtime.AppLauncher.IOSAppLauncherService;
#else
using PlatformAppLauncher = HUFEXT.CrossPromo.Runtime.AppLauncher.DummyAppLauncherService;
#endif

namespace HUFEXT.CrossPromo.Runtime.AppLauncher
{
    public static class AppLauncher
    {
        static readonly IAppLauncher appLauncher;

        static AppLauncher()
        {
            appLauncher = new PlatformAppLauncher();
        }
        
        public static bool IsAppInstalled(string packageName)
        {
            return appLauncher.IsAppInstalled(packageName);
        }

        public static bool LaunchApp(string packageName)
        {
            return appLauncher.LaunchApp(packageName);
        }
    }
}