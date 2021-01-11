/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

namespace TurboLabz.InstantFramework
{
    public class NSRateAppDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.RATE_APP_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            NavigatorViewId viewId = CameFrom (
                NavigatorViewId.LOBBY,
                NavigatorViewId.FRIENDS);
            
            if (evt == NavigatorEvent.ESCAPE) 
            {
                if (viewId == NavigatorViewId.LOBBY) 
                {
                    return new NSLobby();
                } 
                else if (viewId == NavigatorViewId.FRIENDS)
                {
                    return new NSFriends();
                }
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER)
            {
                return new NSMultiplayer();
            }
            else if (evt == NavigatorEvent.SHOW_CHAT)
            {
                return new NSChat();
            }
            else if (evt == NavigatorEvent.SHOW_SUBSCRIPTION_DLG)
            {
                return new NSSubscriptionDlg();
            }
            else if (evt == NavigatorEvent.SHOW_INBOX)
            {
                return new NSInboxView();
            }
            else if (evt == NavigatorEvent.SHOW_ARENA)
            {
                return new NSArenaView();
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
            else if (evt == NavigatorEvent.SHOW_REWARD_DLG_V2)
            {
                return new NSRewardDlg();
            }

            return null;
        }
    }
}

