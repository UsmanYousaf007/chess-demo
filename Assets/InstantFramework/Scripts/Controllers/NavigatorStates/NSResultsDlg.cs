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
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class NSResultsDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.RESULTS_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.ESCAPE)
            {
                cmd.loadGameSignal.Dispatch(); 
                return null;
            }
            else if (evt == NavigatorEvent.SHOW_MENU)
            {
                return new NSMenu();
            }
            else if (evt == NavigatorEvent.SHOW_STATS)
            {
                return new NSStats();
            }

            return null;
        }
    }
}

