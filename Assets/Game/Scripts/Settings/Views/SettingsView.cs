/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using HUFEXT.GenericGDPR.Runtime.API;
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

        //Sound
        public Button audioOffButton;
        public Button audioOnButton;
        public Text soundText;
        public Text soundEffectsText;
        public Text soundOnText;
        public Text soundOffText;

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
            soundText.text = localizationService.Get(LocalizationKey.SETTINGS_SOUND_TITLE);
            soundEffectsText.text = localizationService.Get(LocalizationKey.SETTINGS_SOUND_EFFECT);
            soundOnText.text = localizationService.Get(LocalizationKey.SETTINGS_ON);
            soundOffText.text = localizationService.Get(LocalizationKey.SETTINGS_OFF);

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
            restorePurchaseBtn.onClick.AddListener(OnRestorePurchaseButtonClicked);
            termsOfUseBtn.onClick.AddListener(OnTermsOfUseButtonClicked);
            privacyPolicyBtn.onClick.AddListener(OnPrivacyPolicyButtonClicked);
            audioOffButton.onClick.AddListener(OnAudioOffButtonClicked);
            audioOnButton.onClick.AddListener(OnAudioOnButtonClicked);
            personalisedAdsOffBtn.onClick.AddListener(OnPersonalizedAdsOffButtonClicked);
            personalisedAdsOnBtn.onClick.AddListener(OnPersonalizedAdsOnButtonClicked);

            appVersion.text = "v" + Application.version;

            RefreshAudioButtons();
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

            var isSubscriber = playerModel.HasSubscription();

            string subscriptionInfo = localizationService.Get(LocalizationKey.SETTINGS_ACCOUNT_INFO);
            string subscriptionRenewDate = localizationService.Get(LocalizationKey.SETTINGS_ACCOUNT_RENEW);
            
            priceText.text = isSubscriber ? subscriptionRenewDate.Replace("(date)", playerModel.renewDate) : subscriptionInfo;
           
            upgradeToPremiumBtn.gameObject.SetActive(!isSubscriber);
            manageSubscriptionBtn.gameObject.SetActive(isSubscriber && !playerModel.isPremium);
            priceText.gameObject.SetActive((isSubscriber && !playerModel.isPremium) || !isSubscriber);
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

        private void OnAudioOffButtonClicked()
        {
            audioService.ToggleAudio(true);
            audioService.PlayStandardClick();
            RefreshAudioButtons();
        }

        private void OnAudioOnButtonClicked()
        {
            audioService.ToggleAudio(false);
            RefreshAudioButtons();
        }

        private void RefreshAudioButtons()
        {
            audioOffButton.gameObject.SetActive(!audioService.IsAudioOn());
            audioOnButton.gameObject.SetActive(audioService.IsAudioOn());
        }
    }
}
