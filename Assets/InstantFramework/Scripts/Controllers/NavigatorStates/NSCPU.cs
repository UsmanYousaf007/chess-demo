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

using System;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class NSCPU : NS
    {
        DateTime timeAtScreenShown;

        public override void RenderDisplayOnEnter()
        {
            timeAtScreenShown = TimeUtil.ToDateTime(cmd.backendService.serverClock.currentTimestamp);
            ShowView(NavigatorViewId.CPU);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            cmd.preferencesModel.UpdateTimeSpentAnalyticsData(AnalyticsEventId.time_spent_cpu_match, timeAtScreenShown);

            if (evt == NavigatorEvent.SHOW_CPU_EXIT_DLG ||
                evt == NavigatorEvent.ESCAPE)
            {
                return new NSCPUExitDlg();
            }
            else if (evt == NavigatorEvent.SHOW_CPU_RESULTS_DLG)
            {
                return new NSCPUResultsDlg();
            }
            else if (evt == NavigatorEvent.SHOW_CPU_PROMO_DLG)
            {
                return new NSCPUPromo();
            }
            else if (evt == NavigatorEvent.SHOW_CPU_SAFE_MOVE_DLG)
            {
                return new NSCPUSafeMoveDlg();
            }
            else if (evt == NavigatorEvent.SHOW_CPU_INFO_DLG)
            {
                return new NSCPUInfoDlg();
            }
            else if (evt == NavigatorEvent.SHOW_LOBBY)
            {
                return new NSLobby();
            }

            return null;
        }
    }
}

