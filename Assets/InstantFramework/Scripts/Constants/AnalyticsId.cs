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
        spot_purchase_hindsight,
        spot_purchase_move_meter,
        spot_purchase_coach,
        skill_level_dlg,
        theme_selection_dlg,
        subscription_dlg,
        show_settings
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
        v1_spot_purchase_complete,
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
        quickmatch_direct_request,
        quickmatch_direct_request_accept,
        quickmatch_direct_request_timeout_ingame,
        ads_rewarded_request,
        ads_rewarded_success,
        ads_rewarded_show,
        ads_rewarded_available,
        ads_rewarded_failed,
        ads_rewarded_clicked,
        ads_rewarded_not_available,
        ads_rewarded_skipped,
        ads_interstitial_request,
        //ads_interstitial_success,
        ads_interstitial_show,
        ads_interstitial_failed,
        tap_pow_move_meter,
        cancel_pow_move_meter,
        close_pow_move_meter,
        tap_pow_coach,
        cancel_pow_coach,
        close_pow_coach,
        imp_banner_subscription,
        tap_banner_subscription,
        tap_coach_after_tooltip,
        tap_move_meter_after_tooltip,
        tap_coach_after_training,
        tap_move_meter_after_training,
        tap_yes_skill_level_dlg,
        tap_no_skill_level_dlg,
        tap_chat_player_profile,
        out_game_chat_match_active,
        out_game_chat_match_inactive,
        tap_add_friends,
        tap_remove_friends,
        start_match_with_favourite,
        powerup_usage_no,
        powerup_usage_low,
        powerup_usage_avg,
        powerup_usage_good,
        powerup_usage_awesome,
        time_spent_quick_macth,
        time_spent_long_match,
        time_spent_cpu_match,
        time_spent_lobby,
        first_game_started,
        quick_match_player,
        long_match_player,
        cpu_mactch_player,
        multi_mode_player,
        //apps flyer events for HUUUGE
        launch,
        ad_clicked,
        ad_displayed,
        video_started,
        video_finished,
        game_started,
        game_finished,
        continuous_play,
        focus_lost,
        subscription_dlg_shown,
        get_free_trial_clicked,
        subscription_purchased
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
        context,
        day,
        minutes
    }

}

