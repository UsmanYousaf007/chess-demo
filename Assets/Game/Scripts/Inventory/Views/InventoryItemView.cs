﻿using DG.Tweening;
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
        public ParticleSystem rewardBarParticleSystem;
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
        public string itemPointsShortCode;

        private static StoreIconsContainer iconsContainer;
        private static StoreThumbsContainer thumbsContainer;

        private bool isInitlialised = false;
        private StoreItem storeItem;
        private float rewardBarOriginalWidth;
        private Color originalColor;
        private Tweener addedAnimation;
        private bool haveEnoughGems;
        private bool isItemUnlockedByVideo;

        //Models
        [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }

        //Services
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public ILocalizationService localizationService { get; set; }

        //Dispatch Signals
        public Signal<string> buyButtonSignal = new Signal<string>();
        public Signal<InventoryVideoVO> watchAdSignal = new Signal<InventoryVideoVO>();
        public Signal notEnoughCurrencyToUnlockSignal = new Signal();

        protected override void OnEnable()
        {
            base.OnEnable();
            rewardBarParticleSystem.Stop();
        }

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
                watchAdText.text = localizationService.Get(LocalizationKey.INVENTORY_WATCH_AD);
                toolTipText.text = localizationService.Get(LocalizationKey.INVENTORY_TOOL_TIP);
                SetupPriceAndCount();
                SetupRewardBar();

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
            var vo = new InventoryVideoVO();
            vo.itemKey = shortCode;
            vo.itemPointsKey = itemPointsShortCode;
            watchAdSignal.Dispatch(vo);
        }

        public void SetupPriceAndCount()
        {
            price.text = storeItem.currency3Cost.ToString();
            count.text = playerModel.GetInventoryItemCount(shortCode).ToString();
            haveEnoughGems = playerModel.gems >= storeItem.currency3Cost;
            buyButton.image.sprite = haveEnoughGems ? enoughGems : notEnoughGems;
        }

        private void SetupRewardBar()
        {
            var currentPoints = playerModel.GetInventoryItemCount(itemPointsShortCode);
            var requiredPoints = settingsModel.GetInventorySpecialItemsRewardedVideoCost(shortCode);
            var barFillPercentage = (float)currentPoints / requiredPoints;
            rewardedVideoProgressBar.sizeDelta = new Vector2(rewardBarOriginalWidth * barFillPercentage, rewardedVideoProgressBar.sizeDelta.y);
            rewardedVideoProgressBarText.text = $"{currentPoints}/{requiredPoints}";
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

        public void ShowTooltip()
        {
            toolTip.SetActive(true);
        }

        public void OnRewardedPointAdded()
        {
            var to = playerModel.GetInventoryItemCount(itemPointsShortCode);
            var from = to - 1;
            var requiredPoints = settingsModel.GetInventorySpecialItemsRewardedVideoCost(shortCode);
            var barFillPercentageFrom = (float)from / requiredPoints;
            var barFillPercentageTo = (float)to / requiredPoints;
            var barValueFrom = rewardBarOriginalWidth * barFillPercentageFrom;
            var barValurTo = rewardBarOriginalWidth * barFillPercentageTo;
            rewardedVideoProgressBarText.text = $"{to}/{requiredPoints}";
            AnimateRewardedVideoProgressBar(barValueFrom, barValurTo);
        }

        public void OnItemUnclocked()
        {
            isItemUnlockedByVideo = true;
            var requiredPoints = settingsModel.GetInventorySpecialItemsRewardedVideoCost(shortCode);
            rewardedVideoProgressBarText.text = $"{requiredPoints}/{requiredPoints}";
            AnimateRewardedVideoProgressBar(rewardedVideoProgressBar.sizeDelta.x, rewardBarOriginalWidth);
        }

        private void AnimateRewardedVideoProgressBar(float from, float to)
        {
            audioService.Play(audioService.sounds.SFX_REWARD_UNLOCKED);

            if (!rewardBarParticleSystem.isPlaying)
            {
                rewardBarParticleSystem.Play();
            }

            iTween.ValueTo(this.gameObject,
                iTween.Hash(
                    "from", from,
                    "to", to,
                    "time", 2f,
                    "onupdate", "AnimateBar",
                    "onupdatetarget", this.gameObject,
                    "oncomplete", "AnimationComplete"
                ));
        }

        private void AnimationComplete()
        {
            rewardBarParticleSystem.Stop();

            if (isItemUnlockedByVideo)
            {
                isItemUnlockedByVideo = false;
                SetupRewardBar();
                PlayAnimation();
            }
        }

        private void AnimateBar(float value)
        {
            rewardedVideoProgressBar.sizeDelta = new Vector2(value, rewardedVideoProgressBar.sizeDelta.y);
        }

    }
}
