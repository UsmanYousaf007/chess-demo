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
    public class NSMultiplayer : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.MULTIPLAYER);
            cmd.analyticsService.ScreenVisit(NavigatorViewId.MULTIPLAYER);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.SHOW_MULTIPLAYER_EXIT_DLG ||
                evt == NavigatorEvent.ESCAPE)
            {
                if (cmd.multiplayerChessboardModel.inPlaybackMode)
                {
                    return new NSMultiplayerResultsDlg();
                }
                else
                {
                    return new NSMultiplayerExitDlg();
                }
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER_RESULTS_DLG)
            {
                return new NSMultiplayerResultsDlg();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER_PROMO_DLG)
            {
                return new NSMultiplayerPromo();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER_FIND_DLG)
            {
                return new NSMultiplayerFindDlg();
            }

            return null;
        }
    }
}

