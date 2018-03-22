/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-02-21 13:32:14 UTC+05:00

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public class ShopCurrency1ModalView : View
    {
        // TODO: Remove this injection, views cannot inject services or models
        [Inject] public ILocalizationService localizationService { get; set; }

        public SpriteCache spriteCache;
        public TextColorCache textColorCache;

        public Button closeButton;

        public Text titleLabel;

        public Text rewardWithBonus;

        public Image currencyItemImage;

        public Text rewardWithoutBonus;
        public Text bonus;

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

        public GameObject enoughBucks;
        public GameObject notEnoughBucks;
        public GameObject confirmation;

        //view signals
        public Signal closeButtonClickedSignal = new Signal();
        public Signal<string> confirmPurchaseButtonClickedSignal = new Signal<string>();

        private CoinPackThumbsContainer coinPackThumbsContainer;
        private string activeShopItemId;

        public void Init()
        {
            coinPackThumbsContainer = CoinPackThumbsContainer.Load();

            closeButton.onClick.AddListener(OnCloseButtonClicked);
            confirmationYesButton.onClick.AddListener(OnConfirmationYesButtonClicked);
            confirmationNoButton.onClick.AddListener(OnConfirmationNoButtonClicked);
            enoughBucksBuyButton.onClick.AddListener(OnEnoughBucksBuyButtonClicked);
        }

        void OnDisable()
        {
            enoughBucks.SetActive(false);
            confirmation.SetActive(false);
            notEnoughBucks.SetActive(false);
        }

        public void UpdateView(ShopVO vo)
        {
            titleLabel.text = vo.shopSettings.currencyShopItems[vo.activeShopItemId].displayName;
            rewardWithBonus.text = vo.shopSettings.currencyShopItems[vo.activeShopItemId].currency1Cost.ToString();

            currencyItemImage.sprite = coinPackThumbsContainer.GetThumb(vo.activeShopItemId).thumbnail;

            rewardWithoutBonus.text = (vo.shopSettings.currencyShopItems[vo.activeShopItemId].currency1Cost - vo.shopSettings.currencyShopItems[vo.activeShopItemId].bonusAmount).ToString();
            bonus.text = localizationService.Get(LocalizationKey.S_M_BONUS_LABEL, vo.shopSettings.currencyShopItems[vo.activeShopItemId].bonusAmount);

            enoughBucksDialogueLabel.text = localizationService.Get(LocalizationKey.S_M_BUY_FOR_LABEL, vo.shopSettings.currencyShopItems[vo.activeShopItemId].displayName);
            enoughBucksBuyPriceLabel.text = vo.shopSettings.currencyShopItems[vo.activeShopItemId].currency2Cost.ToString();

            notEnoughBucksDialogueLabel.text = localizationService.Get(LocalizationKey.S_M_NOT_ENOUGH_BUCKS_LABEL);
            notEnoughBucksPriceLabel.text = vo.shopSettings.currencyShopItems[vo.activeShopItemId].currency2Cost.ToString();

            confirmationDialogueLabel.text = localizationService.Get(LocalizationKey.S_M_PURCHASE_CURRENCY_FOR_LABEL,vo.shopSettings.currencyShopItems[vo.activeShopItemId].displayName,vo.shopSettings.currencyShopItems[vo.activeShopItemId].currency2Cost.ToString());
            confirmationYesButtonLabel.text = localizationService.Get(LocalizationKey.S_M_YES_LABEL);
            confirmationNoButtonLabel.text = localizationService.Get(LocalizationKey.S_M_NO_LABEL);

            activeShopItemId = vo.activeShopItemId;

            if (vo.playerModel.currency2 >= vo.shopSettings.currencyShopItems[vo.activeShopItemId].currency2Cost)
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
            closeButtonClickedSignal.Dispatch();
            confirmPurchaseButtonClickedSignal.Dispatch(activeShopItemId);
        }

        public void OnConfirmationYesButtonClicked()
        {
            //pruned
        }

        public void OnConfirmationNoButtonClicked()
        {
            //pruned
        }

        public void OnPurchaseResult()
        {
            closeButtonClickedSignal.Dispatch();
        }
    }
}