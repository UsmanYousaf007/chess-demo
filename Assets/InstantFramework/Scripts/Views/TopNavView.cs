/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


using UnityEngine.UI;

using strange.extensions.mediation.impl;
using UnityEngine;
using System.Collections;
using TurboLabz.TLUtils;
using strange.extensions.signal.impl;
using TurboLabz.InstantGame;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public class TopNavView : View
    {
        // Services
        [Inject] public IAudioService audioService { get; set; }

        public Button supportButton;
        public Button settingsButton;
        public Button addGemsButton;
        public Text gemsCount;

        public Signal settingsButtonClickedSignal = new Signal();
        public Signal supportButtonClicked = new Signal();
        public Signal addGemsButtonClickedSignal = new Signal();

        public void Init()
        {
            supportButton.onClick.AddListener(OnSupportButtonClicked);
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            addGemsButton.onClick.AddListener(OnAddGemsButtonClicked);
        }

        private void OnSupportButtonClicked()
        {
            audioService.PlayStandardClick();
            supportButtonClicked.Dispatch();
        }

        private void OnSettingsButtonClicked()
        {
            audioService.PlayStandardClick();
            settingsButtonClickedSignal.Dispatch();
        }

        private void OnAddGemsButtonClicked()
        {
            audioService.PlayStandardClick();
            addGemsButtonClickedSignal.Dispatch();
        }

        public void UpdateGemsCount(long gems)
        {
            gemsCount.text = gems.ToString();
        }
    }
}