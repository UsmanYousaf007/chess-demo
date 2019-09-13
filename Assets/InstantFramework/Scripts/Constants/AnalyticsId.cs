/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-12-08 22:47:21 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.InstantFramework
{
    public enum AnalyticsScreen
    {
        lobby,
        profile,
        friends,
        shop,
        themes,
        coins,
        powerups,
        quick_match,
        computer_match,
        long_match,
        rate_dialog,
        spot_purchase_safe_move,
        spot_purchase_hint,
        spot_purchase_hindsight
    }

    public enum AnalyticsEventId
    {
        tap_share,
        tap_support,
        tap_coins,
        tap_lobby_bundle,
        tap_community_refresh,
        tap_long_match_create,
        tap_long_match_accept,
        tap_long_match_decline,
        tap_long_match_remove,
        tap_long_match_profile,
        tap_rate_yes,
        tap_rate_no,
        tap_chat_message_send,
        tap_pow_info,
        tap_pow_safe_move_off,
        tap_pow_safe_move_on,
        tap_pow_safe_move_undo,
        tap_pow_hint,
        tap_pow_hindsight,
        spot_purchase_complete,
        store_purchase_complete,
        ads_collect_reward,
        ads_skip_reward,
        ads_friends_back,
        session_fb,
        session_guest,
        session_premium,
        friends_community,
        friends_facebook,
        friends_blocked,
        friends_active_games,
        level_complete,
        level_fail,
        player_elo,
        bot_quick_match_won,
        bot_quick_match_lost,
        long_match_complete_duration,
        selected_theme,
        disconnection_time,
        ads_rewared_request,
        ads_rewared_success,
        ads_rewared_show,
        ads_rewared_available,
        ads_interstitial_request,
        ads_interstitial_success,
        ads_interstitial_show,
        ads_interstitial_available
    }

    public enum AnalyticsContext
    {
        unknown,
        long_match,
        quick_match,
        computer_match,
        shop
    }

    public enum AnalyticsParameter
    {
        is_ranked,
        item_id,
        count,
        level_difficulty,
        elo,
        bot_difficulty,
        duration,
        theme_name,
        fb_logged_in,
        is_bot,
        context
    }

}

