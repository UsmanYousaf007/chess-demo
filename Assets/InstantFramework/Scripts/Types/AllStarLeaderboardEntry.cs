using UnityEngine;
using ArabicSupport;

namespace TurboLabz.InstantFramework
{
    public class AllStarLeaderboardEntry
    {
        public string playerId;
        public string countryId;
        public int score;
        public int rank;
        public int league;
        public string uploadedPicId;
        public string facebookUserId;
        public Sprite profilePicture = null;

        private string _name;
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = ArabicFixer.Fix(value, false, false);
            }
        }
    }
}
