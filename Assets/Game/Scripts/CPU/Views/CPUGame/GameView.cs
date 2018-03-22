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

using UnityEngine;

using strange.extensions.mediation.impl;

using TurboLabz.Gamebet;
using TurboLabz.Common;
using UnityEngine.UI;
using TurboLabz.Chess;

namespace TurboLabz.CPUChess
{
    public partial class GameView : View
    {
        // TODO: Remove this injection, views cannot inject services or models or
        // anything for that matter.
        [Inject] public ILocalizationService localizationService { get; set; }

        public Camera chessboardCamera;
        public GameObject uiBlocker;
        public GameObject chessboardBlocker;
        public SpriteCache spriteCache;

        private Color yellowText = new Color(0.98f, 0.86f, 0.03f);
        private Color redText = new Color(0.91f, 0.29f, 0.24f);
        private Color greenText = new Color(0.15f, 0.53f, 0.33f);

        private bool undoButtonWasActive;
        private bool hintButtonWasActive;
        private bool menuButtonWasActive;

        public void Show()
        {
            gameObject.SetActive(true);
            OnParentShowResults();
            OnParentShowPromotions();
            OnParentShowCapturedPieces();
            OnParentShowScore();
            OnParentShowClock();
            OnParentShowNotation();
            OnParentShowMatchInfo();
            OnParentShowMenu();
            OnParentShowHint();
        }

        public void Hide()
        { 
            gameObject.SetActive(false);
            uiBlocker.SetActive(false);
            chessboardBlocker.SetActive(false);
            OnParentHideChessboard();
            OnParentHideClickAndDrag();
        }

        public bool IsVisible()
        {
            return gameObject.activeSelf;
        }

        private void OnApplicationFocus(bool focus)
        {
            OnApplicationFocusClickAndDrag(focus);
        }

        private void EnableModalBlocker()
        {
            uiBlocker.SetActive(true);
            chessboardBlocker.SetActive(true);
        }

        private void DisableModalBlocker()
        {
            uiBlocker.SetActive(false);
            chessboardBlocker.SetActive(false);
        }

        private void DisableLabel(Text label)
        {
            SetLabelAlpha(label, Colors.DISABLED_TEXT_ALPHA);
        }

        private void EnableLabel(Text label)
        {
            SetLabelAlpha(label, Colors.ENABLED_TEXT_ALPHA);
        }

        private void SetLabelAlpha(Text label, float alpha)
        {
            Color oldColor = label.color;
            label.color = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
        }

        private void StashUndoButton()
        {
            if (IsUndoButtonActive())
            {
                undoButtonWasActive = true;
            }

            DisableUndoButton();
        }

        private void StashHintButton()
        {
            if (IsHintButtonActive())
            {
                hintButtonWasActive = true;
            }

            DisableHintButton();
        }

        private void StashMenuButton()
        {
            if (IsMenuButtonActive())
            {
                menuButtonWasActive = true;
            }

            DisableMenuButton();
        }

        private void PopUndoButton()
        {
            if (undoButtonWasActive)
            {
                EnableUndoButton();
            }
        }

        private void PopHintButton()
        {
            if (hintButtonWasActive)
            {
                EnableHintButton();
            }
        }

        private void PopMenuButton()
        {
            if (menuButtonWasActive)
            {
                EnableMenuButton();
            }
        }
    }
}
