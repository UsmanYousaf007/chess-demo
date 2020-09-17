using HUF.NotificationsUnity.Runtime.API;
using HUF.Utils.Runtime.AndroidManifest;
using JetBrains.Annotations;
using UnityEngine;


namespace HUF.NotificationsUnity.Runtime.Implementation
{
    [CreateAssetMenu(menuName = "HUF/Notifications/UnityNotificationsConfig",
        fileName = "UnityNotificationsConfig")]
    public class UnityNotificationsConfig : AndroidManifestKeysConfig
    {
        public const string DEFAULT_CHANNEL_NAME = "Notifications";
        public const string DEFAULT_CHANNEL_DESC = "Main app notifications";

        [SerializeField] string channelName = DEFAULT_CHANNEL_NAME;
        [SerializeField] string channelDescription = DEFAULT_CHANNEL_DESC;
        [SerializeField] NotificationImportance channelImportance = NotificationImportance.Default;
        [SerializeField] bool rescheduleAfterBoot = true;

        public override string PackageName => "NotificationsUnity";
        public string ChannelName => channelName;
        public string ChannelDescription => channelDescription;

        public NotificationImportance ChannelImportance => channelImportance;
        public override string AndroidManifestTemplatePath =>
            "Assets/Plugins/Android/NotificationsUnity/AndroidManifestTemplate.xml";

        [AndroidManifest(Tag = "uses-permission", Attribute = "android:name",
             ValueToReplace = "android.permission.RECEIVE_BOOT_COMPLETED"),
         UsedImplicitly]
        string AnalyticsCollectionStatusString =>
            rescheduleAfterBoot ? "android.permission.RECEIVE_BOOT_COMPLETED" : null;

        public override void RegisterManualInitializers()
        {
            AddManualInitializer( "Notifications - Unity", HNotificationsUnity.Init );
        }
    }
}