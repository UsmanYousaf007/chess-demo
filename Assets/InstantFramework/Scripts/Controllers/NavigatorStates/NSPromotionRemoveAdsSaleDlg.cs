﻿namespace TurboLabz.InstantFramework
{
    public class NSPromotionRemoveAdsSaleDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.PROMOTION_REMOVE_ADS_SALE_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            NavigatorViewId viewId = CameFrom(NavigatorViewId.LOBBY, NavigatorViewId.MULTIPLAYER, NavigatorViewId.CPU, NavigatorViewId.RATE_APP_DLG);

            if (evt == NavigatorEvent.ESCAPE)
            {
                if (viewId == NavigatorViewId.LOBBY)
                {
                    return new NSLobby();
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

            return null;
        }
    }
}
