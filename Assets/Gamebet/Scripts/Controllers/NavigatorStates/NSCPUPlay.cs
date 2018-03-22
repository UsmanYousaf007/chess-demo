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

namespace TurboLabz.Gamebet
{
    public class NSCPUPlay : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.CPU_PLAY);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.SHOW_CPU_PLAY_EXIT_DLG ||
                evt == NavigatorEvent.ESCAPE)
            {
                return new NSCPUPlayExitDialog();
            }
            else if (evt == NavigatorEvent.SHOW_CPU_PLAY_RESULTS_DLG)
            {
                return new NSCPUPlayResultsDialog();
            }
            else if (evt == NavigatorEvent.SHOW_PROMO_DIALOG)
            {
                return new NSCPUPlayPromo();
            }

            return null;
        }
    }
}

