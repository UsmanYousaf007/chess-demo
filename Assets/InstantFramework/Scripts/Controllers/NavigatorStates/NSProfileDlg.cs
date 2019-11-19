/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

namespace TurboLabz.InstantFramework
{
    public class NSProfileDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.PROFILE_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            NavigatorViewId viewId = CameFrom(NavigatorViewId.MULTIPLAYER, NavigatorViewId.FRIENDS, NavigatorViewId.LOBBY);

            if (evt == NavigatorEvent.ESCAPE)
            {
                if (viewId == NavigatorViewId.FRIENDS)
                {
                    return new NSFriends();
                }
                else if (viewId == NavigatorViewId.LOBBY)
                {
                    return new NSLobby();
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

            return null;
        }
    }
}

