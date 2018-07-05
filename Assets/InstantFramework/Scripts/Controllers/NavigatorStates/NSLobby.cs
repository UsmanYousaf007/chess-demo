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
    public class NSLobby : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.LOBBY);
            cmd.analyticsService.ScreenVisit(NavigatorViewId.LOBBY);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
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
			else if (evt == NavigatorEvent.SHOW_BUCK_PACKS_DLG)
			{
				return new NSBuckPacksDlg();
			}	
			else if (evt == NavigatorEvent.ESCAPE)
			{
				cmd.androidNativeService.SendToBackground();
			}
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER)
            {
                return new NSMultiplayer();
            }

            return null;
        }
    }
}

