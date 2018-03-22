/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-02-27 14:25:55 UTC+05:00

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public class InventoryChessSkinsInfoModalView : View
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

        public Image chessSkinPreviewImage;
        public Image thumbnailFrame;
        public Image thumbnailImage;

        public Button equipButton;
        public Text equipButtonLabel;

        public Text equippedLabel;

        public GameObject equipped;
        public GameObject alreadyEquipped;

        //view signals
        public Signal closeButtonClickedSignal = new Signal();
        public Signal<string> equipButtonClickedSignal = new Signal<string>();

        private SkinThumbsContainer skinThumbsContainer;
        private string activeInventoryItemId;
        private ShopVO currentShopVo;

        public void Init()
        {
            skinThumbsContainer = SkinThumbsContainer.Load();

            closeButton.onClick.AddListener(OnCloseButtonClicked);
            equipButton.onClick.AddListener(OnEquipButtonClicked);
        }

        void OnDisable()
        {
            equipped.SetActive(false);
            alreadyEquipped.SetActive(false);
        }

        public void UpdateView(ShopVO vo)
        {
            currentShopVo = vo;
            
            equipButtonLabel.text = localizationService.Get(LocalizationKey.I_EQUIP_LABEL);;
            equippedLabel.text = localizationService.Get(LocalizationKey.I_EQUIPPED_LABEL);
            
            titleLabel.text = vo.shopSettings.skinShopItems[vo.activeInventoryItemId].displayName;
            tierLabel.text = vo.shopSettings.skinShopItems[vo.activeInventoryItemId].tier;
            tierLabel.color = textColorCache.GetShopItemsHeadingSeperatorColor(vo.shopSettings.skinShopItems[vo.activeInventoryItemId].tier);
            tierLabelLeftSeperatorImage.sprite = spriteCache.GetShopItemsHeadingLeftSeperator(vo.shopSettings.skinShopItems[vo.activeInventoryItemId].tier);
            tierLabelRightSeperatorImage.sprite = spriteCache.GetShopItemsHeadingRightSeperator(vo.shopSettings.skinShopItems[vo.activeInventoryItemId].tier);

            chessSkinPreviewImage.sprite = skinThumbsContainer.GetThumb(vo.activeInventoryItemId).preview;
            thumbnailFrame.sprite = spriteCache.GetShopItemsFrame(vo.shopSettings.skinShopItems[vo.activeInventoryItemId].tier);
            thumbnailImage.sprite = skinThumbsContainer.GetThumb(vo.activeInventoryItemId).thumbnail;


            if (vo.activeInventoryItemId == vo.inventorySettings.activeChessSkinsId)
            {
                alreadyEquipped.SetActive(true);
            }
            else
            {
                equipped.SetActive(true);
            }

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

        public void OnEquipButtonClicked()
        {
            currentShopVo.inventorySettings.activeChessSkinsId = activeInventoryItemId;
            equipButtonClickedSignal.Dispatch(activeInventoryItemId);
            closeButtonClickedSignal.Dispatch();
        }
    }
}
