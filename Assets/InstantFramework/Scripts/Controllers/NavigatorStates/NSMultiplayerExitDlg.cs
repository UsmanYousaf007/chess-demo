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
    public class NSMultiplayerExitDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.MULTIPLAYER_EXIT_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.SHOW_MULTIPLAYER ||
                evt == NavigatorEvent.ESCAPE)
            {
                return new NSMultiplayer();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER_RESULTS_DLG)
            {
                cmd.showViewSignal.Dispatch(NavigatorViewId.MULTIPLAYER);
                return new NSMultiplayerResultsDlg();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER_FIFTY_MOVE_DRAW_DLG)
            {
                cmd.showViewSignal.Dispatch(NavigatorViewId.MULTIPLAYER);
                return new NSMultiplayerFiftyMoveDrawDlg();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER_THREEFOLD_REPEAT_DRAW_DLG)
            {
                cmd.showViewSignal.Dispatch(NavigatorViewId.MULTIPLAYER);
                return new NSMultiplayerThreeFoldRepeatDrawDlg();
            }

            return null;
        }
    }
}

