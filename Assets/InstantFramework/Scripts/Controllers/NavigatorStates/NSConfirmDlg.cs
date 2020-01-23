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
            NavigatorViewId viewId = CameFrom(NavigatorViewId.LOBBY, NavigatorViewId.CPU, NavigatorViewId.MULTIPLAYER, NavigatorViewId.SUBSCRIPTION_DLG);

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
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER)
            {
                return new NSMultiplayer();
            }
            else if (evt == NavigatorEvent.SHOW_CHAT)
            {
                return new NSChat();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER_RESULTS_DLG && viewId == NavigatorViewId.SUBSCRIPTION_DLG)
            {
                if (CameFrom(NavigatorViewId.MULTIPLAYER) == NavigatorViewId.MULTIPLAYER)
                {
                    cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
                    return new NSMultiplayerResultsDlg();
                }
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER_FIFTY_MOVE_DRAW_DLG && viewId == NavigatorViewId.MULTIPLAYER)
            {
                if (CameFrom(NavigatorViewId.MULTIPLAYER) == NavigatorViewId.MULTIPLAYER)
                {
                    cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
                    return new NSMultiplayerFiftyMoveDrawDlg();
                }
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER_THREEFOLD_REPEAT_DRAW_DLG && viewId == NavigatorViewId.MULTIPLAYER)
            {
                if (CameFrom(NavigatorViewId.MULTIPLAYER) == NavigatorViewId.MULTIPLAYER)
                {
                    cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
                    return new NSMultiplayerThreeFoldRepeatDrawDlg();
                }
            }
            return null;
        }
    }
}
