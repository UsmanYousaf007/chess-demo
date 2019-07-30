/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-12-14 13:36:26 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System;
using System.Collections.Generic;
using TurboLabz.Chess;

namespace TurboLabz.Multiplayer
{
    public interface IChessboardModel
    {
        Dictionary<string, Chessboard> chessboards { get; set; }
        bool isValidChallenge(string activeChallengeId);
    }
}
