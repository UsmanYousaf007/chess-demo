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
            stepBackwardButton.interactable = true;
            stepForwardButton.interactable = true;
        }

        public void StepBackwardButtonClicked()
        {
            stepBackwardClickedSignal.Dispatch();
        }

        public void StepForwardButtonClicked()
        {
            stepForwardClickedSignal.Dispatch();
        }

        public void ToggleStepBackward(bool enable)
        {
            stepBackwardButton.interactable = enable;
        }

        public void ToggleStepForward(bool enable)
        {
            stepForwardButton.interactable = enable;
        }

        void EnableStepButtons()
        {
            stepBackwardButton.interactable = backwardButtonPreviousState;
            stepForwardButton.interactable = forwardButtonPreviousState;
        }

        void DisableStepButtons()
        {
            backwardButtonPreviousState = stepBackwardButton.interactable;
            forwardButtonPreviousState = stepForwardButton.interactable;
            ToggleStepBackward(false);
            ToggleStepForward(false);
        }
    }
}
