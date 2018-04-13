/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-01-11 17:59:19 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.Chess
{
    public struct CPUMenuVO
    {
        public int minStrength;
        public int maxStrength;
        public int selectedStrength;
        public int[] durationMinutes;
        public int selectedDurationIndex;
        public ChessColor[] playerColors;
        public int selectedPlayerColorIndex;
        public bool inProgress;
        public int totalGames;
    }
}
