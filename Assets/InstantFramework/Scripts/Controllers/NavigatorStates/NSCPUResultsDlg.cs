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
    public class NSCPUResultsDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.CPU_RESULTS_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.ESCAPE)
            {
                return null;
            }
            else if (evt == NavigatorEvent.SHOW_LOBBY)
            {
                return new NSLobby();
            }
            else if (evt == NavigatorEvent.SHOW_STATS)
            {
                return new NSStats();
            }
            else if (evt == NavigatorEvent.SHOW_CPU)
            {
                return new NSCPU();
            }
            else if (evt == NavigatorEvent.SHOW_HOME)
            {
                return new NSHome();
            }

            return null;
        }
    }
}

