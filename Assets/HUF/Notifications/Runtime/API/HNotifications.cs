using JetBrains.Annotations;

namespace HUF.Notifications.Runtime.API
{
    public static class HNotifications
    {
        static HLocalNotifications local;

        /// <summary>
        /// Use to access Local notifications space.
        /// </summary>
        [PublicAPI]
        public static HLocalNotifications Local => local ?? (local = new HLocalNotifications());

        static HPushNotifications push;

        /// <summary>
        /// Use to access Push notifications space.
        /// </summary>
        [PublicAPI]
        public static HPushNotifications Push => push ?? (push = new HPushNotifications());
    }
}