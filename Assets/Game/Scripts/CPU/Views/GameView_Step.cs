/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


using UnityEngine;
using UnityEngine.UI;

using strange.extensions.signal.impl;
using TurboLabz.Chess;
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;
using TMPro;
using TurboLabz.InstantGame;

namespace TurboLabz.CPU
{
    public partial class GameView
    {
        public Signal stepBackwardClickedSignal = new Signal();
        public Signal stepForwardClickedSignal = new Signal();

        [Header("Step System")]
        public Button stepBackwardButton;
        public Button stepForwardButton;
        public Sprite powerplayAciveStepSprite;
        public Sprite powerPlayInActiveStepSprite;
        public GameObject stepTooltip;

        private bool backwardButtonPreviousState;
        private bool forwardButtonPreviousState;

        public void InitStep()
        {
            //hintButtonLabel.text = localizationService.Get(LocalizationKey.CPU_GAME_HINT_BUTTON);
            stepBackwardButton.onClick.AddListener(StepBackwardButtonClicked);
            stepForwardButton.onClick.AddListener(StepForwardButtonClicked);
        }

        public void OnParentShowStep()
        {
            ToggleStepBackward(false);
            ToggleStepForward(false);
        }

        public void StepBackwardButtonClicked()
        {
            if (isPowerModeOn)
            {
                //HideHint();
                //HideHindsight();
                cancelHintSingal.Dispatch();
                stepBackwardClickedSignal.Dispatch();
                audioService.Play(audioService.sounds.SFX_PLACE_PIECE);
            }
            else
            {
                audioService.PlayStandardClick();
                showPowerplayDlgButonSignal.Dispatch();
            }
        }

        public void StepForwardButtonClicked()
        {
            if (isPowerModeOn)
            {
                //HideHint();
                //HideHindsight();
                cancelHintSingal.Dispatch();
                stepForwardClickedSignal.Dispatch();
                audioService.Play(audioService.sounds.SFX_PLACE_PIECE);
            }
            else
            {
                audioService.PlayStandardClick();
                showPowerplayDlgButonSignal.Dispatch();
            }
        }

        public void ToggleStepBackward(bool enable, bool stash = false)
        {
            stepBackwardButton.interactable = enable;

            if (!stash)
                backwardButtonPreviousState = stepBackwardButton.interactable;
        }

        public void ToggleStepForward(bool enable, bool stash = false)
        {
            stepForwardButton.interactable = enable;

            if (!stash)
                forwardButtonPreviousState = stepForwardButton.interactable;
        }

        void RestoreStepButtons()
        {
            ToggleStepBackward(backwardButtonPreviousState);
            ToggleStepForward(forwardButtonPreviousState);
        }

        void StashStepButtons()
        {
            backwardButtonPreviousState = stepBackwardButton.interactable;
            forwardButtonPreviousState = stepForwardButton.interactable;

            ToggleStepBackward(false, true);
            ToggleStepForward(false, true);
        }

        public void SetupStepButtons()
        {
            var sprieToShow = isPowerModeOn ? powerplayAciveStepSprite : powerPlayInActiveStepSprite;
            stepBackwardButton.image.sprite = sprieToShow;
            stepForwardButton.image.sprite = sprieToShow;
        }
    }
}
