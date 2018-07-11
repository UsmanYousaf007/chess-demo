/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-19 18:54:42 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine.UI;

using strange.extensions.mediation.impl;
using UnityEngine;
using System.Collections;
using TurboLabz.TLUtils;
using strange.extensions.signal.impl;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    public class BottomNavView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }

        public enum ButtonId
        {
            Home,
            Profile,
            Shop,
            Settings
        }

        public ButtonId buttonId;

        public Text homeLabel;
        public Text profileLabel;
        public Text shopLabel;
        public Text settingsLabel;

        public Image homeIcon;
        public Image profileIcon;
        public Image shopIcon;
        public Image settingsIcon;

        public Button homeButton;
        public Button profileButton;
        public Button shopButton;
        public Button settingsButton;

        public Signal homeButtonClickedSignal = new Signal();
        public Signal profileButtonClickedSignal = new Signal();
        public Signal shopButtonClickedSignal = new Signal();
        public Signal settingsButtonClickedSignal = new Signal();

        public void Init()
        {
            homeLabel.text = localizationService.Get(LocalizationKey.NAV_HOME);
            profileLabel.text = localizationService.Get(LocalizationKey.NAV_PROFILE);
            shopLabel.text = localizationService.Get(LocalizationKey.NAV_SHOP);
            settingsLabel.text = localizationService.Get(LocalizationKey.NAV_SETTINGS);

            homeButton.onClick.AddListener(HomeButtonClicked);
            profileButton.onClick.AddListener(ProfileButtonClicked);
            shopButton.onClick.AddListener(ShopButtonClicked);
            settingsButton.onClick.AddListener(SettingsButtonClicked);

            UpdateButtons();
        }

        void UpdateButtons()
        {
            homeButton.interactable = true;
            homeIcon.color = Colors.WHITE;
            homeLabel.color = Colors.WHITE;

            profileButton.interactable = true;
            profileIcon.color = Colors.WHITE;
            profileLabel.color = Colors.WHITE;

            shopButton.interactable = true;
            shopIcon.color = Colors.WHITE;
            shopLabel.color = Colors.WHITE;

            settingsButton.interactable = true;
            settingsIcon.color = Colors.WHITE;
            settingsLabel.color = Colors.WHITE;

            if (buttonId == ButtonId.Home)
            {
                homeButton.interactable = false;
                homeIcon.color = Colors.YELLOW;
                homeLabel.color = Colors.YELLOW;
            }
            else if (buttonId == ButtonId.Profile)
            {
                profileButton.interactable = false;
                profileIcon.color = Colors.YELLOW;
                profileLabel.color = Colors.YELLOW;
            }
            else if (buttonId == ButtonId.Shop)
            {
                shopButton.interactable = false;
                shopIcon.color = Colors.YELLOW;
                shopLabel.color = Colors.YELLOW;

            }
            else if (buttonId == ButtonId.Settings)
            {
                settingsButton.interactable = false;
                settingsIcon.color = Colors.YELLOW;
                settingsLabel.color = Colors.YELLOW;
            }
        }

        void HomeButtonClicked()
        {
            homeButtonClickedSignal.Dispatch();
        }

        void ProfileButtonClicked()
        {
            profileButtonClickedSignal.Dispatch();
        }

        void ShopButtonClicked()
        {
            shopButtonClickedSignal.Dispatch();
        }

        void SettingsButtonClicked()
        {
            settingsButtonClickedSignal.Dispatch();
        }
    }
}
