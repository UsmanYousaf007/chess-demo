/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-02-16 13:12:17 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.Multiplayer
{
    public enum ChessboardEvent
    {
        GAME_STARTED,
        GAME_ACCEPT_REQUESTED,
        GAME_ACCEPTED,
        GAME_DECLINED,
        SQUARE_CLICKED,
        PLAYER_MOVE_COMPLETE,
        OPPONENT_MOVE_COMPLETE,
        OPPONENT_MOVE_RENDER_COMPLETED,
        DRAW_CLAIMED,
        DRAW_REJECTED,
        GAME_ENDED,
        PROMO_SELECTED,
        MOVE_CONFIRMED,
        MOVE_UNDO,
        OFFBOARD_CLICKED
    }
}
