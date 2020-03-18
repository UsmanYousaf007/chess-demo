#if UNITY_IOS
using System.Runtime.InteropServices;

namespace HUFEXT.CrossPromo.AppLauncher
{
    public class IOSAppLauncherService : IAppLauncher
    {
        [DllImport ("__Internal")]
        static extern bool IsIOSAppInstalled(string appScheme);
        
        [DllImport ("__Internal")]
        static extern bool IOSLaunchApp(string appScheme);
        
        public bool IsAppInstalled(string packageName)
        {
            return IsIOSAppInstalled($"{packageName}://");
        }

        public bool LaunchApp(string packageName)
        {
            return IOSLaunchApp($"{packageName}://");
        }
    }
}
#endif