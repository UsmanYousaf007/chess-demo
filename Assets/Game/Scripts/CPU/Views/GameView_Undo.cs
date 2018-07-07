/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-06 17:36:57 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using DG.Tweening;
using strange.extensions.signal.impl;
using TurboLabz.Chess;
using TurboLabz.TLUtils;
using UnityEngine.UI;
using TurboLabz.InstantFramework;

namespace TurboLabz.CPU
{
    public partial class GameView
    {
        public Button undoButton;
        public Text undoButtonLabel;

        public Signal undoMoveClickedSignal = new Signal();

        public void InitUndo()
        {
            undoButtonLabel.text = localizationService.Get(LocalizationKey.CPU_GAME_UNDO_BUTTON);
            undoButton.onClick.AddListener(UndoButtonClicked);
        }

        public void UpdateUndoButton(bool isPlayerTurn, int totalMoveCount)
        {
            if (isPlayerTurn && totalMoveCount >= 2)
            {
                EnableUndoButton();
            }
            else
            {
                DisableUndoButton();
            }
        }

        public void DisableUndoButton()
        {
            undoButton.interactable = false;
            DisableLabel(undoButtonLabel);
        }

        public bool IsUndoButtonActive()
        {
            return undoButton.interactable;
        }

        private void UndoButtonClicked()
        {
            ResetCapturedPieces();
            HideHint();
//            ClearNotation();
            EmptyScores();
            DisableUndoButton();
            undoMoveClickedSignal.Dispatch();
        }

        private void EnableUndoButton()
        {
            undoButton.interactable = true;
            EnableLabel(undoButtonLabel);
        }
    }
}
