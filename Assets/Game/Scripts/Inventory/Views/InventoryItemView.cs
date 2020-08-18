using DG.Tweening;
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
        public Text addedCount;

        private static StoreIconsContainer iconsContainer;
        private static StoreThumbsContainer thumbsContainer;

        private bool isInitlialised = false;
        private StoreItem storeItem;
        private float rewardBarOriginalWidth;
        private Color originalColor;
        private Tweener addedAnimation;
        private bool haveEnoughGems;

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
        public Signal notEnoughCurrencyToUnlockSignal = new Signal();

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

            addedCount.gameObject.SetActive(false);
            originalColor = addedCount.color;
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

            if (haveEnoughGems)
            {
                buyButtonSignal.Dispatch(shortCode);
            }
            else
            {
                notEnoughCurrencyToUnlockSignal.Dispatch();
            }
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
            haveEnoughGems = playerModel.gems >= storeItem.currency3Cost;
            buyButton.image.sprite = haveEnoughGems ? enoughGems : notEnoughGems;
        }

        public void PlayAnimation()
        {
            if (addedAnimation != null)
            {
                addedAnimation.Kill();
                DOTween.Kill(addedCount.transform);
            }

            addedCount.transform.localPosition = Vector3.zero;
            addedCount.color = originalColor;
            addedCount.gameObject.SetActive(true);
            addedAnimation = DOTween.ToAlpha(() => addedCount.color, x => addedCount.color = x, 0.0f, 3.0f).OnComplete(OnFadeComplete);
            addedCount.transform.DOMoveY(addedCount.transform.position.y + 100, 3.0f);
        }

        private void OnFadeComplete()
        {
            addedCount.gameObject.SetActive(false);
        }
    }
}
