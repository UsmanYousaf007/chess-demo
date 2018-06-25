/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-06 22:01:56 UTC+05:00
/// 
/// @description
/// [add_description_here]
using TurboLabz.InstantFramework;

namespace TurboLabz.Multiplayer
{
    public partial class GameMediator
    {
        public void OnRegisterDraw()
        {
            view.InitDraw();
            view.drawClaimedSignal.AddListener(OnDrawClaimed);
            view.drawRejectedSignal.AddListener(OnDrawRejected);
        }

        public void OnRemoveDraw()
        {
            view.CleanupDraw();
            view.drawClaimedSignal.RemoveListener(OnDrawClaimed);
            view.drawRejectedSignal.RemoveListener(OnDrawRejected);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowFiftyMoveDrawDialog(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.MULTIPLAYER_FIFTY_MOVE_DRAW_DLG) 
            {
                view.ShowFiftyMoveDraw();
            }
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowThreeFoldRepeatDrawDialog(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.MULTIPLAYER_THREEFOLD_REPEAT_DRAW_DLG)
            {
                view.ShowThreefoldRepeatDraw();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideDrawDialog(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.MULTIPLAYER_FIFTY_MOVE_DRAW_DLG ||
                viewId == NavigatorViewId.MULTIPLAYER_THREEFOLD_REPEAT_DRAW_DLG) 
            {
                view.HideDrawDialog();
            }
        }

        private void OnDrawClaimed()
        {
            chessboardEventSignal.Dispatch(ChessboardEvent.DRAW_CLAIMED);

        }

        private void OnDrawRejected()
        {
            chessboardEventSignal.Dispatch(ChessboardEvent.DRAW_REJECTED);
        }
    }
}
