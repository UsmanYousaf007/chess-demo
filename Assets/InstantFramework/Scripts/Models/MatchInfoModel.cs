/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

namespace TurboLabz.InstantFramework
{
    public class MatchInfoModel : IMatchInfoModel
    {
        public string challengeId { get; set; }
        public long gameStartTimeMilliseconds { get; set; }
        public PublicProfile opponentPublicProfile { get; set; }
        public string botId { get; set; }
        public int playerPrematchElo { get; set; }

        public bool isBotMatch
        {
            get
            {
                return (botId != null);
            }
        }

        public bool isResuming { get; set; }
        public EndGameResult endGameResult { get; set; }

        public void Reset()
        {
            challengeId = null;
            gameStartTimeMilliseconds = 0;
            opponentPublicProfile = new PublicProfile();
            botId = null;
            isResuming = false;
            endGameResult = EndGameResult.NONE;
            playerPrematchElo = 0;
        }
    }
}
