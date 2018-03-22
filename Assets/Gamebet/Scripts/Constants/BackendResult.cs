/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-12 21:43:55 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.Gamebet
{
    public enum BackendResult
    {
        SUCCESS,

        // Errors
        EXPIRED_RESPONSE,
        CHALLENGE_COMPLETE,
        NO_INTERNET_REACHABILITY,
        BACKEND_CONNECT_FAILED,
        FACEBOOK_INIT_FAILED,
        PING_REQUEST_FAILED,
        AUTH_GUEST_REQUEST_FAILED,
        AUTH_FACEBOOK_REQUEST_FAILED,
        AUTH_FACEBOOK_REQUEST_CANCELLED,
        GET_INIT_DATA_REQUEST_FAILED,
        GET_APP_VERSION_DATA_REQUEST_FAILED,
        ACCOUNT_DETAILS_REQUEST_FAILED,
        MATCH_NOT_FOUND,
        MATCHMAKING_REQUEST_FAILED,
        GET_GAME_START_TIME_REQUEST_FAILED,
        TAKE_TURN_REQUEST_FAILED,
        AI_TAKE_TURN_REQUEST_FAILED,
        AI_RESIGN_REQUEST_FAILED,
        SET_PLAYER_SOCIAL_NAME_FAILED,
        CLAIM_FIFTY_MOVE_DRAW_FAILED,
        CLAIM_THREEFOLD_REPEAT_DRAW_FAILED,
        RESIGN_REQUEST_FAILED,
        CLAIM_AD_REWARD_REQUEST_FAILED,
        BUY_VIRTUAL_GOOD_FAILED,
        CONSUME_VIRTUAL_GOOD_FAILED,
        GRANT_FORGED_VIRTUAL_GOOD_FAILED,
        SELL_FORGE_CARDS_FAILED,
        CLAIM_LOOT_FAILED,
        UPDATE_ACTIVE_INVENTORY_FAILED,
        // Testing code
        SEND_TEST_REQUEST_FAILED,
        SEND_BACKEND_ERROR_SIGNAL
    }
}
