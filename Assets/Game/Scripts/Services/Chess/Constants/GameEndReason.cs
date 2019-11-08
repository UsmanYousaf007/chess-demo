/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-24 18:43:07 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.Chess
{
    public enum GameEndReason
    {
        NONE,
        CHECKMATE,
        STALEMATE,
        DRAW_BY_INSUFFICIENT_MATERIAL,
        DRAW_BY_FIFTY_MOVE_RULE_WITH_MOVE,
        DRAW_BY_FIFTY_MOVE_RULE_WITHOUT_MOVE,
        DRAW_BY_THREEFOLD_REPEAT_RULE_WITH_MOVE,
        DRAW_BY_THREEFOLD_REPEAT_RULE_WITHOUT_MOVE,
        TIMER_EXPIRED,
        RESIGNATION,
        PLAYER_DISCONNECTED,
        DECLINED,
        ABANDON
    }
}
