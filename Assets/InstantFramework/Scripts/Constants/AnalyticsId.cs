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
        show_settings,
        manage_subscription,
        manage_blocked_friends
    }

    public enum AnalyticsEventId
    {
        tap_share,
        tap_support,
        tap_coins,
        tap_lobby_bundle,
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
        ad_user_requested,
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
        
        long_match_complete_duration,
        selected_theme,
        disconnection_time,

        //random quick match
        quickmatch_direct_request_timeout_ingame,
        bot_quick_match_won,
        bot_quick_match_lost,


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

        //powerups
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
        mode_distribution,
        reconnection_shown,
        gs_disconneced,
        gs_call_fail,
        internet_warning_on_splash,

        tap_match,
        tap_resign_game,
        playerturn_timer_runs_out_long,
        played_bot_match,
        played_online_match,
        match_timer_runs_out,


        //Ads
        ad_requested,
        ad_not_available,
        ad_available,
        ad_shown,
        ad_failed,
        ad_player_shutdown,
        ad_cap_reached,
        ad_skipped,
        ad_clicked,
        ad_completed,

        //apps flyer events for HUUUGE
        launch,
        ad_displayed,
        video_started,
        video_finished,
        game_started,
        game_finished,
        continuous_play,
        focus_lost,
        subscription_dlg_shown,
        get_free_trial_clicked,
        subscription_purchased,
        cross_promo_clicked,
        terms_clicked,
        close_subscription_clicked,

        app_quit_during_disconnected,

        //UI Interaction
        refresh_community,

        //Session
        session_facebook,

        //CPU Levels Distribution
        cpu_end_lvl_,

        //Match Start Distrubtion
        match_find_random,
        match_find_friends,
        match_find_friends_notification_in_app,
        match_find_friends_notification_out_app,
        match_find_community,
        match_find_community_notification_in_app,
        match_find_community_notification_out_app,
        classic_match_find_random,
        classic_match_find_friends,
        classic_match_find_community,
        match_find

    }

    public enum AnalyticsContext
    {
        unknown,
        long_match,
        quick_match,
        computer_match,
        random_long_match,
        shop,
        gs_disconnect,
        internet_disconnect,
        internet_switch,
        in_game,
        not_in_game,
        bot_match,
        tenmin_match,
        return_from_background,
        cpu_match,
        matchmaking,

        //Ads
        rewarded,
        interstitial_pregame,
        interstitial_endgame,
        interstitial_rewarded_failed_replacement,
        interstitial_rewarded_capped_replacement,

        //Session
        num_facebook_friends,

        //CPU Levels Distribution
        won_checkmate,
        won_resign,
        lost_checkmate,
        lost_resign,
        draw_agreement,
        draw_stalemate,
        draw_insufficient_material,
        draw_fifty_move,
        draw_threefold_repetition,

        //Match Start Distrubtion
        start_attempt,
        success,
        failed,
        success_bot,
        accepted,
        rejected,
        cancelled

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
        minutes,
        num_facebook_friends
    }

}

