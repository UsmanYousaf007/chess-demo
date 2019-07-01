/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

namespace TurboLabz.InstantFramework
{
	public class NSStore : NS
	{
		public override void RenderDisplayOnEnter()
		{
			ShowView(NavigatorViewId.STORE);
		}

		public override NS HandleEvent(NavigatorEvent evt)
		{
			if (evt == NavigatorEvent.ESCAPE)
			{
				return null;
			}
            else if (evt == NavigatorEvent.SHOW_LOBBY)
            {
                return new NSLobby();
            }
            else if (evt == NavigatorEvent.SHOW_FRIENDS)
            {
                return new NSFriends();
            }
            else if (evt == NavigatorEvent.SHOW_BUY_DLG)
			{
				return new NSBuyDlg();
			}
			else if (evt == NavigatorEvent.SHOW_NOT_ENOUGH_DLG)
			{
				return new NSNotEnoughBucksDlg();
			}
            else if (evt == NavigatorEvent.SHOW_PURCHASED_DLG)
            {
                return new NSPurchasedDlg();
            }

            return null;
		}
	}
}

