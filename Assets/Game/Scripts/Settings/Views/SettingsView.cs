﻿/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.TLUtils;
using System;
using TMPro;
using System.Collections.Generic;
using System.Text;
using TurboLabz.InstantGame;
using TurboLabz.CPU;

namespace TurboLabz.InstantFramework
{
    public class SettingsView : View
    {
        public string key;

        public Button backButton;
        public Text backButtonText;
        public Text settingsTitleText;
        public Text appVersion;

        //Account
        public Text accountTitleText;

        public Text infoText;
        public Button personalisedAdsOnBtn;
        public Button personalisedAdsOffBtn;

        public Button manageSubscriptionBtn;
        public Text manageSubscriptionText;

        public Button restorePurchaseBtn;
        public Text restorePurchaseText;

        public Button termsOfUseBtn;
        public Text termsOfUseText;

        public Button privacyPolicyBtn;
        public Text privacyPolicyText;

        public Button upgradeToPremiumBtn;
        public Text upgradeToPremiumText;

        public Button personalizedAdsBtn;
        public Text personalizedAdsText;

        public Text personalisedAdsOnText;
        public Text personalisedAdsOffText;


        //Signals
        public Signal manageSubscriptionButtonClickedSignal = new Signal();
        public Signal upgradeToPremiumButtonClickedSignal = new Signal();
        public Signal personalizedAdsButtonClickedSignal = new Signal();
        public Signal restorePurchaseButtonClickedSignal = new Signal();

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        //Models 
        [Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        public void Init()
        {
            //Set texts
            settingsTitleText.text = localizationService.Get(LocalizationKey.SETTINGS_TITLE);
            accountTitleText.text = localizationService.Get(LocalizationKey.SETTINGS_ACCOUNT_TITLE);
            backButtonText.text = localizationService.Get(LocalizationKey.LONG_PLAY_BACK_TO_GAME);
           

            //Account
            manageSubscriptionText.text = localizationService.Get(LocalizationKey.SETTINGS_ACCOUNT_MANAGE_SUBSCRIPTION);
            restorePurchaseText.text = localizationService.Get(LocalizationKey.SUBSCRIPTION_DLG_RESTORE_PURCHASE);
            termsOfUseText.text = localizationService.Get(LocalizationKey.SUBSCRIPTION_DLG_TERMS_OF_USE);
            privacyPolicyText.text = localizationService.Get(LocalizationKey.SUBSCRIPTION_DLG_PRIVACY_POLICY);
            upgradeToPremiumText.text = localizationService.Get(LocalizationKey.SETTINGS_ACCOUNT_UPGRADE_TO_PREMIUM);
            personalizedAdsText.text = localizationService.Get(LocalizationKey.SETTINGS_ACCOUNT_PERSONALISED_ADS);


            personalisedAdsOnText.text = localizationService.Get(LocalizationKey.SETTINGS_ON);
            personalisedAdsOffText.text = localizationService.Get(LocalizationKey.SETTINGS_OFF);

            //Set Button Listeners
            manageSubscriptionBtn.onClick.AddListener(OnManageSubscriptionButtonClicked);
            upgradeToPremiumBtn.onClick.AddListener(OnUpgradeToPremiumButtonClicked);
            personalizedAdsBtn.onClick.AddListener(OnPersonalizedAdsButtonClicked);
            restorePurchaseBtn.onClick.AddListener(OnRestorePurchaseButtonClicked);
            termsOfUseBtn.onClick.AddListener(OnTermsOfUseButtonClicked);
            privacyPolicyBtn.onClick.AddListener(OnPrivacyPolicyButtonClicked);

            appVersion.text = "v" + Application.version;

            RefreshPersonalisedAdsToggleButtons();
        }

        void RefreshAccountPanel()
        {
        }

        public void SetSubscriptionPrice()
        {
            var storeItem = metaDataModel.store.items[key];

            if (storeItem == null)
                return;

            string subscriptionInfo = localizationService.Get(LocalizationKey.SETTINGS_ACCOUNT_INFO);
            string price = storeItem.remoteProductPrice;

            string subscriptionPriceString = subscriptionInfo.Replace("(price)", price);
            infoText.text = subscriptionPriceString;

            var isPremium = playerModel.HasSubscription();
            infoText.gameObject.SetActive(!isPremium);
            upgradeToPremiumBtn.gameObject.SetActive(!isPremium);
            manageSubscriptionBtn.gameObject.SetActive(isPremium);
        }

        void OnManageSubscriptionButtonClicked()
        {
            Application.OpenURL(metaDataModel.settingsModel.manageSubscriptionURL);
            audioService.Play(audioService.sounds.SFX_STEP_CLICK);
        }

        void OnUpgradeToPremiumButtonClicked()
        {
            upgradeToPremiumButtonClickedSignal.Dispatch();
            audioService.Play(audioService.sounds.SFX_STEP_CLICK);
        }

        void OnPersonalizedAdsButtonClicked()
        {
            personalizedAdsButtonClickedSignal.Dispatch();
            audioService.Play(audioService.sounds.SFX_STEP_CLICK);
        }

        void OnRestorePurchaseButtonClicked()
        {
            restorePurchaseButtonClickedSignal.Dispatch();
            audioService.Play(audioService.sounds.SFX_STEP_CLICK);
        }

        void OnTermsOfUseButtonClicked()
        {
            Application.OpenURL(metaDataModel.appInfo.termsOfUseURL);
            audioService.Play(audioService.sounds.SFX_STEP_CLICK);
        }

        void OnPrivacyPolicyButtonClicked()
        {
            Application.OpenURL(metaDataModel.appInfo.privacyPolicyURL);
            audioService.Play(audioService.sounds.SFX_STEP_CLICK);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnBackButtonClicked()
        {

        }

        private void RefreshPersonalisedAdsToggleButtons()
        {
            personalisedAdsOffBtn.gameObject.SetActive(false);
            personalisedAdsOnBtn.gameObject.SetActive(true);
        }
    }

     
}
