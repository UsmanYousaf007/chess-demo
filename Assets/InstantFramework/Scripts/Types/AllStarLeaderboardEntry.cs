using UnityEngine;
using ArabicSupport;

namespace TurboLabz.InstantFramework
{
    public class AllStarLeaderboardEntry
    {
        public string playerId;
        public long score;
        public int rank;
        public PublicProfile publicProfile;

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
