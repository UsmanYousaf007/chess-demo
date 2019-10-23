namespace TurboLabz.InstantGame
{
    public class LobbyPromotionKeys
    {
        public const string COACH_BANNER = "CoachBanner";
        public const string STRENGTH_BANNER = "StrengthBanner";

        public static bool Contains(string key)
        {
            if (key.Equals(COACH_BANNER) ||
                key.Equals(STRENGTH_BANNER))
            {
                return true;
            }

            return false;
        }
    }
}
