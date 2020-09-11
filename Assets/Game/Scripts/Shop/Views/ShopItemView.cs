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
        public ShopPayout[] payouts;
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

        private bool isInitlialised = false;
        private StoreItem storeItem;
        private static StoreIconsContainer iconsContainer;
        private static StoreThumbsContainer thumbsContainer;
        private bool isOwned;

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
                title.text = isGems ? storeItem.displayName.Split(' ')[0] : storeItem.displayName;
                icon.sprite = iconsContainer.GetSprite(shortCode);
                icon.SetNativeSize();
                thumbnail.sprite = thumbsContainer.GetSprite(isGems ? "Gem" : shortCode);
                buyButton.onClick.AddListener(OnBuyButtonClicked);
                thumbnailButton.onClick.AddListener(OnBuyButtonClicked);

                if (isBundle && storeItem.bundledItems != null)
                {
                    if (storeItem.currency3Cost > 0)
                    {
                        currencyPayout.icon.sprite = iconsContainer.GetSprite("Gem");
                        currencyPayout.count.text = storeItem.currency3Cost.ToString();
                    }

                    var i = 0;
                    foreach (var item in storeItem.bundledItems)
                    {
                        if (i < payouts.Length)
                        {
                            payouts[i].icon.sprite = iconsContainer.GetSprite(item.Key);
                            payouts[i].count.text = $"{item.Value} {storeSettingsModel.items[item.Key].displayName}";
                            i++;
                        }
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
            }

            buyButton.interactable = available;
            thumbnailButton.enabled = available;
            price.gameObject.SetActive(available);
            loading.SetActive(!available);
        }

        private void OnBuyButtonClicked()
        {
            audioService.PlayStandardClick();

            if (checkOwned && isOwned)
            {
                return;
            }

            buyButtonSignal.Dispatch(shortCode);
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
                else
                {
                    isOwned = playerModel.HasSubscription() || playerModel.OwnsVGood(shortCode);
                }

                ownedText.text = localizationService.Get(LocalizationKey.STORE_BUNDLE_FIELD_OWNED);
                buyButton.gameObject.SetActive(!isOwned);
                owned.SetActive(isOwned);
            }
        }
    }
}
