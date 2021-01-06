/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class NSLobby : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.LOBBY);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.SHOW_CPU)
            {
                return new NSCPU();
            }
            else if (evt == NavigatorEvent.SHOW_STATS)
            {
                return new NSStats();
            }
            else if (evt == NavigatorEvent.SHOW_FRIENDS)
            {
                return new NSFriends();
            }
            else if (evt == NavigatorEvent.SHOW_PROFILE_DLG)
            {
                return new NSProfileDlg();
            }
            else if (evt == NavigatorEvent.ESCAPE)
            {
                cmd.androidNativeService.SendToBackground();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER)
            {
                return new NSMultiplayer();
            }
            else if (evt == NavigatorEvent.SHOW_RATE_APP_DLG)
            {
                return new NSRateAppDlg();
            }
            else if (evt == NavigatorEvent.SHOW_CHAT)
            {
                return new NSChat();
            }
            else if (evt == NavigatorEvent.SHOW_CONFIRM_DLG)
            {
                return new NSConfirmDlg();
            }
            else if (evt == NavigatorEvent.SHOW_THEME_SELECTION_DLG)
            {
                return new NSThemeSelectionDlg();
            }
            else if (evt == NavigatorEvent.SHOW_SUBSCRIPTION_DLG)
            {
                return new NSSubscriptionDlg();
            }
            else if (evt == NavigatorEvent.SHOW_SETTINGS)
            {
                return new NSSettings();
            }
            else if (evt == NavigatorEvent.SHOW_EARN_REWARDS_DLG)
            {
                return new NSEarnRewardsDlg();
            }
            else if (evt == NavigatorEvent.CREATE_MATCH_LIMIT_REACHED_DIALOG)
            {
                return new NSLimitReachedDlg();
            }
            else if (evt == NavigatorEvent.SHOW_START_GAME_DLG)
            {
                return new NSStartGameDlg();
            }
            else if (evt == NavigatorEvent.SHOW_REMOVE_FRIEND_DLG)
            {
                return new NSRemoveFriendDlg();
            }
            else if (evt == NavigatorEvent.SHOW_START_CPU_GAME_DLG)
            {
                return new NSStartCPUGame();
            }
            else if (evt == NavigatorEvent.SHOW_AD_SKIPPED_DLG)
            {
                return new NSAdSkippedDlg();
            }
            else if (evt == NavigatorEvent.SHOW_LESSON_VIDEO)
            {
                return new NSLessonVideo();
            }
            else if (evt == NavigatorEvent.SHOW_TOPICS_VIEW)
            {
                return new NSLessonTopics();
            }
            else if (evt == NavigatorEvent.SHOW_ARENA)
            {
                return new NSArenaView();
            }
            else if (evt == NavigatorEvent.SHOW_SHOP)
            {
                return new NSShop();
            }
            else if (evt == NavigatorEvent.SHOW_INVENTORY)
            {
                return new NSInventory();
            }
            else if (evt == NavigatorEvent.SHOW_INBOX)
            {
                return new NSInboxView();
            }
            else if (evt == NavigatorEvent.SHOW_TOURNAMENT_LEADERBOARDS)
            {
                return new NSTournamentLeaderboard();
            }
            else if (evt == NavigatorEvent.SHOW_INVITE_DLG)
            {
                return new NSInviteDlg();
            }
            else if (evt == NavigatorEvent.SHOW_SOCIAL_CONNECTION_DLG)
            {
                return new NSSocialConnectionDlg();
            }
            else if (evt == NavigatorEvent.SHOW_PROMOTION_REMOVE_ADS_DLG)
            {
                return new NSPromotionRemoveAdsDlg();
            }
            else if (evt == NavigatorEvent.SHOW_PROMOTION_REMOVE_ADS_SALE_DLG)
            {
                return new NSPromotionRemoveAdsSaleDlg();
            }
            else if (evt == NavigatorEvent.SHOW_PROMOTION_CHESS_COURSE_DLG)
            {
                return new NSPromotionChessCourseDlg();
            }
            else if (evt == NavigatorEvent.SHOW_PROMOTION_CHESS_SETS_BUNDLE_DLG)
            {
                return new NSPromotionChessSetsBundleDlg();
            }
            else if (evt == NavigatorEvent.SHOW_PROMOTION_ELITE_BUNDLE_DLG)
            {
                return new NSPromotionEliteBundleDlg();
            }
            else if (evt == NavigatorEvent.SHOW_PROMOTION_WELCOME_BUNDLE_DLG)
            {
                return new NSPromotionWelcomeBundleDlg();
            }
            else if (evt == NavigatorEvent.SHOW_SUBSCRIPTION_SALE_DLG)
            {
                return new NSSubscriptionSaleDlg();
            }
            else if (evt == NavigatorEvent.SHOW_REWARD_DLG)
            {
                return new NSRewardDlgView();
            }
            else if (evt == NavigatorEvent.SHOW_LOGIN_DLG)
            {
                return new NSLoginDlg();
            }
            else if (evt == NavigatorEvent.SHOW_LEADERBOARD_VIEW)
            {
                return new NSLeaderboardView();
            }
            else if (evt == NavigatorEvent.SHOW_LEAGUE_PERKS_VIEW)
            {
                return new NSLeaguePerksView();
            }
            else if (evt == NavigatorEvent.SHOW_SELECT_TIME_MODE)
            {
                return new NSSelectTimeMode();
            }
            else if (evt == NavigatorEvent.SHOW_CHAMPIONSHIP_RESULT_DLG)
            {
                return new NSChampionshipResultDlg();
            }
            else if (evt == NavigatorEvent.SHOW_CHAMPIONSHIP_NEW_RANK_DLG)
            {
                return new NSChampionshipNewRankDlg();
            }
            else if (evt == NavigatorEvent.SHOW_SPOT_COIN_PURCHASE)
            {
                return new NSSpotCoinPurchaseDlg();
            }
            else if (evt == NavigatorEvent.SHOW_DAILY_REWARD_DLG)
            {
                return new NSDailyRewardDlg();
            }
            else if (evt == NavigatorEvent.SHOW_SPOT_PURCHASE)
            {
                return new NSSpotPurchase();
            }

            return null;
        }
    }
}

