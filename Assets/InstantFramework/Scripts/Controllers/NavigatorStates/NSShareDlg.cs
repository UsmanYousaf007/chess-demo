using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class NSShareDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.SHARE_SCREEN_DIALOG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            NavigatorViewId viewId = CameFrom(NavigatorViewId.MULTIPLAYER, NavigatorViewId.CPU);

            if (evt == NavigatorEvent.ESCAPE)
            {
                if (viewId == NavigatorViewId.CPU)
                {
                    return new NSCPU();
                }
                else if (viewId == NavigatorViewId.MULTIPLAYER)
                {
                    return new NSMultiplayer();
                }
            }
            if (evt == NavigatorEvent.SHOW_MULTIPLAYER)
            {
                return new NSMultiplayer();
            }
            else if (evt == NavigatorEvent.SHOW_MULTIPLAYER_RESULTS_DLG)
            {
                cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
                return new NSMultiplayerResultsDlg();
            }

            return null;
        }
    }
}

