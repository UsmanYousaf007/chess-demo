namespace HUFEXT.CrossPromo.Runtime.AppLauncher
{
    public interface IAppLauncher
    {
        bool IsAppInstalled(string packageName);
        bool LaunchApp(string packageName);
    }
}