/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using HUFEXT.GenericGDPR.Runtime.API;
using TurboLabz.TLUtils;
using System;
using TMPro;
using System.Collections.Generic;
using System.Text;
using TurboLabz.InstantGame;
using TurboLabz.CPU;
using HUF.Analytics.API;

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

        public Text priceText;

        public Text personalizedAdsText;
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

        public Text personalisedAdsOnText;
        public Text personalisedAdsOffText;


        //Signals
        public Signal manageSubscriptionButtonClickedSignal = new Signal();
        public Signal upgradeToPremiumButtonClickedSignal = new Signal();
        public Signal restorePurchaseButtonClickedSignal = new Signal();

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IAdsService adsService { get; set; }

        //Models 
        [Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        public void Init()
        {
            //Set texts
            settingsTitleText.text = localizationService.Get(LocalizationKey.SETTINGS_TITLE);
            accountTitleText.text = localizationService.Get(LocalizationKey.SETTINGS_ACCOUNT_TITLE);
            backButtonText.text = localizationService.Get(LocalizationKey.BACK_TEXT);
           

            //Account
            manageSubscriptionText.text = localizationService.Get(LocalizationKey.SETTINGS_ACCOUNT_MANAGE_SUBSCRIPTION);
            restorePurchaseText.text = localizationService.Get(LocalizationKey.SUBSCRIPTION_DLG_RESTORE_PURCHASE);
            termsOfUseText.text = localizationService.Get(LocalizationKey.SUBSCRIPTION_DLG_TERMS_OF_USE);
            privacyPolicyText.text = localizationService.Get(LocalizationKey.SUBSCRIPTION_DLG_PRIVACY_POLICY);
            upgradeToPremiumText.text = localizationService.Get(LocalizationKey.SETTINGS_ACCOUNT_UPGRADE_TO_PREMIUM);
            personalizedAdsText.text = localizationService.Get(LocalizationKey.SETTINGS_ACCOUNT_PERSONALISED_ADS);


            personalisedAdsOnText.text = localizationService.Get(LocalizationKey.SETTINGS_ON);
            personalisedAdsOffText.text = localizationService.Get(LocalizationKey.SETTINGS_OFF);

            personalisedAdsOffBtn.onClick.AddListener(OnPersonalizedAdsOffButtonClicked);
            personalisedAdsOnBtn.onClick.AddListener(OnPersonalizedAdsOnButtonClicked);

            //Set Button Listeners
            manageSubscriptionBtn.onClick.AddListener(OnManageSubscriptionButtonClicked);
            upgradeToPremiumBtn.onClick.AddListener(OnUpgradeToPremiumButtonClicked);
            restorePurchaseBtn.onClick.AddListener(OnRestorePurchaseButtonClicked);
            termsOfUseBtn.onClick.AddListener(OnTermsOfUseButtonClicked);
            privacyPolicyBtn.onClick.AddListener(OnPrivacyPolicyButtonClicked);

            appVersion.text = "v" + Application.version;

            RefreshPersonalisedAdsToggleButtons();

#if UNITY_ANDROID
            restorePurchaseBtn.gameObject.SetActive(false);
#endif
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RefreshPersonalisedAdsToggleButtons();
        }

        public void SetSubscriptionPrice()
        {
            var storeItem = metaDataModel.store.items[key];

            if (storeItem == null)
                return;

            var isPremium = playerModel.HasSubscription();

            string subscriptionInfo = localizationService.Get(LocalizationKey.SETTINGS_ACCOUNT_INFO);
            //string subscriptionRenewDate = localizationService.Get(LocalizationKey.SETTINGS_ACCOUNT_RENEW);
            //string price = storeItem.remoteProductPrice;

            //string subscriptionPriceString = !isPremium ? subscriptionInfo.Replace("(price)", price) : subscriptionRenewDate.Replace("(date)", playerModel.renewDate);
            priceText.text = subscriptionInfo;
           
            upgradeToPremiumBtn.gameObject.SetActive(!isPremium);
            manageSubscriptionBtn.gameObject.SetActive(isPremium);
        }

        void OnManageSubscriptionButtonClicked()
        {
            Application.OpenURL(metaDataModel.settingsModel.manageSubscriptionURL);
            audioService.PlayStandardClick();
        }

        void OnUpgradeToPremiumButtonClicked()
        {
            upgradeToPremiumButtonClickedSignal.Dispatch();
            audioService.PlayStandardClick();
        }

        //Personalised Ads Button
        private void RefreshPersonalisedAdsToggleButtons()
        {
            personalisedAdsOffBtn.gameObject.SetActive(HGenericGDPR.IsPolicyAccepted != GDPRStatus.ACCEPTED);
            personalisedAdsOnBtn.gameObject.SetActive(HGenericGDPR.IsPolicyAccepted == GDPRStatus.ACCEPTED);
        }

        private void OnPersonalizedAdsOffButtonClicked()
        {
            audioService.PlayStandardClick();
            HGenericGDPR.IsPolicyAccepted = GDPRStatus.ACCEPTED;
            RefreshPersonalisedAdsToggleButtons();
            SetConsent();
        }

        private void OnPersonalizedAdsOnButtonClicked()
        {
            audioService.PlayStandardClick();
            HGenericGDPR.IsPolicyAccepted = GDPRStatus.TURNED_OFF;
            RefreshPersonalisedAdsToggleButtons();
            SetConsent();
        }

        private void SetConsent()
        {
            adsService.CollectSensitiveData(HGenericGDPR.IsPolicyAccepted == GDPRStatus.ACCEPTED);
            HAnalytics.CollectSensitiveData(HGenericGDPR.IsPolicyAccepted == GDPRStatus.ACCEPTED);
        }

        void OnRestorePurchaseButtonClicked()
        {
            restorePurchaseButtonClickedSignal.Dispatch();
            audioService.PlayStandardClick();
        }

        void OnTermsOfUseButtonClicked()
        {
            Application.OpenURL(metaDataModel.appInfo.termsOfUseURL);
            audioService.PlayStandardClick();
        }

        void OnPrivacyPolicyButtonClicked()
        {
            Application.OpenURL(metaDataModel.appInfo.privacyPolicyURL);
            audioService.PlayStandardClick();
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
    }

     
}
