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
            Shop,
            Friends
        }

        public ButtonId buttonId;

        public Text homeLabel;
        public Text shopLabel;
        public Text friendsLabel;

        public Image homeIcon;
        public Image shopIcon;
        public Image friendsIcon;

        public Button homeButton;
        public Button shopButton;
        public Button friendsButton;

        public Signal homeButtonClickedSignal = new Signal();
        public Signal shopButtonClickedSignal = new Signal();
        public Signal friendsButtonClickedSignal = new Signal();

        public void Init()
        {
            homeLabel.text = localizationService.Get(LocalizationKey.NAV_HOME);
            shopLabel.text = localizationService.Get(LocalizationKey.NAV_SHOP);
            friendsLabel.text = localizationService.Get(LocalizationKey.NAV_FRIENDS);

            homeButton.onClick.AddListener(HomeButtonClicked);
            shopButton.onClick.AddListener(ShopButtonClicked);
            friendsButton.onClick.AddListener(SettingsButtonClicked);

            UpdateButtons();
        }

        void UpdateButtons()
        {
            homeButton.interactable = true;
            homeIcon.color = Colors.WHITE_150;
            homeLabel.color = Colors.WHITE_150;

            shopButton.interactable = true;
            shopIcon.color = Colors.WHITE_150;
            shopLabel.color = Colors.WHITE_150;

            friendsButton.interactable = true;
            friendsIcon.color = Colors.WHITE_150;
            friendsLabel.color = Colors.WHITE_150;

            if (buttonId == ButtonId.Home)
            {
                homeButton.interactable = false;
                homeIcon.color = Colors.YELLOW;
                homeLabel.color = Colors.YELLOW;
            }
            else if (buttonId == ButtonId.Shop)
            {
                shopButton.interactable = false;
                shopIcon.color = Colors.YELLOW;
                shopLabel.color = Colors.YELLOW;

            }
            else if (buttonId == ButtonId.Friends)
            {
                friendsButton.interactable = false;
                friendsIcon.color = Colors.YELLOW;
                friendsLabel.color = Colors.YELLOW;
            }
        }

        void HomeButtonClicked()
        {
            homeButtonClickedSignal.Dispatch();
        }

        void ShopButtonClicked()
        {
            shopButtonClickedSignal.Dispatch();
        }

        void SettingsButtonClicked()
        {
            friendsButtonClickedSignal.Dispatch();
        }
    }
}
