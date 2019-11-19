/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

namespace TurboLabz.InstantFramework
{
	public class NSBuyDlg : NS
	{
		public override void RenderDisplayOnEnter()
		{
            ShowDialog(NavigatorViewId.BUY_DLG);
		}

		public override NS HandleEvent(NavigatorEvent evt)
		{
            NavigatorViewId viewId = CameFrom(
                NavigatorViewId.STORE,
                NavigatorViewId.SPOT_PURCHASE_DLG);

            if (evt == NavigatorEvent.ESCAPE)
            {
                if (viewId == NavigatorViewId.STORE)
                {
                    return new NSStore();
                }
                else if (viewId == NavigatorViewId.SPOT_PURCHASE_DLG)
                {
                    NavigatorViewId spotParentViewId = CameFrom( NavigatorViewId.MULTIPLAYER, NavigatorViewId.CPU);

                    if (spotParentViewId == NavigatorViewId.CPU)
                    {
                        cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CPU);
                    }
                    else if (spotParentViewId == NavigatorViewId.MULTIPLAYER)
                    {
                        cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
                    }
                    return new NSSpotPurchaseDlg();
                }
            }
            else if (evt == NavigatorEvent.SHOW_CPU)
            {
                return new NSCPU();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER)
            {
                return new NSMultiplayer();
            }
            else if (evt == NavigatorEvent.SHOW_CHAT)
            {
                return new NSChat();
            }
            return null;
		}
	}
}

