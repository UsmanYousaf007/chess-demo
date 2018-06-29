/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

namespace TurboLabz.InstantFramework
{
    public enum BackendResult
    {
        SUCCESS,
        CANCELED,

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
        CLAIM_REWARD_REQUEST_FAILED,
        BUY_VIRTUAL_GOOD_FAILED,
        CONSUME_VIRTUAL_GOOD_FAILED,
        UPDATE_ACTIVE_INVENTORY_FAILED,
        SESSION_TERMINATED_ON_MULTIPLE_AUTH,
        // Testing code
        SEND_TEST_REQUEST_FAILED,
        SEND_BACKEND_ERROR_SIGNAL
    }
}
