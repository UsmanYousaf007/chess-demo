﻿namespace TurboLabz.InstantFramework
{
    public class NSSpotPurchase : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.SPOT_PURCHASE_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            NavigatorViewId viewId = CameFrom(NavigatorViewId.INVENTORY, NavigatorViewId.TOURNAMENT_LEADERBOARD_VIEW, NavigatorViewId.SPOT_INVENTORY_DLG,
                NavigatorViewId.MULTIPLAYER, NavigatorViewId.CPU, NavigatorViewId.MULTIPLAYER_RESULTS_DLG, NavigatorViewId.LESSONS_VIEW);

            if (evt == NavigatorEvent.ESCAPE)
            {
                if (viewId == NavigatorViewId.INVENTORY)
                {
                    return new NSInventory();
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
                else if (viewId == NavigatorViewId.MULTIPLAYER_RESULTS_DLG)
                {
                    cmd.hideViewSignal.Dispatch(NavigatorViewId.SPOT_PURCHASE_DLG);
                    return new NSMultiplayerResultsDlg();
                }
                else if (viewId == NavigatorViewId.LESSONS_VIEW)
                {
                    return new NSLessonsView();
                }
                else if (viewId == NavigatorViewId.TOURNAMENT_LEADERBOARD_VIEW)
                {
                    return new NSTournamentLeaderboard();
                }
                else if (viewId == NavigatorViewId.SPOT_INVENTORY_DLG)
                {
                    cmd.hideViewSignal.Dispatch(NavigatorViewId.SPOT_PURCHASE_DLG);
                    return new NSSpotInventory();
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
            else if (evt == NavigatorEvent.SHOW_INBOX)
            {
                return new NSInboxView();
            }
            else if (evt == NavigatorEvent.SHOW_ARENA)
            {
                return new NSArenaView();
            }
            else if (evt == NavigatorEvent.SHOW_CONFIRM_DLG)
            {
                return new NSConfirmDlg();
            }

            return null;
        }
    }
}
