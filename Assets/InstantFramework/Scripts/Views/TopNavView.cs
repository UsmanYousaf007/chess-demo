﻿/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


using UnityEngine.UI;

using strange.extensions.mediation.impl;
using UnityEngine;
using System.Collections;
using TurboLabz.TLUtils;
using strange.extensions.signal.impl;
using TurboLabz.InstantGame;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public class TopNavView : View
    {
        // Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }
        [Inject] public IMetaDataModel metaDataModel { get; set; }

        public Button selectThemeButton;
        public Text selectThemeText;
        public Button supportButton;
        public Button addBucksButton;
        public Button settingsButton;
        public Text playerBucks;
        public Text freeNoAdsPeriodLabel;
        public GameObject rewardUnlockedAlert;
        public RectTransform rewardBar;
        public GameObject rewardBarObject;
        public Button rewardBarButton;
        public ParticleSystem particlesSys;

        public Signal addBucksButtonClickedSignal = new Signal();
        public Signal removeAdsButtonClickedSignal = new Signal();
        public Signal settingsButtonClickedSignal = new Signal();
        public Signal selectThemeClickedSignal = new Signal();
        public Signal rewardBarClicked = new Signal();
        public Signal supportButtonClicked = new Signal();

        private float rewardBarOriginalWidth;
        private Vector3 scaleRewardBarObjectTo = new Vector3(1.3f, 1.3f, 1);

        public void Init()
        {
            addBucksButton.onClick.AddListener(OnAddBucksButtonClicked);
            selectThemeText.text = localizationService.Get(LocalizationKey.SELECT_THEME);
            selectThemeButton.onClick.AddListener(OnSelectThemeClicked);
            supportButton.onClick.AddListener(OnSupportButtonClicked);
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            rewardBarButton.onClick.AddListener(OnRewardBarClicked);
            rewardBarOriginalWidth = rewardBar.sizeDelta.x;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            particlesSys.Stop();
        }

        public void UpdatePlayerBucks(long bucks)
        {
            playerBucks.text = bucks.ToString();
        }

        public void UpdateRemoveAds(string freePeriod, bool isFreePeriod)
        {
            freeNoAdsPeriodLabel.gameObject.SetActive(false);
            if (isFreePeriod)
            {
                freeNoAdsPeriodLabel.gameObject.SetActive(true);
                freeNoAdsPeriodLabel.text = localizationService.Get(LocalizationKey.FREE_NO_ADS_PERIOD);
                freeNoAdsPeriodLabel.text += " " + freePeriod;
            }
        }

        public void OnStoreAvailable(bool isAvailable)
        {
            addBucksButton.interactable = isAvailable;
        }

        private void OnAddBucksButtonClicked()
        {
            addBucksButtonClickedSignal.Dispatch();
            analyticsService.Event(AnalyticsEventId.tap_coins);
        }

        private void OnSupportButtonClicked()
        {
            supportButtonClicked.Dispatch();
        }

        private void OnRemoveAdsButtonClicked()
        {
            removeAdsButtonClickedSignal.Dispatch();
        }

        private void OnSettingsButtonClicked()
        {
            Debug.Log("Dispatch Signal settings Btton Clicked:Top Nav View");
            audioService.PlayStandardClick();
            settingsButtonClickedSignal.Dispatch();
        }

        private void OnSelectThemeClicked()
        {
            audioService.PlayStandardClick();
            selectThemeClickedSignal.Dispatch();   
        }

        public void OnRewardUnlocked(string key, int quantity)
        {
            var reward = metaDataModel.store.items[key];

            if (reward != null && reward.kind.Equals(GSBackendKeys.ShopItem.SKIN_SHOP_TAG))
            {
                rewardUnlockedAlert.SetActive(true);
            }
        }

        public void SetupRewardBar()
        {
            ShowRewardBar();
            var barFillPercentage = playerModel.rewardCurrentPoints / playerModel.rewardPointsRequired;
            rewardBar.sizeDelta = new Vector2(rewardBarOriginalWidth * barFillPercentage, rewardBar.sizeDelta.y);
        }

        public void ShowRewardBar()
        {
            rewardBarObject.SetActive(!playerModel.HasSubscription());
        }

        public void AnimateRewardBar(float from, float to)
        {
            var barFillPercentageFrom = from / playerModel.rewardPointsRequired;
            var barFillPercentageTo = to / playerModel.rewardPointsRequired;

            if (from >= to)
            {
                rewardBar.sizeDelta = new Vector2(rewardBarOriginalWidth * barFillPercentageTo, rewardBar.sizeDelta.y);
                return;
            }

            if (!particlesSys.isPlaying)
            {
                particlesSys.Play();
            }

            iTween.ValueTo(this.gameObject,
                iTween.Hash(
                    "from", rewardBarOriginalWidth * barFillPercentageFrom,
                    "to", rewardBarOriginalWidth * barFillPercentageTo,
                    "time", 2f,
                    "onupdate", "AnimateBar",
                    "onupdatetarget", this.gameObject, "oncomplete", "StopPlayingParticles"
                ));
        }


        private void StopPlayingParticles()
        {
            particlesSys.Stop();
        }

        private void AnimateBar(float value)
        {
            rewardBar.sizeDelta = new Vector2(value, rewardBar.sizeDelta.y);
        }

        private void OnRewardBarClicked()
        {
            audioService.PlayStandardClick();
            rewardBarClicked.Dispatch();
        }
    }
}
