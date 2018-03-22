/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-02-21 13:27:49 UTC+05:00

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public class ShopAvatarsBorderModalView : View
    {
        // TODO: Remove this injection, views cannot inject services or models
        [Inject] public ILocalizationService localizationService { get; set; }

        public SpriteCache spriteCache;
        public TextColorCache textColorCache;

        public Button closeButton;

        public Text titleLabel;
        public Text tierLabel;
        public Image tierLabelLeftSeperatorImage;
        public Image tierLabelRightSeperatorImage;

        public Image chessSkinImage;

        public Text enoughBucksDialogueLabel;
        public Button enoughBucksBuyButton;
        public Text enoughBucksBuyPriceLabel;

        public Text notEnoughBucksDialogueLabel;
        public Text notEnoughBucksPriceLabel;

        public Text confirmationDialogueLabel;
        public Button confirmationYesButton;
        public Text confirmationYesButtonLabel;
        public Button confirmationNoButton;
        public Text confirmationNoButtonLabel;

        public Text addToCollectionDialogueLabel;
        public Button addToCollectionButton;
        public Text addToCollectionButtonLabel;

        public Text viewCollectionDialogueLabel;
        public Button viewCollectionButton;
        public Text viewCollectionButtonLabel;

        public GameObject closeButtonObject;
        public GameObject enoughBucks;
        public GameObject notEnoughBucks;
        public GameObject confirmation;
        public GameObject addToCollection;
        public GameObject viewCollection;

        //view signals
        public Signal closeButtonClickedSignal = new Signal();
        public Signal<string> confirmPurchaseButtonClickedSignal = new Signal<string>();
        public Signal addToCollectionButtonClickedSignal = new Signal();
        public Signal viewCollectionButtonClickedSignal = new Signal();

        private AvatarBorderThumbsContainer avatarBorderThumbsContainer;
        private string activeShopItemId;

        public void Init()
        {
            avatarBorderThumbsContainer = AvatarBorderThumbsContainer.Load();

            closeButton.onClick.AddListener(OnCloseButtonClicked);
            enoughBucksBuyButton.onClick.AddListener(OnEnoughBucksBuyButtonClicked);
            confirmationYesButton.onClick.AddListener(OnConfirmationYesButtonClicked);
            confirmationNoButton.onClick.AddListener(OnConfirmationNoButtonClicked);
            addToCollectionButton.onClick.AddListener(OnAddToCollectionButtonClicked);
            viewCollectionButton.onClick.AddListener(OnViewCollectionButtonClicked);
        }

        void OnDisable()
        {
            enoughBucks.SetActive(false);
            notEnoughBucks.SetActive(false);
            confirmation.SetActive(false);
            addToCollection.SetActive(false);
            viewCollection.SetActive(false);
        }

        public void UpdateView(ShopVO vo)
        {
            closeButtonObject.SetActive(true);

            titleLabel.text = vo.shopSettings.avatarBorderShopItems[vo.activeShopItemId].displayName;
            tierLabel.text = vo.shopSettings.avatarBorderShopItems[vo.activeShopItemId].tier;
            tierLabel.color = textColorCache.GetShopItemsHeadingSeperatorColor(vo.shopSettings.avatarBorderShopItems[vo.activeShopItemId].tier);
            tierLabelLeftSeperatorImage.sprite = spriteCache.GetShopItemsHeadingLeftSeperator(vo.shopSettings.avatarBorderShopItems[vo.activeShopItemId].tier);
            tierLabelRightSeperatorImage.sprite = spriteCache.GetShopItemsHeadingRightSeperator(vo.shopSettings.avatarBorderShopItems[vo.activeShopItemId].tier);

            chessSkinImage.sprite = avatarBorderThumbsContainer.GetThumb(vo.activeShopItemId).thumbnail;

            enoughBucksDialogueLabel.text = localizationService.Get(LocalizationKey.S_M_BUY_FOR_LABEL);
            enoughBucksBuyPriceLabel.text = vo.shopSettings.avatarBorderShopItems[vo.activeShopItemId].currency2Cost.ToString();

            notEnoughBucksDialogueLabel.text = localizationService.Get(LocalizationKey.S_M_NOT_ENOUGH_BUCKS_LABEL);
            notEnoughBucksPriceLabel.text = vo.shopSettings.avatarBorderShopItems[vo.activeShopItemId].currency2Cost.ToString();

            confirmationDialogueLabel.text = localizationService.Get(LocalizationKey.S_M_PURCHASE_AVATARS_BORDER_FOR_LABEL,vo.shopSettings.avatarBorderShopItems[vo.activeShopItemId].currency2Cost.ToString());
            confirmationYesButtonLabel.text = localizationService.Get(LocalizationKey.S_M_YES_LABEL);
            confirmationNoButtonLabel.text = localizationService.Get(LocalizationKey.S_M_NO_LABEL);

            addToCollectionDialogueLabel.text = localizationService.Get(LocalizationKey.S_M_BOUGHT_AVATARS_BORDER_LABEL);
            addToCollectionButtonLabel.text = localizationService.Get(LocalizationKey.S_M_ADD_TO_COLLECTION_LABEL);

            viewCollectionDialogueLabel.text = localizationService.Get(LocalizationKey.S_M_OWNED_LABEL);
            viewCollectionButtonLabel.text = localizationService.Get(LocalizationKey.S_M_VIEW_COLLECTION_LABEL);

            activeShopItemId = vo.activeShopItemId;

            if (vo.inventorySettings.allShopItems.ContainsKey(vo.activeShopItemId))
            {
                viewCollection.SetActive(true);
            }
            else if (vo.playerModel.currency2 >= vo.shopSettings.avatarBorderShopItems[vo.activeShopItemId].currency2Cost)
            {
                enoughBucks.SetActive(true);
            }
            else
            {
                notEnoughBucks.SetActive(true);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnCloseButtonClicked()
        {
            closeButtonClickedSignal.Dispatch();
        }

        public void OnEnoughBucksBuyButtonClicked()
        {
            enoughBucks.SetActive(false);
            confirmPurchaseButtonClickedSignal.Dispatch(activeShopItemId);
            closeButtonObject.SetActive(false);
        }

        public void OnConfirmationYesButtonClicked()
        {
            //pruned
        }

        public void OnConfirmationNoButtonClicked()
        {
            //pruned
        }

        public void OnAddToCollectionButtonClicked()
        {
            addToCollection.SetActive(false);
            closeButtonClickedSignal.Dispatch();
            addToCollectionButtonClickedSignal.Dispatch();
        }

        public void OnViewCollectionButtonClicked()
        {
            viewCollectionButtonClickedSignal.Dispatch();
        }

        public void OnPurchaseResult()
        {
            addToCollection.SetActive(true);
        }
    }
}
