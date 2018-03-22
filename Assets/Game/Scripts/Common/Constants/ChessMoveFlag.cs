/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-12-20 10:33:25 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.Chess
{
    public enum ChessMoveFlag
    {
        NONE,
        STANDARD,
        CASTLE_QUEEN_SIDE,
        CASTLE_KING_SIDE,
        PAWN_PROMOTION_QUEEN,
        PAWN_PROMOTION_ROOK,
        PAWN_PROMOTION_BISHOP,
        PAWN_PROMOTION_KNIGHT,
        EN_PASSANT
    }
}
