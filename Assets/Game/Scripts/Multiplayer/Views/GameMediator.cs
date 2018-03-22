/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-06 17:45:03 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.mediation.impl;

using TurboLabz.Gamebet;
using TurboLabz.Chess;
using TurboLabz.Common;

namespace TurboLabz.MPChess 
{
    public partial class GameMediator : Mediator
    {
        // View injection
        [Inject] public GameView view { get; set; }

        // Dispatch signals
        [Inject] public ChessboardEventSignal chessboardEventSignal { get; set; }
        [Inject] public StopTimersSignal stopTimersSignal { get; set; }

        public override void OnRegister()
        {
            view.Init();

            OnRegisterChat();
            OnRegisterChessboard();
            OnRegisterDraw();
            OnRegisterClock();
            OnRegisterPromotions();
            OnRegisterResign();
            OnRegisterResults();
            OnRegisterScore();
            OnRegisterMatchInfo();
        }

        public override void OnRemove()
        {
            view.Cleanup();

            OnRemoveChat();
            OnRemoveChessboard();
            OnRemoveDraw();
            OnRemoveClock();
            OnRemovePromotions();
            OnRemoveResign();
            OnRemoveResults();
            OnRemoveScore();
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.MP_PLAY) 
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.MP_PLAY)
            {
                view.Hide();
            }
        }
    }
}
