/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-01-11 17:36:46 UTC+05:00
/// 
/// @description
/// [add_description_here]
using TurboLabz.Chess;
using System.Collections.Generic;

namespace TurboLabz.CPU
{
    public class CPUSettings
    {
        public const int MIN_STRENGTH = 1;
        public const int MAX_STRENGTH = 10;
        public static readonly int[] DURATION_MINUTES = { 0,    1,     5,    60 };
        public static readonly ChessColor[] PLAYER_COLORS = { ChessColor.WHITE, ChessColor.BLACK, ChessColor.NONE };
        public const int DEFAULT_STRENGTH = 1;
        public const int DEFAULT_TIMER_INDEX = 0;
        public const int DEFAULT_PLAYER_COLOR_INDEX = 0;
        public const string DEFAULT_PLAYER_ID = "DEFAULT_PLAYER_ID";
    }
}
