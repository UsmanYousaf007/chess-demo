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

namespace TurboLabz.InstantFramework
{
    public class TopNavView : View
    {
        // Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        public Button shareButton;
        public Button audioOffButton;
        public Button audioOnButton;
        public Button supportButton;
        public Button addBucksButton;
        public Text playerBucks;
        public Text freeNoAdsPeriodLabel;

        public Signal shareAppButtonClickedSignal = new Signal();
        public Signal addBucksButtonClickedSignal = new Signal();
        public Signal removeAdsButtonClickedSignal = new Signal();

        public void Init()
        {
            shareButton.onClick.AddListener(OnShareAppButtonClicked);
            addBucksButton.onClick.AddListener(OnAddBucksButtonClicked);
            audioOffButton.onClick.AddListener(OnAudioOffButtonClicked);
            audioOnButton.onClick.AddListener(OnAudioOnButtonClicked);
            supportButton.onClick.AddListener(OnSupportButtonClicked);

            RefreshAudioButtons();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RefreshAudioButtons();
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

        private void OnShareAppButtonClicked()
        {
            shareAppButtonClickedSignal.Dispatch();
        }

        private void OnAddBucksButtonClicked()
        {
            addBucksButtonClickedSignal.Dispatch();
        }

        private void OnAudioOffButtonClicked()
        {
            audioService.ToggleAudio(false);
            RefreshAudioButtons();
        }

        private void OnAudioOnButtonClicked()
        {
            audioService.ToggleAudio(true);
            audioService.PlayStandardClick();
            RefreshAudioButtons();
        }

        private void RefreshAudioButtons()
        {
            audioOffButton.gameObject.SetActive(audioService.IsAudioOn());
            audioOnButton.gameObject.SetActive(!audioService.IsAudioOn());
        }

        private void OnSupportButtonClicked()
        {
            analyticsService.TapHelp();

            Application.OpenURL("mailto:" + Settings.SUPPORT_EMAIL);
        }

        private void OnRemoveAdsButtonClicked()
        {
            removeAdsButtonClickedSignal.Dispatch();
        }
    }
}
