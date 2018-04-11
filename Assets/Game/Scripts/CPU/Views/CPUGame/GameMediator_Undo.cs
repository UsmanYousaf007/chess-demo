/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-06 22:01:38 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections.Generic;
using TurboLabz.Chess;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantChess
{
    public partial class GameMediator
    {
        [Inject] public UndoMoveSignal undoMoveSignal { get; set; }

        public void OnRegisterUndo()
        {
            view.InitUndo();
            view.undoMoveClickedSignal.AddListener(OnUndoMove);
        }

        [ListensTo(typeof(UpdateUndoButtonSignal))]
        public void OnUpdateUndoButton(bool isPlayerTurn, int totalMoveCount)
        {
            view.UpdateUndoButton(isPlayerTurn, totalMoveCount);
        }

        [ListensTo(typeof(DisableUndoButtonSignal))]
        public void OnDisableUndoButton()
        {
            view.DisableUndoButton();
        }

        public void OnRemoveUndo()
        {
            view.undoMoveClickedSignal.RemoveAllListeners();
        }

        private void OnUndoMove()
        {
            undoMoveSignal.Dispatch();
        }
    }
}
