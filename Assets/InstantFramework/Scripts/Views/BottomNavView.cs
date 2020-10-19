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
using DG.Tweening;

namespace TurboLabz.InstantFramework
{
    public class BottomNavView : View
    {
        public enum ButtonId
        {
            Home,
            Shop,
            Friends,
            Inventory,
            Arena
        }

        public ButtonId buttonId;

        public Image selectedImage;

        public Text homeLabel;
        public Text friendsLabel;
        public Text inventoryLabel;
        public Text shopLabel;
        public Text arenaLabel;

        public GameObject homeSelected;
        public GameObject friendsSelected;
        public GameObject inventorySelected;
        public GameObject shopSelected;
        public GameObject arenaSelected;

        public Button homeButton;
        public Button friendsButton;
        public Button inventoryButton;
        public Button shopButton;
        public Button arenaButton;

        public Image homeIcon;
        public Image friendsIcon;
        public Image inventoryIcon;
        public Image shopIcon;
        public Image arenaIcon;

        public GameObject shopAlert;
        public GameObject inventoryAlert;

        public RectTransform selectedRT;

        public Signal homeButtonClickedSignal = new Signal();
        public Signal friendsButtonClickedSignal = new Signal();
        public Signal inventoryButtonClickedSignal = new Signal();
        public Signal shopButtonClickedSignal = new Signal();
        public Signal arenaButtonClickedSignal = new Signal();

        //Services
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public ILocalizationService localizationService { get; set; }

        //Models
        [Inject] public IPreferencesModel preferencesModel { get; set; }

        bool onStart;

        public void Init()
        {
            homeLabel.text = localizationService.Get(LocalizationKey.NAV_HOME);
            friendsLabel.text = localizationService.Get(LocalizationKey.NAV_FRIENDS);
            inventoryLabel.text = localizationService.Get(LocalizationKey.NAV_INVENTORY);
            shopLabel.text = localizationService.Get(LocalizationKey.NAV_SHOP);
            arenaLabel.text = localizationService.Get(LocalizationKey.NAV_ARENA);

            homeButton.onClick.AddListener(HomeButtonClicked);
            friendsButton.onClick.AddListener(FriendsButtonClicked);
            inventoryButton.onClick.AddListener(InventoryButtonClicked);
            shopButton.onClick.AddListener(ShopButtonClicked);
            arenaButton.onClick.AddListener(ArenaButtonClicked);
            UpdateButtons();
        }

        IEnumerator WaitUntilEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            selectedImage.rectTransform.sizeDelta = new Vector2(homeButton.GetComponent<RectTransform>().rect.width, homeButton.GetComponent<RectTransform>().rect.height);
            onStart = true;
            //UpdateButtons();
        }

        private void SetSelectedImageStartingPos()
        {
            selectedImage.enabled = true;
        }

        private void OnEnable()
        {
            //if (!onStart)
            //{
            //    StartCoroutine(WaitUntilEndOfFrame());
            //}
            UpdateAlerts();
            //UpdateButtons();
        }

        public void UpdateAlerts()
        {
            shopAlert.SetActive(!preferencesModel.shopTabVisited);
            inventoryAlert.SetActive(!preferencesModel.inventoryTabVisited);
        }

        public void UpdateButtonID(ButtonId btnID)
        {
            buttonId = btnID;
        }

        public void UpdateButtons()
        {
            //if (!onStart)
            //    return;

            homeButton.interactable = true;
            homeSelected.SetActive(false);
            homeLabel.enabled = true;
            homeLabel.color = Colors.WHITE_150;
            homeIcon.gameObject.transform.localScale = new Vector3(0.87f, 0.87f, 0.87f);

            friendsButton.interactable = true;
            friendsSelected.SetActive(false);
            friendsLabel.enabled = true;
            friendsLabel.color = Colors.WHITE_150;
            friendsIcon.gameObject.transform.localScale = new Vector3(0.87f, 0.87f, 0.87f);

            inventoryButton.interactable = true;
            inventorySelected.SetActive(false);
            inventoryLabel.enabled = true;
            inventoryLabel.color = Colors.WHITE_150;
            inventoryIcon.gameObject.transform.localScale = new Vector3(0.87f, 0.87f, 0.87f);

            shopButton.interactable = true;
            shopSelected.SetActive(false);
            shopLabel.enabled = true;
            shopLabel.color = Colors.WHITE_150;
            shopIcon.gameObject.transform.localScale = new Vector3(0.87f, 0.87f, 0.87f);

            arenaButton.interactable = true;
            arenaSelected.SetActive(false);
            arenaLabel.enabled = true;
            arenaLabel.color = Colors.WHITE_150;
            arenaIcon.gameObject.transform.localScale = new Vector3(0.87f, 0.87f, 0.87f);

            if (buttonId == ButtonId.Home)
            {
                homeButton.interactable = false;
                homeSelected.SetActive(true);
                //homeLabel.enabled = false;
                homeLabel.color = Colors.YELLOW;
                homeIcon.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
                //iTween.MoveTo(selectedImage.gameObject,
                //iTween.Hash("position", homeSelected.transform.position, "time", 0.4f, "oncomplete", "SetSelectedImageStartingPos", "oncompletetarget", this.gameObject));
            }
            else if (buttonId == ButtonId.Friends)
            {
                friendsButton.interactable = false;
                friendsSelected.SetActive(true);
                //friendsLabel.enabled = false;
                friendsLabel.color = Colors.YELLOW;
                friendsIcon.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
                //iTween.MoveTo(selectedImage.gameObject,
                //iTween.Hash("position", friendsSelected.transform.position, "time", 0.4f));
            }
            else if (buttonId == ButtonId.Inventory)
            {
                inventoryButton.interactable = false;
                inventorySelected.SetActive(true);
                //inventoryLabel.enabled = false;
                inventoryLabel.color = Colors.YELLOW;
                inventoryIcon.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
                //iTween.MoveTo(selectedImage.gameObject,
                //iTween.Hash("position", inventorySelected.transform.position, "time", 0.4f));
            }
            else if (buttonId == ButtonId.Shop)
            {
                shopButton.interactable = false;
                shopSelected.SetActive(true);
                //shopLabel.enabled = false;
                shopLabel.color = Colors.YELLOW;
                shopIcon.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
                //iTween.MoveTo(selectedImage.gameObject,
                //iTween.Hash("position", shopSelected.transform.position, "time", 0.4f));
            }
            else if (buttonId == ButtonId.Arena)
            {
                arenaButton.interactable = false;
                arenaSelected.SetActive(true);
                //arenaLabel.enabled = false;
                arenaLabel.color = Colors.YELLOW;
                arenaIcon.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
                //iTween.MoveTo(selectedImage.gameObject,
                //iTween.Hash("position", arenaSelected.transform.position, "time", 0.4f));
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

        void ArenaButtonClicked()
        {
            audioService.PlayStandardClick();
            arenaButtonClickedSignal.Dispatch();
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

        public void Show(bool value)
        {
            gameObject.SetActive(value);
        }
    }
}
