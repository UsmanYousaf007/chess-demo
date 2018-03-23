/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-12-24 11:33:10 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.Chess
{
    public class ChessMoveResult
    {
        public ChessMoveFlag moveFlag;
        public bool isPlayerInCheck;
        public bool isOpponentInCheck;
        public bool isFiftyMoveRuleActive;
        public bool isThreefoldRepeatRuleActive;
        public GameEndReason gameEndReason;
        public string description;
        public ChessSquare capturedSquare;
    }
}
