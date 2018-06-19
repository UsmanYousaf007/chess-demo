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
    public class NSCPU : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.CPU);
            cmd.analyticsService.ScreenVisit(NavigatorViewId.CPU);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.SHOW_CPU_EXIT_DLG ||
                evt == NavigatorEvent.ESCAPE)
            {
                if (cmd.cpuChessboardModel.inPlaybackMode)
                {
                    return new NSCPUResultsDlg();
                }
                else
                {
                    return new NSCPUExitDlg();
                }
            }
            else if (evt == NavigatorEvent.SHOW_CPU_RESULTS_DLG)
            {
                return new NSCPUResultsDlg();
            }
            else if (evt == NavigatorEvent.SHOW_CPU_PROMO_DLG)
            {
                return new NSCPUPromo();
            }

            return null;
        }
    }
}

