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
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantChess 
{
    public partial class GameMediator : Mediator
    {
        // View injection
        [Inject] public GameView view { get; set; }

        // Dispatch signals
        [Inject] public ChessboardEventSignal chessboardEventSignal { get; set; }
        [Inject] public StopTimersSignal stopTimersSignal { get; set; }
        [Inject] public SaveGameSignal saveGameSignal { get; set; }
        [Inject] public PauseTimersSignal pauseTimersSignal { get; set; }
        [Inject] public ResumeTimersSignal resumeTimersSignal { get; set; }

        public override void OnRegister()
        {
            OnRegisterChessboard();
            OnRegisterClock();
            OnRegisterPromotions();
            OnRegisterResults();
            OnRegisterScore();
            OnRegisterMatchInfo();
            OnRegisterMenu();
            OnRegisterHint();
            OnRegisterUndo();
        }

        public override void OnRemove()
        {
            OnRemoveChessboard();
            OnRemoveClock();
            OnRemovePromotions();
            OnRemoveResults();
            OnRemoveScore();
            OnRemoveMenu();
            OnRemoveHint();
            OnRemoveUndo();
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.PLAY) 
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.PLAY)
            {
                stopTimersSignal.Dispatch();
                saveGameSignal.Dispatch();
                view.Hide();
            }
        }

        [ListensTo(typeof(GameAppEventSignal))]
        public void OnAppEventChessboard(AppEvent evt)
        {
            if (!view || !view.IsVisible())
            {
                return;
            }

            if (evt == AppEvent.PAUSED || evt == AppEvent.QUIT)
            {
                pauseTimersSignal.Dispatch();   
                saveGameSignal.Dispatch();
            }
            else if (evt == AppEvent.RESUMED)
            {
                resumeTimersSignal.Dispatch();
            }
        }
    }
}
