namespace TurboLabz.InstantFramework
{
    public class NSConfirmDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.CONFIRM_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            NavigatorViewId viewId = CameFrom(NavigatorViewId.LOBBY, NavigatorViewId.CPU, NavigatorViewId.MULTIPLAYER, NavigatorViewId.SUBSCRIPTION_DLG, NavigatorViewId.MANAGE_BLOCKED_FRIENDS, NavigatorViewId.TOPICS_VIEW, NavigatorViewId.SHOP, NavigatorViewId.SPOT_PURCHASE_DLG, NavigatorViewId.INVENTORY, NavigatorViewId.LESSONS_VIEW);

            if (evt == NavigatorEvent.ESCAPE)
            {
                if (viewId == NavigatorViewId.LOBBY)
                {
                    return new NSLobby();
                }
                else if (viewId == NavigatorViewId.SUBSCRIPTION_DLG)
                {
                    cmd.hideViewSignal.Dispatch(NavigatorViewId.CONFIRM_DLG);
                    return new NSSubscriptionDlg();
                }
                else if (viewId == NavigatorViewId.CPU)
                {
                    return new NSCPU();
                }
                else if (viewId == NavigatorViewId.MULTIPLAYER)
                {
                    return new NSMultiplayer();
                }
                else if (viewId == NavigatorViewId.MANAGE_BLOCKED_FRIENDS)
                {
                    return new NSManageBlockedFriends();
                }
                else if (viewId == NavigatorViewId.TOPICS_VIEW)
                {
                    return new NSLessonTopics();
                }
                else if (viewId == NavigatorViewId.SHOP)
                {
                    return new NSShop();
                }
                else if (viewId == NavigatorViewId.SPOT_PURCHASE_DLG)
                {
                    cmd.hideViewSignal.Dispatch(NavigatorViewId.CONFIRM_DLG);
                    return new NSSpotPurchase();
                }
                else if (viewId == NavigatorViewId.INVENTORY)
                {
                    return new NSInventory();
                }
                else if (viewId == NavigatorViewId.LESSONS_VIEW)
                {
                    return new NSLessonsView();
                }
                else if (viewId == NavigatorViewId.PROMOTION_CHESS_COURSE_DLG)
                {
                    cmd.hideViewSignal.Dispatch(NavigatorViewId.CONFIRM_DLG);
                    return new NSPromotionChessCourseDlg();
                }
                else if (viewId == NavigatorViewId.PROMOTION_CHESS_SETS_BUNDLE_DLG)
                {
                    cmd.hideViewSignal.Dispatch(NavigatorViewId.CONFIRM_DLG);
                    return new NSPromotionChessSetsBundleDlg();
                }
                else if (viewId == NavigatorViewId.PROMOTION_ELITE_BUNDLE_DLG)
                {
                    cmd.hideViewSignal.Dispatch(NavigatorViewId.CONFIRM_DLG);
                    return new NSPromotionEliteBundleDlg();
                }
                else if (viewId == NavigatorViewId.PROMOTION_REMOVE_ADS_DLG)
                {
                    cmd.hideViewSignal.Dispatch(NavigatorViewId.CONFIRM_DLG);
                    return new NSPromotionRemoveAdsDlg();
                }
                else if (viewId == NavigatorViewId.PROMOTION_REMOVE_ADS_SALE_DLG)
                {
                    cmd.hideViewSignal.Dispatch(NavigatorViewId.CONFIRM_DLG);
                    return new NSPromotionRemoveAdsSaleDlg();
                }
                else if (viewId == NavigatorViewId.PROMOTION_WELCOME_BUNDLE_DLG)
                {
                    cmd.hideViewSignal.Dispatch(NavigatorViewId.CONFIRM_DLG);
                    return new NSPromotionWelcomeBundleDlg();
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
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER_RESULTS_DLG && CameFrom(NavigatorViewId.MULTIPLAYER) == NavigatorViewId.MULTIPLAYER)
            {
                cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
                return new NSMultiplayerResultsDlg();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER_FIFTY_MOVE_DRAW_DLG && CameFrom(NavigatorViewId.MULTIPLAYER) == NavigatorViewId.MULTIPLAYER)
            {
                cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
                return new NSMultiplayerFiftyMoveDrawDlg();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER_THREEFOLD_REPEAT_DRAW_DLG && CameFrom(NavigatorViewId.MULTIPLAYER) == NavigatorViewId.MULTIPLAYER)
            {
                cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
                return new NSMultiplayerThreeFoldRepeatDrawDlg();
            }
            else if (evt == NavigatorEvent.SHOW_INBOX)
            {
                return new NSInboxView();
            }
            else if (evt == NavigatorEvent.SHOW_ARENA)
            {
                return new NSArenaView();
            }

            return null;
        }
    }
}
