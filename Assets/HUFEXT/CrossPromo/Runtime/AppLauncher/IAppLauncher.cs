namespace HUFEXT.CrossPromo.AppLauncher
{
    public interface IAppLauncher
    {
        bool IsAppInstalled(string packageName);
        bool LaunchApp(string packageName);
    }
}