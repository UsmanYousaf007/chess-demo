using JetBrains.Annotations;

namespace HUF.Notifications.Runtime.API
{
    public static class HNotifications
    {
        static HLocalNotifications local;

        /// <summary>
        /// Provides access to Local notifications.
        /// </summary>
        [PublicAPI]
        public static HLocalNotifications Local => local ?? ( local = new HLocalNotifications() );

        static HPushNotifications push;

        /// <summary>
        /// Provides access to Push notifications.
        /// </summary>
        [PublicAPI]
        public static HPushNotifications Push => push ?? ( push = new HPushNotifications() );
    }
}