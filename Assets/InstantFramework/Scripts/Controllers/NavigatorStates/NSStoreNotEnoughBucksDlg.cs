/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

namespace TurboLabz.InstantFramework
{
	public class NSNotEnoughBucksDlg : NS
	{
		public override void RenderDisplayOnEnter()
		{
			ShowDialog(NavigatorViewId.NOT_ENOUGH_BUCKS_DLG);
		}

		public override NS HandleEvent(NavigatorEvent evt)
		{
			if (evt == NavigatorEvent.SHOW_STORE ||
				evt == NavigatorEvent.ESCAPE)
			{
				return new NSStore();
			}
			else if (evt == NavigatorEvent.SHOW_BUCK_PACKS_DLG) 
			{
                return new NSBuckPacksDlg();
			}

			return null;
		}
	}
}

