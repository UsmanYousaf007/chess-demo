/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

namespace TurboLabz.InstantFramework
{
    public class NSSpotPurchaseDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.SPOT_PURCHASE_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.SHOW_MULTIPLAYER)
            {
                return new NSMultiplayer();
            }
            else if (evt == NavigatorEvent.SHOW_CPU)
            {
                return new NSCPU();
            }

            NavigatorViewId viewId = CameFrom(
                NavigatorViewId.MULTIPLAYER,
                NavigatorViewId.CPU,
                NavigatorViewId.LOBBY);

            if (evt == NavigatorEvent.ESCAPE)
            {
                if (viewId == NavigatorViewId.MULTIPLAYER)
                {
                    return new NSMultiplayer();
                }
                else if (viewId == NavigatorViewId.CPU)
                {
                    return new NSCPU();
                }
                else if (viewId == NavigatorViewId.LOBBY)
                {
                    return new NSLobby();
                }
            }
            else if (evt == NavigatorEvent.SHOW_CPU_RESULTS_DLG)
            {
                cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CPU);
                return new NSCPUResultsDlg();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER_RESULTS_DLG)
            {
                cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
                return new NSMultiplayerResultsDlg();
            }
            else if (evt == NavigatorEvent.SHOW_BUY_DLG)
            {
                return new NSBuyDlg();
            }
            else if (evt == NavigatorEvent.SHOW_CHAT)
            {
                return new NSChat();
            }
            else if (evt == NavigatorEvent.SHOW_CONFIRM_DLG)
            {
                return new NSConfirmDlg();
            }

            return null;
        }
    }
}

