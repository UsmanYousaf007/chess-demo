/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class NSLobby : NS
    {
        DateTime timeAtScreenShown;

        public override void RenderDisplayOnEnter()
        {
            timeAtScreenShown = TimeUtil.ToDateTime(cmd.backendService.serverClock.currentTimestamp);
            ShowView(NavigatorViewId.LOBBY);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            cmd.preferencesModel.timeSpentLobby += (float)(TimeUtil.ToDateTime(cmd.backendService.serverClock.currentTimestamp) - timeAtScreenShown).TotalMinutes;

            if (evt == NavigatorEvent.SHOW_CPU)
            {
                return new NSCPU();
            }
            else if (evt == NavigatorEvent.SHOW_STATS)
            {
                return new NSStats();
            }
			else if (evt == NavigatorEvent.SHOW_STORE)
			{
				return new NSStore();
			}
            else if (evt == NavigatorEvent.SHOW_FRIENDS)
            {
                return new NSFriends();
            }
            else if (evt == NavigatorEvent.SHOW_PROFILE_DLG)
            {
                return new NSProfileDlg();
            }
            else if (evt == NavigatorEvent.ESCAPE)
			{
				cmd.androidNativeService.SendToBackground();
			}
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER)
            {
                return new NSMultiplayer();
            }
            else if (evt == NavigatorEvent.SHOW_RATE_APP_DLG)
            {
                return new NSRateAppDlg();
            }
            else if (evt == NavigatorEvent.SHOW_SPOT_PURCHASE)
            {
                return new NSSpotPurchaseDlg();
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

