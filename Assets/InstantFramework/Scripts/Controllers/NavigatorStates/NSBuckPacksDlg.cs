/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

namespace TurboLabz.InstantFramework
{
	public class NSBuckPacksDlg : NS
	{
		public override void RenderDisplayOnEnter()
		{
			ShowDialog(NavigatorViewId.BUCK_PACKS_DLG);
		}

		public override NS HandleEvent(NavigatorEvent evt)
		{
			NavigatorViewId viewId = CameFrom (
                NavigatorViewId.LOBBY,
                NavigatorViewId.STORE,
                NavigatorViewId.STATS,
                NavigatorViewId.FRIENDS);

			if (evt == NavigatorEvent.ESCAPE) 
			{
				if (viewId == NavigatorViewId.LOBBY) 
				{
					return new NSLobby();
				} 
				else if (viewId == NavigatorViewId.STORE) 
				{
					return new NSStore();
				}
                else if (viewId == NavigatorViewId.STATS)
                {
                    return new NSStats();
                }
                else if (viewId == NavigatorViewId.FRIENDS)
                {
                    return new NSFriends();
                }
			}
			return null;
		}
	}
}

