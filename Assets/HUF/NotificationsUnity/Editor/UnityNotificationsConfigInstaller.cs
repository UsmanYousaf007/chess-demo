using HUF.NotificationsUnity.Runtime.Implementation;
using HUF.Utils.Editor.Configs;

namespace HUF.NotificationsUnity.Editor
{
    public class UnityNotificationsConfigInstaller : BaseConfigInstaller
    {
        static void OnPostprocessAllAssets( string[] imported,
            string[] deleted,
            string[] moved,
            string[] movedFromPaths )
        {
            PostprocessImport<UnityNotificationsConfig>(
                "Unity Notifications",
                "UnityNotificationsConfigInstaller.cs",
                imported );
        }
    }
}