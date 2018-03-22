/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-03-08 16:44:47 UTC+05:00
using strange.extensions.mediation.impl;
using UnityEngine.UI;
using strange.extensions.signal.impl;
using UnityEngine;
using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public class InventoryLootInfoModalView : View
    {
        // TODO: Remove this injection, views cannot inject services or models
        [Inject] public ILocalizationService localizationService { get; set; }

        public SpriteCache spriteCache;
        public TextColorCache textColorCache;

        [Header("Avatar")]

        public Button avatarCloseButton;

        public Text avatarTitleLabel;

        public Image avatarThumbnailPreview;
        public Image avatarThumbnailFrame;
        public Image avatarThumbnailImage;

        public Text avatarCardNos;

        public Button avatarBuildButton;
        public Text avatarBuildButtonLabel;
        public Text avatarNotEnoughCardsBuildButtonLabel;

        public Text avatarNotEnoughCardDialogue;
        public Text avatarOwnedLabel;

        public GameObject avatarBuild;
        public GameObject avatarNotEnoughCards;
        public GameObject avatarOwned;

        public GameObject avatarThumbnail;

        [Header("Avatar Border")]

        public Image avatarBorderThumbnailPreview;
        public Image avatarBorderThumbnailFrame;
        public Image avatarBorderThumbnailImage;

        public GameObject avatarBorderThumbnail;

        [Header("Chess Skin")]

        public Button chessSkinCloseButton;

        public Text chessSkinTitleLabel;
        public Text tierLabel;
        public Image tierLabelLeftSeperatorImage;
        public Image tierLabelRightSeperatorImage;

        public Image chessSkinThumbnailPreview;
        public Image chessSkinThumbnailFrame;
        public Image chessSkinThumbnailImage;

        public Text chessSkinCardNos;

        public Button chessSkinBuildButton;
        public Text chessSkinBuildButtonLabel;
        public Text chessSkinNotEnoughCardsBuildButtonLabel;

        public Text chessSkinNotEnoughCardDialogue;
        public Text chessSkinOwnedLabel;

        public GameObject chessSkinBuild;
        public GameObject chessSkinNotEnoughCards;
        public GameObject chessSkinOwned;

        public GameObject tierHeading;
        public GameObject chessSkinThumbnail;

        //view signals
        public Signal closeButtonClickedSignal = new Signal();
        public Signal<string> buildButtonClickedSignal = new Signal<string>();

        private string currentForgeCardKey;

        private SkinThumbsContainer skinThumbsContainer;
        private AvatarThumbsContainer avatarThumbsContainer;
        private AvatarBorderThumbsContainer avatarBorderThumbsContainer;

        public void Init()
        {
            skinThumbsContainer = SkinThumbsContainer.Load();
            avatarThumbsContainer = AvatarThumbsContainer.Load();
            avatarBorderThumbsContainer = AvatarBorderThumbsContainer.Load();

            avatarCloseButton.onClick.AddListener(OnCloseButtonClicked);
            avatarBuildButton.onClick.AddListener(OnBuildButtonClicked);

            chessSkinCloseButton.onClick.AddListener(OnCloseButtonClicked);
            chessSkinBuildButton.onClick.AddListener(OnBuildButtonClicked);
        }

        void OnDisable()
        {   
            tierHeading.SetActive(false);
            avatarThumbnail.SetActive(false);
            avatarBorderThumbnail.SetActive(false);
            chessSkinThumbnail.SetActive(false);

            avatarBuild.SetActive(false);
            avatarOwned.SetActive(false);
            avatarNotEnoughCards.SetActive(false);

            chessSkinBuild.SetActive(false);
            chessSkinOwned.SetActive(false);
            chessSkinNotEnoughCards.SetActive(false);
        }

        public void UpdateView(ShopVO vo)
        {
            currentForgeCardKey = vo.activeForgeCardItemId;

            avatarNotEnoughCardDialogue.text = localizationService.Get(LocalizationKey.I_NOT_ENOUGH_CARDS_LABEL);
            avatarBuildButtonLabel.text = localizationService.Get(LocalizationKey.I_BUILD_BUTTON_LABEL);
            avatarNotEnoughCardsBuildButtonLabel.text = localizationService.Get(LocalizationKey.I_BUILD_BUTTON_LABEL);
            avatarOwnedLabel.text = localizationService.Get(LocalizationKey.I_OWNED_Label);

            chessSkinNotEnoughCardDialogue.text = localizationService.Get(LocalizationKey.I_NOT_ENOUGH_CARDS_LABEL);
            chessSkinBuildButtonLabel.text = localizationService.Get(LocalizationKey.I_BUILD_BUTTON_LABEL);
            chessSkinNotEnoughCardsBuildButtonLabel.text = localizationService.Get(LocalizationKey.I_BUILD_BUTTON_LABEL);
            chessSkinOwnedLabel.text = localizationService.Get(LocalizationKey.I_OWNED_Label);

            LogUtil.Log("The active ID is : " + vo.activeInventoryItemId);

            ShopItem item = vo.shopSettings.allShopItems[vo.activeInventoryItemId] as ShopItem; 

            LogUtil.Log("The The TheCard Nos are: " + vo.inventorySettings.allShopItems[vo.activeForgeCardItemId] + " forgeCardKey: " + vo.activeForgeCardItemId, "red");

            if (item.kind == GSBackendKeys.ShopItem.AVATAR_SHOP_TAG)
            {
                avatarThumbnail.SetActive(true);

                avatarTitleLabel.text = vo.shopSettings.avatarShopItems[vo.activeInventoryItemId].displayName;
            
                if (vo.inventorySettings.allShopItems.ContainsKey(vo.activeInventoryItemId))
                {
                    avatarCardNos.text = localizationService.Get(LocalizationKey.I_X_LABEL, vo.inventorySettings.allShopItems[vo.activeForgeCardItemId]);
                }
                else
                {
                    avatarCardNos.text = localizationService.Get(LocalizationKey.I_DIVIDER_LABEL, vo.inventorySettings.allShopItems[vo.activeForgeCardItemId], vo.forgeSettingsModel.forgeItems[vo.activeForgeCardItemId].requiredQuantity);
                }

                avatarThumbnailFrame.sprite = spriteCache.GetShopItemsFrame(vo.shopSettings.avatarShopItems[vo.activeInventoryItemId].tier);
                avatarThumbnailImage.sprite = avatarThumbsContainer.GetThumb(vo.activeInventoryItemId).thumbnail;
                avatarThumbnailPreview.sprite = avatarThumbsContainer.GetThumb(vo.activeInventoryItemId).thumbnail;
            }

            else if (item.kind == GSBackendKeys.ShopItem.AVATARBORDER_SHOP_TAG)
            {
                avatarBorderThumbnail.SetActive(true);
                tierHeading.SetActive(true);

                chessSkinTitleLabel.text = vo.shopSettings.avatarBorderShopItems[vo.activeInventoryItemId].displayName;
                tierLabel.text = vo.shopSettings.avatarBorderShopItems[vo.activeInventoryItemId].tier;
                tierLabel.color = textColorCache.GetShopItemsHeadingSeperatorColor(vo.shopSettings.avatarBorderShopItems[vo.activeInventoryItemId].tier);
                tierLabelLeftSeperatorImage.sprite = spriteCache.GetShopItemsHeadingLeftSeperator(vo.shopSettings.avatarBorderShopItems[vo.activeInventoryItemId].tier);
                tierLabelRightSeperatorImage.sprite = spriteCache.GetShopItemsHeadingRightSeperator(vo.shopSettings.avatarBorderShopItems[vo.activeInventoryItemId].tier);

                avatarBorderThumbnailFrame.sprite = spriteCache.GetShopItemsFrame(vo.shopSettings.avatarBorderShopItems[vo.activeInventoryItemId].tier);
                avatarBorderThumbnailImage.sprite = avatarBorderThumbsContainer.GetThumb(vo.activeInventoryItemId).thumbnail;
                avatarBorderThumbnailPreview.sprite = avatarBorderThumbsContainer.GetThumb(vo.activeInventoryItemId).thumbnail;
            }

            else if (item.kind == GSBackendKeys.ShopItem.SKIN_SHOP_TAG)
            {
                chessSkinThumbnail.SetActive(true);
                tierHeading.SetActive(true);

                chessSkinTitleLabel.text = vo.shopSettings.skinShopItems[vo.activeInventoryItemId].displayName;
                tierLabel.text = vo.shopSettings.skinShopItems[vo.activeInventoryItemId].tier;
                tierLabel.color = textColorCache.GetShopItemsHeadingSeperatorColor(vo.shopSettings.skinShopItems[vo.activeInventoryItemId].tier);
                tierLabelLeftSeperatorImage.sprite = spriteCache.GetShopItemsHeadingLeftSeperator(vo.shopSettings.skinShopItems[vo.activeInventoryItemId].tier);
                tierLabelRightSeperatorImage.sprite = spriteCache.GetShopItemsHeadingRightSeperator(vo.shopSettings.skinShopItems[vo.activeInventoryItemId].tier);

                if (vo.inventorySettings.allShopItems.ContainsKey(vo.activeInventoryItemId))
                {
                    chessSkinCardNos.text = localizationService.Get(LocalizationKey.I_X_LABEL, vo.inventorySettings.allShopItems[vo.activeForgeCardItemId]);
                }
                else
                {
                    chessSkinCardNos.text = localizationService.Get(LocalizationKey.I_DIVIDER_LABEL, vo.inventorySettings.allShopItems[vo.activeForgeCardItemId], vo.forgeSettingsModel.forgeItems[vo.activeForgeCardItemId].requiredQuantity);
                }

                chessSkinThumbnailFrame.sprite = spriteCache.GetShopItemsFrame(vo.shopSettings.skinShopItems[vo.activeInventoryItemId].tier);
                chessSkinThumbnailImage.sprite = skinThumbsContainer.GetThumb(vo.activeInventoryItemId).thumbnail;
                chessSkinThumbnailPreview.sprite = skinThumbsContainer.GetThumb(vo.activeInventoryItemId).preview;
            }

            if (vo.inventorySettings.allShopItems.ContainsKey(vo.activeInventoryItemId))
            {
                chessSkinOwned.SetActive(true);
                avatarOwned.SetActive(true);
            }
            else if (vo.inventorySettings.allShopItems[vo.activeForgeCardItemId] >= vo.forgeSettingsModel.forgeItems[vo.activeForgeCardItemId].requiredQuantity)
            {
                chessSkinBuild.SetActive(true);
                avatarBuild.SetActive(true);
            }
            else
            {
                chessSkinNotEnoughCards.SetActive(true);
                avatarNotEnoughCards.SetActive(true);
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

        public void OnBuildForgeCardSuccess()
        {
            closeButtonClickedSignal.Dispatch();
        }

        private void OnCloseButtonClicked()
        {
            closeButtonClickedSignal.Dispatch();
        }

        private void OnBuildButtonClicked()
        {
            buildButtonClickedSignal.Dispatch(currentForgeCardKey);
            LogUtil.Log("The build button is clicked", "red");
        }
    }
}