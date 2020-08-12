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
        public bool isGems;

        private bool isInitlialised = false;
        private StoreItem storeItem;

        //Models
        [Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        //Services
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public ILocalizationService localizationService { get; set; }

        //Dispatch Signals
        public Signal<string> buyButtonSignal = new Signal<string>();

        public void OnStoreAvailable(StoreIconsContainer iconsConainter, StoreThumbsContainer thumbsContainer, bool available)
        {
            if (storeItem == null && metaDataModel.store.items.ContainsKey(shortCode))
            {
                storeItem = metaDataModel.store.items[shortCode];
            }

            if (storeItem == null)
            {
                return;
            }

            if (!isInitlialised)
            {
                title.text = isGems ? storeItem.displayName.Split(' ')[0] : storeItem.displayName;
                icon.sprite = iconsConainter.GetSprite(shortCode);
                icon.SetNativeSize();
                thumbnail.sprite = thumbsContainer.GetSprite(isGems ? "Gem" : shortCode);
                buyButton.onClick.AddListener(OnBuyButtonClicked);

                if (storeItem.currency3Payout > 0)
                {
                    currencyPayout.icon.sprite = iconsConainter.GetSprite("Gem");
                    currencyPayout.count.text = storeItem.currency3Payout.ToString();
                }

                var i = 0;
                foreach (var item in storeItem.bundledItems)
                {
                    payouts[i].icon.sprite = iconsConainter.GetSprite(item.Key);
                    payouts[i].count.text = item.Value.ToString();
                }

                if (checkOwned)
                {
                    var isOwned = playerModel.HasSubscription() || playerModel.OwnsVGood(shortCode);
                    ownedText.text = localizationService.Get(LocalizationKey.STORE_BUNDLE_FIELD_OWNED);
                    buyButton.gameObject.SetActive(!isOwned);
                    owned.SetActive(isOwned);
                }

                isInitlialised = true;
            }

            if (available)
            {
                price.text = storeItem.remoteProductPrice;
            }

            buyButton.interactable = available;
            price.gameObject.SetActive(available);
            loading.SetActive(!available);
        }

        private void OnBuyButtonClicked()
        {
            audioService.PlayStandardClick();
            buyButtonSignal.Dispatch(shortCode);
        }
    }
}
