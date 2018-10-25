/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

namespace TurboLabz.InstantFramework
{
    public class NSFriends : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.FRIENDS);
            cmd.analyticsService.ScreenVisit(NavigatorViewId.FRIENDS);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.SHOW_MULTIPLAYER)
            {
                return new NSMultiplayer();
            }
            if (evt == NavigatorEvent.SHOW_PROFILE_DLG)
            {
                return new NSProfileDlg();
            }
            else if (evt == NavigatorEvent.ESCAPE)
            {
                return null;
            }
            else if (evt == NavigatorEvent.SHOW_LOBBY)
            {
                return new NSLobby();
            }
            else if (evt == NavigatorEvent.SHOW_STORE)
            {
                return new NSStore();
            }
            else if (evt == NavigatorEvent.SHOW_STATS)
            {
                return new NSStats();
            }
            else if (evt == NavigatorEvent.SHOW_BUCK_PACKS_DLG)
            {
                return new NSBuckPacksDlg();
            }
            else if (evt == NavigatorEvent.SHOW_RATE_APP_DLG)
            {
                return new NSRateAppDlg();
            }

            return null;
        }
    }
}

