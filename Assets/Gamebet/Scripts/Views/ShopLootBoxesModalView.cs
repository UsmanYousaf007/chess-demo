/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-02-21 13:28:50 UTC+05:00

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.Common;
using System.Collections.Generic;

namespace TurboLabz.Gamebet
{
    public class ShopLootBoxesModalView : View
    {
        // TODO: Remove this injection, views cannot inject services or models
        [Inject] public ILocalizationService localizationService { get; set; }

        public SpriteCache spriteCache;
        public TextColorCache textColorCache;

        public Button closeButton;

        public Text titleLabel;

        public Image lootBoxImage;

        public Text enoughBucksDialogueLabel;
        public Button enoughBucksBuyButton;
        public Text enoughBucksBuyPriceLabel;

        public Text notEnoughBucksDialogueLabel;
        public Text notEnoughBucksPriceLabel;

        public Text addToLootDialogueLabel;
        public Button addToLootButton;
        public Text addToLootButtonLabel;

        public GameObject closeButtonObject;
        public LootBoxReward lootBoxRewardPrefab;
        public Transform rewardGroup;
        public GameObject enoughBucks;
        public GameObject notEnoughBucks;
        public GameObject addToLoot;

        //view signals
        public Signal closeButtonClickedSignal = new Signal();
        public Signal<string> confirmPurchaseButtonClickedSignal = new Signal<string>();

        private LootThumbsContainer lootThumbsContainer;
        private string activeShopItemId;

        public void Init()
        {
            lootThumbsContainer = LootThumbsContainer.Load();

            closeButton.onClick.AddListener(OnCloseButtonClicked);
            enoughBucksBuyButton.onClick.AddListener(OnEnoughBucksBuyButtonClicked);
            addToLootButton.onClick.AddListener(OnAddToCollectionButtonClicked);
        }

        void OnDisable()
        {
            enoughBucks.SetActive(false);
            notEnoughBucks.SetActive(false);
            addToLoot.SetActive(false);
        }

        public void UpdateView(ShopVO vo)
        {
            closeButtonObject.SetActive(true);

            activeShopItemId = vo.activeShopItemId;

            titleLabel.text = vo.shopSettings.lootBoxShopItems[vo.activeShopItemId].displayName;

            lootBoxImage.sprite = lootThumbsContainer.GetThumb(vo.activeShopItemId).thumbnail;

            UnityUtil.DestroyChildren(rewardGroup.transform);

            IList<string> itemDescription =  vo.shopSettings.lootBoxShopItems[vo.activeShopItemId].itemDescriptions;

            foreach(string description in itemDescription)
            {
                LootBoxReward lootBoxReward = Instantiate<LootBoxReward>(lootBoxRewardPrefab);
                lootBoxReward.rewardLabel.text = description;
                lootBoxReward.transform.SetParent(rewardGroup.transform, false);
            }

            enoughBucksDialogueLabel.text = localizationService.Get(LocalizationKey.S_M_BUY_FOR_LABEL);
            enoughBucksBuyPriceLabel.text = vo.shopSettings.lootBoxShopItems[vo.activeShopItemId].currency2Cost.ToString();

            notEnoughBucksDialogueLabel.text = localizationService.Get(LocalizationKey.S_M_NOT_ENOUGH_BUCKS_LABEL);
            notEnoughBucksPriceLabel.text = vo.shopSettings.lootBoxShopItems[vo.activeShopItemId].currency2Cost.ToString();

            addToLootDialogueLabel.text = localizationService.Get(LocalizationKey.S_M_BOUGHT_LOOT_BOX_LABEL);
            addToLootButtonLabel.text = localizationService.Get(LocalizationKey.S_M_ADD_TO_LOOT_LABEL);

            if (vo.playerModel.currency2 >= vo.shopSettings.lootBoxShopItems[vo.activeShopItemId].currency2Cost)
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
            confirmPurchaseButtonClickedSignal.Dispatch(activeShopItemId);
            enoughBucks.SetActive(false);
            closeButtonObject.SetActive(false);
        }

        public void OnAddToCollectionButtonClicked()
        {
            addToLoot.SetActive(false);
            closeButtonClickedSignal.Dispatch();
        }

        public void OnPurchaseResult()
        {
            addToLoot.SetActive(true);
        }
    }
}
