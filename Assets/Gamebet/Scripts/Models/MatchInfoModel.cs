/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-04 13:35:53 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.Gamebet
{
    public class MatchInfoModel : IMatchInfoModel
    {
        public string challengeId { get; set; }
        public string roomId { get; set; }
        public long gameStartTimeMilliseconds { get; set; }
        public PublicProfile opponentPublicProfile { get; set; }
        public string botId { get; set; }

        public bool isBotMatch
        {
            get
            {
                return (botId != null);
            }
        }

        public bool isResuming { get; set; }
        public EndGameResult endGameResult { get; set; }
    }
}
