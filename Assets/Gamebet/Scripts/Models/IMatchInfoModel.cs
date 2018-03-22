/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-04 13:31:38 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.Gamebet
{
    public interface IMatchInfoModel
    {
        string challengeId { get; set; }
        string roomId { get; set; }
        long gameStartTimeMilliseconds { get; set; }
        PublicProfile opponentPublicProfile { get; set; }
        string botId { get; set; }
        bool isBotMatch { get; }
        bool isResuming { get; set; }
        EndGameResult endGameResult { get; set; }
    }
}
