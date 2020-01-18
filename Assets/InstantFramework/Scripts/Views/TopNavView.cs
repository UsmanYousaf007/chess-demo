/// @license Propriety <http://license.url>
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

        // Dispatch Signals
        [Inject] public ContactSupportSignal contactSupportSignal { get; set; }

        public Button selectThemeButton;
        public Text selectThemeText;
        public Button supportButton;
        public Button addBucksButton;
        public Button settingsButton;
        public Text playerBucks;
        public Text freeNoAdsPeriodLabel;
        public GameObject rewardUnlockedAlert;

        public Signal addBucksButtonClickedSignal = new Signal();
        public Signal removeAdsButtonClickedSignal = new Signal();
        public Signal settingsButtonClickedSignal = new Signal();
        public Signal selectThemeClickedSignal = new Signal();

        public void Init()
        {
            addBucksButton.onClick.AddListener(OnAddBucksButtonClicked);
            selectThemeText.text = localizationService.Get(LocalizationKey.SELECT_THEME);
            selectThemeButton.onClick.AddListener(OnSelectThemeClicked);
            supportButton.onClick.AddListener(OnSupportButtonClicked);
            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
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
            contactSupportSignal.Dispatch();
        }

        private void OnRemoveAdsButtonClicked()
        {
            removeAdsButtonClickedSignal.Dispatch();
        }

        private void OnSettingsButtonClicked()
        {
            Debug.Log("Dispatch Signal settings Btton Clicked:Top Nav View");
            settingsButtonClickedSignal.Dispatch();
        }

        private void OnSelectThemeClicked()
        {
            selectThemeClickedSignal.Dispatch();
            rewardUnlockedAlert.gameObject.SetActive(false);
        }

        public void OnRewardUnlocked(string key, int quantity)
        {
            //TODO show alert image in case theme is unlocked
        }
    }
}
