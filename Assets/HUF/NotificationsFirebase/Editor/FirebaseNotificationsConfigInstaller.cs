using HUF.NotificationsFirebase.Runtime.Implementation;
using HUF.Utils.Editor.Configs;

namespace HUF.NotificationsFirebase.Editor
{
    public class FirebaseNotificationsConfigInstaller : BaseConfigInstaller
    {
        static void OnPostprocessAllAssets( string[] imported,
            string[] deleted,
            string[] moved,
            string[] movedFromPaths )
        {
            PostprocessImport<FirebaseNotificationsConfig>(
                "Firebase Notifications",
                "FirebaseNotificationsConfigInstaller.cs",
                imported );
        }
    }
}