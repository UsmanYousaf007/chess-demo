using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.InstantFramework
{
    public class InventoryItemView : View
    {
        public string shortCode;
        public Text title;
        public Image icon;
        public Text description;
        public RectTransform rewardedVideoProgressBar;
        public Text rewardedVideoProgressBarText;
        public Button watchAdButton;
        public Text watchAdText;
        public Button buyButton;
        public Text price;
        public Text count;
        public Image thumbnail;
        public GameObject toolTip;
        public Text toolTipText;
        public Sprite enoughGems;
        public Sprite notEnoughGems;
        public bool skipIconLoading;

        private static StoreIconsContainer iconsContainer;
        private static StoreThumbsContainer thumbsContainer;

        private bool isInitlialised = false;
        private StoreItem storeItem;
        private float rewardBarOriginalWidth;

        //Models
        [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }

        //Services
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public ILocalizationService localizationService { get; set; }

        //Dispatch Signals
        public Signal<string> buyButtonSignal = new Signal<string>();
        public Signal<string> watchAdSignal = new Signal<string>();

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

            rewardBarOriginalWidth = rewardedVideoProgressBar.sizeDelta.x;
            //bar fill code
            //var barFillPercentage = playerModel.rewardCurrentPoints / playerModel.rewardPointsRequired;
            //rewardBar.sizeDelta = new Vector2(rewardBarOriginalWidth * barFillPercentage, rewardBar.sizeDelta.y);
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
                buyButton.onClick.AddListener(OnBuyButtonClicked);
                watchAdButton.onClick.AddListener(OnWatchAdButtonClicked);
                title.text = storeItem.displayName;
                description.text = storeItem.description;
                thumbnail.sprite = thumbsContainer.GetSprite(shortCode);
                toolTip.SetActive(false);
                rewardedVideoProgressBar.sizeDelta = new Vector2(0, rewardedVideoProgressBar.sizeDelta.y);
                var rewardedVideoCost = settingsModel.inventorySpecialItemsRewardedVideoCost.ContainsKey(shortCode) ? settingsModel.inventorySpecialItemsRewardedVideoCost[shortCode] : 0;
                rewardedVideoProgressBarText.text = $"0/{rewardedVideoCost}";
                watchAdText.text = localizationService.Get(LocalizationKey.INVENTORY_WATCH_AD);
                toolTipText.text = localizationService.Get(LocalizationKey.INVENTORY_TOOL_TIP);
                SetupPriceAndCount();

                if (!skipIconLoading)
                {
                    icon.sprite = iconsContainer.GetSprite(shortCode);
                    icon.SetNativeSize();
                }

                isInitlialised = true;
            }
        }

        private void OnBuyButtonClicked()
        {
            audioService.PlayStandardClick();
            buyButtonSignal.Dispatch(shortCode);
        }

        private void OnWatchAdButtonClicked()
        {
            audioService.PlayStandardClick();
            watchAdSignal.Dispatch(shortCode);
        }

        public void SetupPriceAndCount()
        {
            price.text = storeItem.currency3Cost.ToString();
            count.text = playerModel.GetInventoryItemCount(shortCode).ToString();
            buyButton.image.sprite = playerModel.gems >= storeItem.currency3Cost ? enoughGems : notEnoughGems;
        }
    }
}
