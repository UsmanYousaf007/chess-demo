namespace TurboLabz.InstantFramework
{
    public class NSPromotionWelcomeBundleDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.PROMOTION_WELCOME_BUNDLE_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            NavigatorViewId viewId = CameFrom(NavigatorViewId.LOBBY, NavigatorViewId.THEME_SELECTION, NavigatorViewId.MULTIPLAYER,
                NavigatorViewId.CPU, NavigatorViewId.SETTINGS, NavigatorViewId.RATE_APP_DLG, NavigatorViewId.SHOP,
                NavigatorViewId.LESSONS_VIEW, NavigatorViewId.TOPICS_VIEW, NavigatorViewId.LESSON_VIDEO);

            if (evt == NavigatorEvent.ESCAPE)
            {
                if (viewId == NavigatorViewId.LOBBY)
                {
                    return new NSLobby();
                }
                else if (viewId == NavigatorViewId.THEME_SELECTION)
                {
                    cmd.hideViewSignal.Dispatch(NavigatorViewId.PROMOTION_REMOVE_ADS_SALE_DLG);
                    return new NSThemeSelectionDlg();
                }
                else if (viewId == NavigatorViewId.SETTINGS)
                {
                    return new NSSettings();
                }
                else if (viewId == NavigatorViewId.MULTIPLAYER)
                {
                    cmd.disableModalBlockersSignal.Dispatch();
                    return new NSMultiplayer();
                }
                else if (viewId == NavigatorViewId.CPU)
                {
                    cmd.disableModalBlockersSignal.Dispatch();
                    return new NSCPU();
                }
                else if (viewId == NavigatorViewId.RATE_APP_DLG)
                {
                    cmd.hideViewSignal.Dispatch(NavigatorViewId.PROMOTION_REMOVE_ADS_SALE_DLG);
                    return new NSRateAppDlg();
                }
                else if (viewId == NavigatorViewId.LESSONS_VIEW)
                {
                    return new NSLessonsView();
                }
                else if (viewId == NavigatorViewId.TOPICS_VIEW)
                {
                    return new NSLessonTopics();
                }
                else if (viewId == NavigatorViewId.LESSON_VIDEO)
                {
                    return new NSLessonVideo();
                }
                else if (viewId == NavigatorViewId.SHOP)
                {
                    return new NSShop();
                }
            }
            else if (evt == NavigatorEvent.SHOW_CHAT)
            {
                return new NSChat();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER)
            {
                return new NSMultiplayer();
            }
            else if (evt == NavigatorEvent.SHOW_CONFIRM_DLG)
            {
                return new NSConfirmDlg();
            }
            else if (evt == NavigatorEvent.SHOW_THEME_SELECTION_DLG)
            {
                return new NSThemeSelectionDlg();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER_RESULTS_DLG && viewId == NavigatorViewId.MULTIPLAYER)
            {
                cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
                return new NSMultiplayerResultsDlg();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER_FIFTY_MOVE_DRAW_DLG && viewId == NavigatorViewId.MULTIPLAYER)
            {
                cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
                return new NSMultiplayerFiftyMoveDrawDlg();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER_THREEFOLD_REPEAT_DRAW_DLG && viewId == NavigatorViewId.MULTIPLAYER)
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
