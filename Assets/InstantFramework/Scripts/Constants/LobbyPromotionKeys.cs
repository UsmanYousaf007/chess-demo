namespace TurboLabz.InstantGame
{
    public class LobbyPromotionKeys
    {
        public const string COACH_BANNER = "CoachBanner";
        public const string STRENGTH_BANNER = "StrengthBanner";
        public const string ULTIMATE_BANNER = "UltimateBanner";
        public const string ADS_BANNER = "AdsBanner";
        public const string COACH_PURCHASE = "CoachPurchase";
        public const string STRENGTH_PURCHASE = "StrengthPurchase";
        public const string GAME_UPDATE_BANNER = "GameUpdateBanner";

        public static bool Contains(string key)
        {
            if (key.Equals(COACH_BANNER)
                || key.Equals(STRENGTH_BANNER)
                || key.Equals(ULTIMATE_BANNER)
                || key.Equals(ADS_BANNER)
                || key.Equals(COACH_PURCHASE)
                || key.Equals(STRENGTH_PURCHASE)
                || key.Equals(GAME_UPDATE_BANNER)
                )
            {
                return true;
            }

            return false;
        }
    }
}
