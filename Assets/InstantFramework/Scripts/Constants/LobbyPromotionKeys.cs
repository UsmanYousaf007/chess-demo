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
        public const string COINS_BANNER = "CoinsBanner";
        public const string ELITE_BANNER = "EliteBanner";
        public const string GOLDEN_BANNER = "GoldenBanner";
        public const string EMERALD_BANNER = "EmeraldBanner";
        public const string RUBY_BANNNER = "RubyBanner";
        public const string DIAMOND_BANNER = "DiamondBanner";
        public const string GRAND_MASTER_BANNER = "GrandMasterBanner";

        public static bool Contains(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            if (key.Equals(ELITE_BANNER)
                || key.Equals(GOLDEN_BANNER)
                || key.Equals(EMERALD_BANNER)
                || key.Equals(RUBY_BANNNER)
                || key.Equals(DIAMOND_BANNER)
                || key.Equals(GRAND_MASTER_BANNER)
                || key.Equals(SUBSCRIPTION_BANNER)
                || key.Equals(COINS_BANNER)
                || key.Equals(GAME_UPDATE_BANNER)
                || key.Equals(THEMES_BANNER)
                || key.Equals(LESSONS_BANNER)
                || key.Equals(ADS_BANNER)
                || key.Equals(REWARDS_BANNER)
                )
            {
                return true;
            }

            return false;
        }
    }
}
