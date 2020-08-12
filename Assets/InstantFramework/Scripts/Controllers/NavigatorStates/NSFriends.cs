/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

namespace TurboLabz.InstantFramework
{
    public class NSFriends : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.FRIENDS);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.SHOW_MULTIPLAYER)
            {
                return new NSMultiplayer();
            }
            if (evt == NavigatorEvent.SHOW_PROFILE_DLG)
            {
                return new NSProfileDlg();
            }
            else if (evt == NavigatorEvent.ESCAPE)
            {
                cmd.androidNativeService.SendToBackground();
            }
            else if (evt == NavigatorEvent.SHOW_LOBBY)
            {
                return new NSLobby();
            }
            else if (evt == NavigatorEvent.SHOW_STATS)
            {
                return new NSStats();
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
            else if (evt == NavigatorEvent.SHOW_SETTINGS)
            {
                return new NSSettings();
            }
            else if (evt == NavigatorEvent.SHOW_THEME_SELECTION_DLG)
            {
                return new NSThemeSelectionDlg();
            }
            else if (evt == NavigatorEvent.SHOW_EARN_REWARDS_DLG)
            {
                return new NSEarnRewardsDlg();
            }
            else if (evt == NavigatorEvent.CREATE_MATCH_LIMIT_REACHED_DIALOG)
            {
                return new NSLimitReachedDlg();
            }
            else if (evt == NavigatorEvent.SHOW_MANAGE_BLOCKED_FRIENDS)
            {
                return new NSManageBlockedFriends();
            }
            else if (evt == NavigatorEvent.SHOW_START_GAME_DLG)
            {
                return new NSStartGameDlg();
            }
            else if (evt == NavigatorEvent.SHOW_INVITE_DLG)
            {
                return new NSInviteDlg();
            }
            else if (evt == NavigatorEvent.SHOW_REMOVE_FRIEND_DLG)
            {
                return new NSRemoveFriendDlg();
            }
            else if (evt == NavigatorEvent.SHOW_FIND_YOUR_FRIEND_DLG)
            {
                return new NSFindFriendsDlg();
            }
            else if (evt == NavigatorEvent.SHOW_SHOP)
            {
                return new NSShop();
            }

            return null;
        }
    }
}

