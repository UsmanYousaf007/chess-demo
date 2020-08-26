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
        public enum ButtonId
        {
            Home,
            Shop,
            Friends,
            Inventory
        }

        public ButtonId buttonId;

        public Text homeLabel;
        public Text friendsLabel;
        public Text inventoryLabel;
        public Text shopLabel;

        public Image homeIcon;
        public Image friendsIcon;
        public Image inventoryIcon;
        public Image shopIcon;

        public Button homeButton;
        public Button friendsButton;
        public Button inventoryButton;
        public Button shopButton;

        public GameObject shopAlert;
        public GameObject inventoryAlert;

        public Signal homeButtonClickedSignal = new Signal();
        public Signal friendsButtonClickedSignal = new Signal();
        public Signal inventoryButtonClickedSignal = new Signal();
        public Signal shopButtonClickedSignal = new Signal();

        //Services
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public ILocalizationService localizationService { get; set; }

        //Models
        [Inject] public IPreferencesModel preferencesModel { get; set; }

        public void Init()
        {
            homeLabel.text = localizationService.Get(LocalizationKey.NAV_HOME);
            friendsLabel.text = localizationService.Get(LocalizationKey.NAV_FRIENDS);
            inventoryLabel.text = localizationService.Get(LocalizationKey.NAV_INVENTORY);
            shopLabel.text = localizationService.Get(LocalizationKey.NAV_SHOP);

            homeButton.onClick.AddListener(HomeButtonClicked);
            friendsButton.onClick.AddListener(FriendsButtonClicked);
            inventoryButton.onClick.AddListener(InventoryButtonClicked);
            shopButton.onClick.AddListener(ShopButtonClicked);

            UpdateButtons();
        }

        private void OnEnable()
        {
            UpdateAlerts();
        }

        public void UpdateAlerts()
        {
            shopAlert.SetActive(!preferencesModel.shopTabVisited);
            inventoryAlert.SetActive(!preferencesModel.inventoryTabVisited);
        }

        void UpdateButtons()
        {
            homeButton.interactable = true;
            homeIcon.color = Colors.WHITE_100;
            homeLabel.color = Colors.WHITE_100;

            friendsButton.interactable = true;
            friendsIcon.color = Colors.WHITE_100;
            friendsLabel.color = Colors.WHITE_100;

            inventoryButton.interactable = true;
            inventoryIcon.color = Colors.WHITE_100;
            inventoryLabel.color = Colors.WHITE_100;

            shopButton.interactable = true;
            shopIcon.color = Colors.WHITE_100;
            shopLabel.color = Colors.WHITE_100;

            if (buttonId == ButtonId.Home)
            {
                homeButton.interactable = false;
                homeIcon.color = Colors.YELLOW;
                homeLabel.color = Colors.YELLOW;
            }
            else if (buttonId == ButtonId.Friends)
            {
                friendsButton.interactable = false;
                friendsIcon.color = Colors.YELLOW;
                friendsLabel.color = Colors.YELLOW;
            }
            else if (buttonId == ButtonId.Inventory)
            {
                inventoryButton.interactable = false;
                inventoryIcon.color = Colors.YELLOW;
                inventoryLabel.color = Colors.YELLOW;
            }
            else if (buttonId == ButtonId.Shop)
            {
                shopButton.interactable = false;
                shopIcon.color = Colors.YELLOW;
                shopLabel.color = Colors.YELLOW;
            }
        }

        void HomeButtonClicked()
        {
            audioService.PlayStandardClick();
            homeButtonClickedSignal.Dispatch();
        }

        void FriendsButtonClicked()
        {
            audioService.PlayStandardClick();
            friendsButtonClickedSignal.Dispatch();
        }

        void InventoryButtonClicked()
        {
            audioService.PlayStandardClick();
            preferencesModel.inventoryTabVisited = true;
            inventoryButtonClickedSignal.Dispatch();
        }

        void ShopButtonClicked()
        {
            audioService.PlayStandardClick();
            preferencesModel.shopTabVisited = true;
            shopButtonClickedSignal.Dispatch();
        }
    }
}
