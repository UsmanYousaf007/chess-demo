using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace TurboLabz.InstantFramework
{
    public class ShopItemView : View
    {
        [Serializable]
        public class ShopPayout
        {
            public Image icon;
            public Text count;
        }

        public string shortCode;
        public Text title;
        public ShopPayout currencyPayout;
        public ShopPayout currency2Payout;
        public Text price;
        public Button buyButton;
        public GameObject loading;
        public Image icon;
        public Image thumbnail;
        public bool checkOwned;
        public GameObject owned;
        public Text ownedText;
        public Button thumbnailButton;
        public bool isGems;
        public bool isBundle;
        public bool showDiscount;
        public GameObject discountObj;
        public float discountPercentage;
        public Text discountAmount;
        public Text discountValue;
        public bool isSpot;
        public bool loadThumbnail = true;
        public bool canGoOnSale;
        public string saleShortCode;
        public Text orignalPrice;
        public Text newPrice;
        public GameObject ribbon;
        public Text ribbonText;

        private bool isInitlialised = false;
        private StoreItem storeItem;
        private StoreItem saleItem;
        private static StoreIconsContainer iconsContainer;
        private static StoreThumbsContainer thumbsContainer;
        private bool isOwned;
        private bool isOnSale;

        //Models
        [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        //Services
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public ILocalizationService localizationService { get; set; }

        //Dispatch Signals
        public Signal<string> buyButtonSignal = new Signal<string>();

        public void Init()
        {
            if (iconsContainer == null)
            {
                iconsContainer = StoreIconsContainer.Load();
            }

            if (thumbsContainer == null)
            {
                thumbsContainer = StoreThumbsContainer.Load();
            }

            ShowSaleItems(false);
        }

        public void OnStoreAvailable(bool available)
        {
            if (storeItem == null && storeSettingsModel.items.ContainsKey(shortCode))
            {
                storeItem = storeSettingsModel.items[shortCode];
            }

            if (storeItem == null)
            {
                return;
            }

            if (!isInitlialised)
            {
                title.text = isGems ? storeItem.displayName.Split(' ')[0] : isBundle ? storeItem.description : storeItem.displayName;
                icon.sprite = iconsContainer.GetSprite(shortCode);
                icon.SetNativeSize();
                buyButton.onClick.AddListener(OnBuyButtonClicked);
                thumbnailButton.onClick.AddListener(OnBuyButtonClicked);

                if (loadThumbnail)
                {
                    thumbnail.sprite = thumbsContainer.GetSprite(isGems ? "Gem" : shortCode);
                }

                if (isBundle)
                {
                    if (storeItem.currency3Cost > 0)
                    {
                        currencyPayout.icon.sprite = iconsContainer.GetSprite("Gem");
                        currencyPayout.count.text = storeItem.currency3Cost.ToString();
                    }

                    if (storeItem.currency4Cost > 0)
                    {
                        currency2Payout.icon.sprite = iconsContainer.GetSprite("Coin");
                        currency2Payout.count.text = storeItem.currency4Cost.ToString("N0");
                    }
                }

                SetOwnedStatus();

                if (showDiscount)
                {
                    discountObj.SetActive(true);
                    discountAmount.text = $"{discountPercentage * 100}% Bonus";
                    discountValue.text = $"{(int)(storeItem.currency3Payout - (storeItem.currency3Payout * discountPercentage))}";
                }

                isInitlialised = true;
            }

            if (available)
            {
                price.text = storeItem.remoteProductPrice;
                SetupSalePrice();
            }

            buyButton.interactable = available;
            thumbnailButton.enabled = available;
            price.gameObject.SetActive(available && !isOnSale);
            loading.SetActive(!available);
        }

        private void OnBuyButtonClicked()
        {
            audioService.PlayStandardClick();

            if (checkOwned && isOwned)
            {
                return;
            }

            buyButtonSignal.Dispatch(isOnSale ? saleShortCode : shortCode);
        }

        public void SetOwnedStatus()
        {
            if (checkOwned)
            {
                if (shortCode.Equals(GSBackendKeys.ShopItem.ALL_THEMES_PACK))
                {
                    isOwned = playerModel.OwnsAllThemes();
                }
                else if (shortCode.Equals(GSBackendKeys.ShopItem.ALL_LESSONS_PACK))
                {
                    isOwned = playerModel.OwnsAllLessons();
                }
                else if (shortCode.Equals(GSBackendKeys.ShopItem.REMOVE_ADS_PACK))
                {
                    isOwned = playerModel.HasRemoveAds();
                }
                else
                {
                    isOwned = playerModel.HasSubscription() || playerModel.OwnsVGood(shortCode);
                }

                ownedText.text = localizationService.Get(LocalizationKey.STORE_BUNDLE_FIELD_OWNED);
                buyButton.gameObject.SetActive(!isOwned);
                owned.SetActive(isOwned);
            }
        }

        private void ShowSaleItems(bool show)
        {
            orignalPrice.gameObject.SetActive(show);
            newPrice.gameObject.SetActive(show);
            ribbon.gameObject.SetActive(show);
            price.gameObject.SetActive(!show);
        }

        private void SetupSalePrice()
        {
            if (!canGoOnSale)
            {
                return;
            }

            saleItem = storeSettingsModel.items[saleShortCode];

            if (saleItem == null)
            {
                return;
            }

            var discount = storeItem.productPrice > 0 ? 1 - (float)(saleItem.productPrice / storeItem.productPrice) : 0.5f;
            orignalPrice.text = storeItem.remoteProductPrice;
            newPrice.text = saleItem.remoteProductPrice;
            ribbonText.text = $"FIRE SALE! {(int)(discount * 100)}% OFF";
        }

        public void SetupSale(string saleKey)
        {
            if (canGoOnSale && saleKey.Equals(saleShortCode))
            {
                isOnSale = true;
                ShowSaleItems(true);
            }
        }
    }
}
