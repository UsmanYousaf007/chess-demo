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
        manage_blocked_friends,
        lessons_videos,
        lessons_topics,
        lessons_play,
        inventory,
        spot_purchase_dlg,
        spot_coin_purchase_dlg,
        arena,
        tournament_leaderboard,
        inbox,
        league_perks,
        spot_inventory_dlg
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
        ftue_gdpr_accept,

        //early inidicator
        install_game_count,
        install_game_fav_mode,

        //power_ups
        power_ups_used,
        booster_used,

        //lessons
        all_lessons_complete,

        //profile pic
        upload_picture,

        //monetisation
        inventory_rewarded_video_watched,
        banner_shown,
        banner_clicked,
        banner_purchased,
        booster_shown,
        shop_purchase,
        shop_popup_view,
        shop_popup_purchase,
        inventory_source,
        items_owned,
        key_obtained_rv,
        key_obtained_gem,
        resource_used,
        resource_via_gems,
        resource_via_videos,
        resource_via_free,
        resource_via_bundle,
        coin_popup_purchase,
        gems_used,
        bet_increment_used,
        bet_increment_default,

        //inbox
        inbox_visits,
        inbox_tournament_reward_collected,
        inbox_daily_league_reward_collected,
        inbox_subscription_reward_collected,

        //tournaments
        tap_tier_info,
        start_tournament,
        tap_notification,
        tournament_start_location,
        championship_finish_rank,
        current_league,
        engaged_finish_rank,
        trophies_earned,
        tournament_first_game_start_location,
        championship_coins_by_rank,

        //navigation
        navigation_clicked,

        //promotions
        promotion_dlg_shown,
        promotion_dlg_purchased
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
        threemin_match,
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
        interstitial_tournament_pregame,
        interstitial_tournament_endcard_continue,
        interstitial_in_game_cpu,
        interstitial_in_game_30_min,
        interstitial_in_game_classic,
        rewarded_daily_doubler,
        rewarded_lobby_coins_chest,
        rewarded_out_of_coins_lobby_banner_popup,
        rewarded_out_of_coins_lobby_popup,
        rewarded_coins_spot_state_1,

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
        friends,

        //lessons
        started,
        completed,
        unlocked,

        //power_ups
        coach,
        move_meter,

        //profile picture
        dlg_shown,        
        choose_existing,
        take_new,

        //monetization
        rating_booster,
        lucky_gem_booster,
        hint,
        key,
        rewarded_rating_booster,
        rewarded_hints,
        rewarded_keys,
        rewarded_gem_booster,
        rewarded_tickets,
        lobby_lessons_pack,
        lobby_themes_pack,
        lobby_remove_ads,
        lobby_collect_rewards,
        lobby_out_of_coins,
        unlock_all_themes,
        unlock_all_lessons,
        lobby_update_banner,
        lobby_subscription_banner,
        ratingBooster,
        gems,
        ticket,
        popup_rating_booster,
        popup_hint,
        popup_key,
        popup_ticket,
        rewarded_rating_booster_popup,
        rewarded_hints_popup,
        rewarded_keys_popup,
        rewarded_tickets_popup,
        inventory_nav,
        tournament_main,
        tournament_leaderboard,
        lessons,
        themes,
        cpu_pre_game_power_mode,
        cpu_in_game_power_mode,
        power_mode,
        coin_doubler,
        lesson,
        theme,
        coins,

        //tournament
        main,
        end_game_card,
        tournaments_tab,
        lobby,

        //navigation
        inventory,
        games,
        arena,

        //promotions
        remove_ads,
        remove_ads_fire_sale,
        welcome,
        elite,
        lessons_pack,
        themes_pack,
        annual_mega_sale
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
        num_facebook_friends,
        gems,
        tickets,
        rating_boosters,
        hints,
        keys,
        coins
    }
}

