/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-02-17 16:33:03 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections.Generic;
using System.Text;
using TurboLabz.TLUtils;

namespace TurboLabz.Chess
{
    public struct HintVO
    {
        public ChessSquare fromSquare;
        public ChessSquare toSquare;
        public bool isHindsight;
        public int strength;
        public string piece;
        public string skinId;
        public bool didPlayerMadeBestMove;
    }
}
