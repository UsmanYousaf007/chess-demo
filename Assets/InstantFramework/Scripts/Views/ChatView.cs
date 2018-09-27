/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine.UI;
using strange.extensions.mediation.impl;
using UnityEngine;
using TurboLabz.TLUtils;
using System.Collections.Generic;
using strange.extensions.signal.impl;
using System;
using TurboLabz.InstantGame;
using TMPro;

namespace TurboLabz.InstantFramework
{
    public partial class ChatView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }

        public Button backToGameButton;
        public Text backToGameLabel;
        public TMP_InputField chatInputField;

        public Signal backToGameButtonClickedSignal = new Signal();

        public void Init()
        {
            backToGameButton.onClick.AddListener(OnBackToGameButtonClicked);
            //chatInputField.on.onClick.AddListener(OnChatInputButtonClicked);

            backToGameLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_BACK_TO_GAME);
        }

        public void Show() 
        { 
            gameObject.SetActive(true); 
        }

        public void Hide()
        { 
            gameObject.SetActive(false); 
        }

        private void OnBackToGameButtonClicked()
        {
            backToGameButtonClickedSignal.Dispatch();
        }

        private void OnChatInputButtonClicked()
        {
            TouchScreenKeyboard keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Search, false, true);
            keyboard.text = "wazzup";

            TMPro.TMP_InputField field; 





        }
    }
}
