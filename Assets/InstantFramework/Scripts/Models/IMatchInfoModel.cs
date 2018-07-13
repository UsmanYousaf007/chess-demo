/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

namespace TurboLabz.InstantFramework
{
    public interface IMatchInfoModel
    {
        void Reset();
        string challengeId { get; set; }
        long gameStartTimeMilliseconds { get; set; }
        PublicProfile opponentPublicProfile { get; set; }
        string botId { get; set; }
        float botDifficulty { get; set; }
        bool isBotMatch { get; }
        bool isResuming { get; set; }
        EndGameResult endGameResult { get; set; }
        int playerPrematchElo { get; set; }
    }
}
