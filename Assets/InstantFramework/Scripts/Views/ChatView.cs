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

namespace TurboLabz.InstantFramework
{
    public partial class ChatView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }

        public Button backToGameButton;

        public Signal backToGameButtonClickedSignal = new Signal();

        public void Init()
        {
            backToGameButton.onClick.AddListener(OnBackToGameButtonClicked);
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
    }
}
