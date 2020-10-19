/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-12-14 13:37:03 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.InstantFramework
{
    public class NSInviteDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.INVITE_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.ESCAPE)
            {
                NavigatorViewId viewId = CameFrom(NavigatorViewId.FRIENDS, NavigatorViewId.LOBBY);

                if (viewId == NavigatorViewId.FRIENDS)
                {
                    return new NSFriends();
                }
                if (viewId == NavigatorViewId.LOBBY)
                {
                    return new NSLobby();
                }
            }

            return null;
        }
    }
}
