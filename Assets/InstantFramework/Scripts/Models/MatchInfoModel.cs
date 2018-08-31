/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public class MatchInfo
    {
        public long gameStartTimeMilliseconds { get; set; }
        public PublicProfile opponentPublicProfile { get; set; }
        public string botId { get; set; }
        public float botDifficulty { get; set; }
        public int eloScoreDelta { get; set; }

        public bool isBotMatch
        {
            get
            {
                return (botId != null);
            }
        }
            
        public EndGameResult endGameResult { get; set; }
        public bool concluded { get; set; }

        public MatchInfo()
        {
            gameStartTimeMilliseconds = 0;
            opponentPublicProfile = new PublicProfile();
            botId = null;
            botDifficulty = 0;
            endGameResult = EndGameResult.NONE;
            eloScoreDelta = 0;
        }
    }

    public class MatchInfoModel : IMatchInfoModel
    {
        public Dictionary<string, MatchInfo> matches { get; set; }
        public string activeChallengeId { get; set; }
        public string activeLongMatchOpponentId { get; set; }
        public MatchInfo activeMatch 
        { 
            get
            {
                return matches[activeChallengeId];   
            }
        }

        [PostConstruct]
        public void PostConstruct()
        {
            matches = new Dictionary<string, MatchInfo>();
        }

        public MatchInfo CreateMatch(string challengeId)
        {
            MatchInfo matchInfo = new MatchInfo();
            matches.Add(challengeId, matchInfo);
            return matchInfo;
        }


    }
}
