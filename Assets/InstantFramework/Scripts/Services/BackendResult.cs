/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

namespace TurboLabz.InstantFramework
{
    public enum BackendResult
    {
        NONE,
        SUCCESS,
        CANCELED,

        // Errors
        //EXPIRED_RESPONSE,
        //CHALLENGE_COMPLETE,
        NO_INTERNET_REACHABILITY,
        //BACKEND_CONNECT_FAILED,
        //FACEBOOK_INIT_FAILED,
        PING_REQUEST_FAILED,
        AUTH_GUEST_REQUEST_FAILED,
        AUTH_FACEBOOK_REQUEST_FAILED,
        //AUTH_FACEBOOK_REQUEST_CANCELLED,
        GET_INIT_DATA_REQUEST_FAILED,
        //GET_APP_VERSION_DATA_REQUEST_FAILED,
        //ACCOUNT_DETAILS_REQUEST_FAILED,
        //MATCH_NOT_FOUND,
        MATCHMAKING_REQUEST_FAILED,
        GET_GAME_START_TIME_REQUEST_FAILED,
        TAKE_TURN_REQUEST_FAILED,
        AI_TAKE_TURN_REQUEST_FAILED,
        AI_RESIGN_REQUEST_FAILED,
        SET_PLAYER_SOCIAL_NAME_FAILED,
        CLAIM_FIFTY_MOVE_DRAW_FAILED,
        CLAIM_THREEFOLD_REPEAT_DRAW_FAILED,
        RESIGN_REQUEST_FAILED,
        //DECLINE_REQUEST_FAILED,
        CLAIM_REWARD_REQUEST_FAILED,
        CREATE_LONG_MATCH_FAILED,
        BUY_VIRTUAL_GOOD_FAILED,
        CONSUME_VIRTUAL_GOOD_FAILED,
        UPDATE_ACTIVE_INVENTORY_FAILED,
        SESSION_TERMINATED_ON_MULTIPLE_AUTH,
        VERIFY_REMOTE_STORE_PURCHASE_FAILED,
		FRIENDS_OP_FAILED,
        PUSH_NOTIFICATION_REGISTRATION_FAILED,
        ACCEPT_FAILED,
        DECLINE_FAILED,
        UNREGISTER_FAILED,
        //SYNC_FACEBOOK_AUTH_REQUEST_FAILED,
        SEND_CHAT_MESSAGE_FAILED,
        AUTH_EMAIL_REQUEST_FAILED,
        //RESUME_MATCH_DATA_FAILED,
        REQUEST_TIMEOUT,

        // Testing code
        SEND_TEST_REQUEST_FAILED,
        //SEND_BACKEND_ERROR_SIGNAL,
        GAME_CRAHSED,
        PURCHASE_ATTEMPT,
        PURCHASE_COMPLETE,
        PURCHASE_CANCEL,
        PURCHASE_FAILED,
        ONLINECHECKER_REQUEST_FAILED,
        UPDATE_PLAYER_DATA_FAILED,
        WATCHDOG_PING_ACK_FAILED,
        SYNC_RECONNECT_DATA_FAILED,
        NOT_AUTHORIZED,
        OFFER_DRAW_OP_FAILED,
        AUTH_SIGN_IN_WITH_APPLE_FAILED,

        UPLOAD_URL_GET_FAILED,
        DOWNLOAD_URL_GET_FAILED
    }
}
