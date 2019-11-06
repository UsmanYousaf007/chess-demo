/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

namespace TurboLabz.InstantFramework
{
    public class NSRateAppDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.RATE_APP_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            NavigatorViewId viewId = CameFrom (
                NavigatorViewId.LOBBY,
                NavigatorViewId.FRIENDS);
            
            if (evt == NavigatorEvent.ESCAPE) 
            {
                if (viewId == NavigatorViewId.LOBBY) 
                {
                    return new NSLobby();
                } 
                else if (viewId == NavigatorViewId.FRIENDS)
                {
                    return new NSFriends();
                }
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER)
            {
                return new NSMultiplayer();
            }

            return null;
        }
    }
}

