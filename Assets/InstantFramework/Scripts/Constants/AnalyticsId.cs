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
        //Engineering tools
        reconnection_shown,
        gs_disconneced,
        gs_call_fail,
        gs_disconnected_unique,
        internet_warning_on_splash,
        disconnection_time,
        app_quit_during_disconnected,
        target_architecture,

        //Ads
        ad_user_requested,
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
        cross_promo_clicked,

        //UI Interaction
        refresh_community,
        remove_strip_clicked,

        //Session
        session_facebook,
        session_apple_id,
        session_guest,

        //CPU Levels Distribution
        cpu_end_lvl_,

        //Match Start Distrubtion
        match_find,

        //offer draw
        offer_draw,

        //subscription
        subscription_dlg_shown,
        subscription_session,
        subscription_purchased,
        subscription_renewed,
        subscription_dlg_purchased,

        //elo distribution
        elo,

        //match end distribution
        match_end,

        //FTUE funnel
        ftue_app_launch,
        ftue_gdpr,
        ftue_skill_level_dlg,
        ftue_intstall_popup,
        ftue_lobby,

        //early inidicator
        install_game_count,
        install_game_fav_mode
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
        onemin_match,
        thirtymin_match,
        ARM64,
        ARM,

        //Ads
        rewarded,
        interstitial,
        interstitial_pregame,
        interstitial_endgame,
        interstitial_rewarded_failed_replacement,
        interstitial_rewarded_capped_replacement,
        banner,

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
        cancelled,
        sent,

        //Subscription
        yearly,
        monthly,
        premium,

        //Match End Distribution
        clock,
        draw,
        check_mate,
        resign,
        disconect,
        clock_bot_win,
        clock_player_win,
        check_mate_bot_win,
        check_mate_player_win,
        resign_bot_win,
        resign_player_win,
        declined,

        //remove strip
        recently_played,
        friends
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
        seconds,
        num_facebook_friends
    }
}

