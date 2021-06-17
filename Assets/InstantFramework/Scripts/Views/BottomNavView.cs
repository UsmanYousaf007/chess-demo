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
using strange.extensions.signal.impl;
using DG.Tweening;


namespace TurboLabz.InstantFramework
{
    [System.CLSCompliant(false)]
    public class BottomNavView : View
    {
        public enum ButtonId
        {
            Home,
            Shop,
            Friends,
            Inventory,
            Arena,
            Lesson
        }

        public ButtonId buttonId;

        public Text homeLabel;
        public Text friendsLabel;
        public Text inventoryLabel;
        public Text shopLabel;

        public GameObject homeSelected;
        public GameObject friendsSelected;
        public GameObject inventorySelected;
        public GameObject shopSelected;

        public Button homeButton;
        public Button friendsButton;
        public Button inventoryButton;
        public Button shopButton;

        public Image homeIcon;
        public Image friendsIcon;
        public Image inventoryIcon;
        public Image shopIcon;

        public GameObject shopAlert;
        public GameObject inventoryAlert;

        public GameObject saleRibbon;

        public Signal homeButtonClickedSignal = new Signal();
        public Signal friendsButtonClickedSignal = new Signal();
        public Signal inventoryButtonClickedSignal = new Signal();
        public Signal shopButtonClickedSignal = new Signal();

        //Services
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public ILocalizationService localizationService { get; set; }

        //Models
        [Inject] public IPreferencesModel preferencesModel { get; set; }

        private Sequence freeTagAnimationSequence;

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
            //ShowSale(false);
        }

        private void OnEnable()
        {
            UpdateAlerts();
            SaleTagAnimation(true);
        }

        private void OnDisable()
        {
            SaleTagAnimation(false);
        }

        public void UpdateAlerts()
        {
            //shopAlert.SetActive(!preferencesModel.shopTabVisited);
            inventoryAlert.SetActive(!preferencesModel.inventoryTabVisited);
        }

        public void UpdateButtonID(ButtonId btnID)
        {
            buttonId = btnID;
        }

        public void UpdateButtons()
        {
            homeButton.interactable = true;
            homeSelected.SetActive(false);
            homeIcon.enabled = true;

            friendsButton.interactable = true;
            friendsSelected.SetActive(false);
            friendsIcon.enabled = true;

            inventoryButton.interactable = true;
            inventorySelected.SetActive(false);
            inventoryIcon.enabled = true;

            shopButton.interactable = true;
            shopSelected.SetActive(false);
            shopIcon.enabled = true;

            if (buttonId == ButtonId.Home)
            {
                homeButton.interactable = false;
                homeSelected.SetActive(true);
                homeIcon.enabled = false;
            }
            else if (buttonId == ButtonId.Friends)
            {
                friendsButton.interactable = false;
                friendsSelected.SetActive(true);
                friendsIcon.enabled = false;
            }
            else if (buttonId == ButtonId.Inventory)
            {
                inventoryButton.interactable = false;
                inventorySelected.SetActive(true);
                inventoryIcon.enabled = false;
            }
            else if (buttonId == ButtonId.Shop)
            {
                shopButton.interactable = false;
                shopSelected.SetActive(true);
                shopIcon.enabled = false;
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

        public void Show(bool value)
        {
            gameObject.SetActive(value);
        }

        private void SaleTagAnimation(bool play)
        {
            if (freeTagAnimationSequence != null)
            {
                freeTagAnimationSequence.Kill();
                freeTagAnimationSequence = null;
            }

            if (play) { 
                freeTagAnimationSequence = DOTween.Sequence();
                freeTagAnimationSequence.AppendInterval(7f);
                freeTagAnimationSequence.AppendCallback(() => saleRibbon.transform.DOScale(1.4f, 0.2f));
                freeTagAnimationSequence.AppendCallback(() => saleRibbon.transform.DOLocalRotate(new Vector3(0, 0, 20), 0.04f, RotateMode.Fast));
                freeTagAnimationSequence.AppendInterval(0.04f);
                freeTagAnimationSequence.AppendCallback(() => saleRibbon.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.04f, RotateMode.Fast));
                freeTagAnimationSequence.AppendInterval(0.04f);
                freeTagAnimationSequence.AppendCallback(() => saleRibbon.transform.DOLocalRotate(new Vector3(0, 0, -20), 0.04f, RotateMode.Fast));
                freeTagAnimationSequence.AppendInterval(0.04f);
                freeTagAnimationSequence.AppendCallback(() => saleRibbon.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.04f, RotateMode.Fast));
                freeTagAnimationSequence.AppendInterval(0.04f);
                freeTagAnimationSequence.AppendCallback(() => saleRibbon.transform.DOLocalRotate(new Vector3(0, 0, 20), 0.04f, RotateMode.Fast));
                freeTagAnimationSequence.AppendInterval(0.04f);
                freeTagAnimationSequence.AppendCallback(() => saleRibbon.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.04f, RotateMode.Fast));
                freeTagAnimationSequence.AppendInterval(0.04f);
                freeTagAnimationSequence.AppendCallback(() => saleRibbon.transform.DOLocalRotate(new Vector3(0, 0, -20), 0.04f, RotateMode.Fast));
                freeTagAnimationSequence.AppendInterval(0.04f);
                freeTagAnimationSequence.AppendCallback(() => saleRibbon.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.04f, RotateMode.Fast));
                freeTagAnimationSequence.AppendCallback(() => saleRibbon.transform.DOScale(1.0f, 0.5f));
                freeTagAnimationSequence.AppendInterval(0.5f);
                freeTagAnimationSequence.SetLoops(-1);
                freeTagAnimationSequence.PlayForward();
            }
        }

        public void ShowSale(bool show)
        {
            saleRibbon.SetActive(show);
        }
    }
}
