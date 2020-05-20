namespace HUFEXT.CrossPromo.Runtime.AppLauncher
{
    public class DummyAppLauncherService : IAppLauncher
    {
        public bool IsAppInstalled(string packageName)
        {
            return false;
        }

        public bool LaunchApp(string packageName)
        {
            return false;
        }
    }
}