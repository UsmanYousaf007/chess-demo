namespace TurboLabz.InstantGame
{
    public class LobbyPromotionKeys
    {
        public const string THEMES_BANNER = "ThemesBanner";
        public const string LESSONS_BANNER = "LessonsBanner";
        public const string ADS_BANNER = "AdsBanner";
        public const string GAME_UPDATE_BANNER = "GameUpdateBanner";
        public const string SUBSCRIPTION_BANNER = "SubscriptionBanner";
        public const string REWARDS_BANNER = "RewardsBanner";

        public static bool Contains(string key)
        {
            if (key.Equals(THEMES_BANNER)
                || key.Equals(LESSONS_BANNER)
                || key.Equals(ADS_BANNER)
                || key.Equals(GAME_UPDATE_BANNER)
                || key.Equals(SUBSCRIPTION_BANNER)
                || key.Equals(REWARDS_BANNER)
                )
            {
                return true;
            }

            return false;
        }
    }
}
