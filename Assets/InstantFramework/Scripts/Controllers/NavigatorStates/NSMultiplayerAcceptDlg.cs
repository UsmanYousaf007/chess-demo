/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

namespace TurboLabz.InstantFramework
{
    public class NSMultiplayerAcceptDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.MULTIPLAYER_ACCEPT_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.SHOW_MULTIPLAYER)
            {
                return new NSMultiplayer();
            }
            else if (evt == NavigatorEvent.SHOW_FRIENDS)
            {
                return new NSFriends();
            }

            return null;
        }
    }
}

