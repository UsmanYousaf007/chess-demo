/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-03-05 11:54:02 UTC+05:00

using strange.extensions.mediation.impl;
using UnityEngine.UI;
using UnityEngine;
using strange.extensions.signal.impl;
using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public class InventoryLootDismantleModalView : View
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

        public Image thumbnailFrame;
        public Image thumbnailImage;

        public Text ownedCardLabel;

        public Text coinsAmount;

        public Button plusButton;
        public Button minusButton;
        public Text dismantlingCardNoLabel;

        public Text diaglogueLabel;

        public Button dismantleButton;
        public Text dismantleButtonLabel;

        public Button collectButton;
        public Text collectButtonLabel;

        public GameObject dismantle;
        public GameObject collectCoins;
        public GameObject chessSkinAndAvatarThumbnail;
        public GameObject avatarBorderThumbnail;


        //view signals
        public Signal closeButtonClickedSignal = new Signal();
        public Signal<ForgeCardVO> dismantleButtonClickedSignal = new Signal<ForgeCardVO>();

        private SkinThumbsContainer skinThumbsContainer;
        private AvatarThumbsContainer avatarThumbsContainer;
        private AvatarBorderThumbsContainer avatarBorderThumbsContainer;

        private string activeInvetoryForgeCardItemId;
        private string activeInventoryItemId;
        private ShopVO currentShopVo;
        private int dismantleQuantity = 1;
        private int totalCardsOwned;
        private int sellForCoinsAmount;

        public void Init()
        {
            skinThumbsContainer = SkinThumbsContainer.Load();
            avatarThumbsContainer = AvatarThumbsContainer.Load();
            avatarBorderThumbsContainer = AvatarBorderThumbsContainer.Load();

            closeButton.onClick.AddListener(OnCloseButtonClicked);
            plusButton.onClick.AddListener(OnPlusButtonClicked);
            minusButton.onClick.AddListener(OnMinusButtonClicked);
            dismantleButton.onClick.AddListener(OnDismantleButtonClicked);
            collectButton.onClick.AddListener(OnCollectButtonClicked);
        }

        void OnDisable()
        {
            dismantle.SetActive(false);
            collectCoins.SetActive(false);
            chessSkinAndAvatarThumbnail.SetActive(false);
            avatarBorderThumbnail.SetActive(false);
        }

        public void UpdateView(ShopVO vo)
        {
            currentShopVo = vo;

            diaglogueLabel.text = localizationService.Get(LocalizationKey.I_DISMANTLE_DIALOGUE_LABEL);
            dismantleButtonLabel.text = localizationService.Get(LocalizationKey.I_DISMANTLE_BUTTON_LABEL);
            collectButtonLabel.text = localizationService.Get(LocalizationKey.I_COLLECT_BUTTON_LABEL);


            LogUtil.Log("The active ID is : " + vo.activeInventoryItemId);

            ShopItem item = vo.shopSettings.allShopItems[vo.forgeSettingsModel.forgeItems[vo.activeForgeCardItemId].forgeItemKey] as ShopItem; 

            LogUtil.Log("The The TheCard Nos are: " + vo.inventorySettings.allShopItems[vo.activeForgeCardItemId] + " forgeCardKey: " + vo.activeForgeCardItemId, "red");

            if (item.kind == GSBackendKeys.ShopItem.AVATAR_SHOP_TAG)
            {
                chessSkinAndAvatarThumbnail.SetActive(true);

                titleLabel.text = vo.shopSettings.avatarShopItems[vo.activeInventoryItemId].displayName;
                tierLabel.text = vo.shopSettings.avatarShopItems[vo.activeInventoryItemId].tier;  
                tierLabel.color = textColorCache.GetShopItemsHeadingSeperatorColor(vo.shopSettings.avatarShopItems[vo.activeInventoryItemId].tier);
                tierLabelLeftSeperatorImage.sprite = spriteCache.GetShopItemsHeadingLeftSeperator(vo.shopSettings.avatarShopItems[vo.activeInventoryItemId].tier);
                tierLabelRightSeperatorImage.sprite = spriteCache.GetShopItemsHeadingRightSeperator(vo.shopSettings.avatarShopItems[vo.activeInventoryItemId].tier);

                thumbnailFrame.sprite = spriteCache.GetShopItemsFrame(vo.shopSettings.avatarShopItems[vo.activeInventoryItemId].tier);
                thumbnailImage.sprite = avatarThumbsContainer.GetThumb(vo.activeInventoryItemId).thumbnail;
            }

            else if (item.kind == GSBackendKeys.ShopItem.AVATARBORDER_SHOP_TAG)
            {
                avatarBorderThumbnail.SetActive(true);

                titleLabel.text = vo.shopSettings.avatarBorderShopItems[vo.activeInventoryItemId].displayName;
                tierLabel.text = vo.shopSettings.avatarBorderShopItems[vo.activeInventoryItemId].tier;
                tierLabel.color = textColorCache.GetShopItemsHeadingSeperatorColor(vo.shopSettings.avatarBorderShopItems[vo.activeInventoryItemId].tier);
                tierLabelLeftSeperatorImage.sprite = spriteCache.GetShopItemsHeadingLeftSeperator(vo.shopSettings.avatarBorderShopItems[vo.activeInventoryItemId].tier);
                tierLabelRightSeperatorImage.sprite = spriteCache.GetShopItemsHeadingRightSeperator(vo.shopSettings.avatarBorderShopItems[vo.activeInventoryItemId].tier);

                thumbnailFrame.sprite = spriteCache.GetShopItemsFrame(vo.shopSettings.avatarBorderShopItems[vo.activeInventoryItemId].tier);
                thumbnailImage.sprite = avatarBorderThumbsContainer.GetThumb(vo.activeInventoryItemId).thumbnail;
            }

            else if (item.kind == GSBackendKeys.ShopItem.SKIN_SHOP_TAG)
            {
                chessSkinAndAvatarThumbnail.SetActive(true);

                titleLabel.text = vo.shopSettings.skinShopItems[vo.activeInventoryItemId].displayName;
                tierLabel.text = vo.shopSettings.skinShopItems[vo.activeInventoryItemId].tier;
                tierLabel.color = textColorCache.GetShopItemsHeadingSeperatorColor(vo.shopSettings.skinShopItems[vo.activeInventoryItemId].tier);
                tierLabelLeftSeperatorImage.sprite = spriteCache.GetShopItemsHeadingLeftSeperator(vo.shopSettings.skinShopItems[vo.activeInventoryItemId].tier);
                tierLabelRightSeperatorImage.sprite = spriteCache.GetShopItemsHeadingRightSeperator(vo.shopSettings.skinShopItems[vo.activeInventoryItemId].tier);

                thumbnailFrame.sprite = spriteCache.GetShopItemsFrame(vo.shopSettings.skinShopItems[vo.activeInventoryItemId].tier);
                thumbnailImage.sprite = skinThumbsContainer.GetThumb(vo.activeInventoryItemId).thumbnail;
            }

            //vo.forgeSettingsModel.forgeItems[vo.activeForgeCardItemId].requiredQuantity

            sellForCoinsAmount = vo.forgeSettingsModel.forgeItems[vo.activeForgeCardItemId].sellCoins;
            totalCardsOwned = vo.inventorySettings.allShopItems[vo.activeForgeCardItemId];

            coinsAmount.text = (dismantleQuantity * sellForCoinsAmount).ToString();
            ownedCardLabel.text = localizationService.Get(LocalizationKey.I_X_LABEL, totalCardsOwned);
            dismantlingCardNoLabel.text = localizationService.Get(LocalizationKey.I_DIVIDER_LABEL, dismantleQuantity, totalCardsOwned);
            dismantle.SetActive(true);

            activeInvetoryForgeCardItemId = vo.activeForgeCardItemId;
            activeInventoryItemId = vo.activeInventoryItemId;
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

        private void OnPlusButtonClicked()
        {
            if (dismantleQuantity < totalCardsOwned)
            {
                ++dismantleQuantity;
                coinsAmount.text = (dismantleQuantity * sellForCoinsAmount).ToString();
                dismantlingCardNoLabel.text = localizationService.Get(LocalizationKey.I_DIVIDER_LABEL, dismantleQuantity, totalCardsOwned);
            }
        }

        private void OnMinusButtonClicked()
        {
            if (dismantleQuantity > 1)
            {
                --dismantleQuantity;
                coinsAmount.text = (dismantleQuantity * sellForCoinsAmount).ToString();
                dismantlingCardNoLabel.text = localizationService.Get(LocalizationKey.I_DIVIDER_LABEL, dismantleQuantity, totalCardsOwned);
            }
        }

        public void OnDismantleButtonClicked()
        {
            dismantle.SetActive(false);
            ForgeCardVO vo;
            vo.cardId = activeInvetoryForgeCardItemId ;
            vo.numSell = dismantleQuantity;

            dismantleButtonClickedSignal.Dispatch(vo);

            LogUtil.Log("Dismantle button clicked", "red");
        }

        public void OnDismantleSuccess()
        {
            collectCoins.SetActive(true);
        }

        public void OnCollectButtonClicked()
        {
            collectCoins.SetActive(false);
            closeButtonClickedSignal.Dispatch();
            LogUtil.Log("Collect button clicked", "red");
        }
    }
}
