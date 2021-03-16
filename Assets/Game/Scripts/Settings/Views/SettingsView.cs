/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

#if UNITY_IOS
using HUF.Ads.Runtime.API;
using TurboLabz.InstantGame;
#endif

namespace TurboLabz.InstantFramework
{
    [System.CLSCompliant(false)]
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
        //public Text soundText;
        public Text soundEffectsText;
        public Text soundOnText;
        public Text soundOffText;

        //Account
        //public Text accountTitleText;

        public Text priceText;

        public Text personalizedAdsText;
        public Button personalisedAdsOnBtn;
        public Button personalisedAdsOffBtn;
        public Text personalisedAdsOnText;
        public Text personalisedAdsOffText;
        public Image personalisedAdsOffBall;

        public Text autoConvertPawntoQueenText;
        public Button autoConvertPawntoQueenOnBtn;
        public Button autoConvertPawntoQueenOffBtn;
        public Text autoConvertPawntoQueenOnBtnText;
        public Text autoConvertPawntoQueenOffBtnText;

        public Button manageSubscriptionBtn;
        public Text manageSubscriptionText;

        public Button restorePurchaseBtn;
        public Text restorePurchaseText;

        public Button termsOfUseBtn;
        public Text termsOfUseText;

        public Button privacyPolicyBtn;
        public Text privacyPolicyText;

        public Button FAQBtn;
        public Text FAQText;

        public Button upgradeToPremiumBtn;
        public Text upgradeToPremiumText;

        public Button chatOnDiscordBtn;
        public Text chatOnDiscordText;

        //Signals
        public Signal manageSubscriptionButtonClickedSignal = new Signal();
        public Signal upgradeToPremiumButtonClickedSignal = new Signal();
        public Signal restorePurchaseButtonClickedSignal = new Signal();
        public Signal applySettingsSignal = new Signal();

        [Inject] public ShowBottomNavSignal showBottomNavSignal { get; set; }

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IAdsService adsService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }

        //Models 
        [Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }

        bool settingsChanged = false;

        public void Init()
        {
            //Set texts
            settingsTitleText.text = localizationService.Get(LocalizationKey.SETTINGS_TITLE);
            //accountTitleText.text = localizationService.Get(LocalizationKey.SETTINGS_ACCOUNT_TITLE);
            backButtonText.text = localizationService.Get(LocalizationKey.BACK_TEXT);
            //soundText.text = localizationService.Get(LocalizationKey.SETTINGS_SOUND_TITLE);
            soundEffectsText.text = localizationService.Get(LocalizationKey.SETTINGS_SOUND_EFFECT);
            soundOnText.text = localizationService.Get(LocalizationKey.ON_TEXT);
            soundOffText.text = localizationService.Get(LocalizationKey.OFF_TEXT);

            //Account
            manageSubscriptionText.text = localizationService.Get(LocalizationKey.SETTINGS_ACCOUNT_MANAGE_SUBSCRIPTION);
            restorePurchaseText.text = localizationService.Get(LocalizationKey.SUBSCRIPTION_DLG_RESTORE_PURCHASE);
            termsOfUseText.text = localizationService.Get(LocalizationKey.SUBSCRIPTION_DLG_TERMS_OF_USE);
            privacyPolicyText.text = localizationService.Get(LocalizationKey.SUBSCRIPTION_DLG_PRIVACY_POLICY);
            upgradeToPremiumText.text = localizationService.Get(LocalizationKey.SETTINGS_ACCOUNT_UPGRADE_TO_PREMIUM);
            chatOnDiscordText.text = localizationService.Get(LocalizationKey.SETTINGS_CHAT_ON_DISCORD);
            personalizedAdsText.text = localizationService.Get(LocalizationKey.SETTINGS_ACCOUNT_PERSONALISED_ADS);
            personalisedAdsOnText.text = localizationService.Get(LocalizationKey.ON_TEXT);
            personalisedAdsOffText.text = localizationService.Get(LocalizationKey.OFF_TEXT);
            FAQText.text = localizationService.Get(LocalizationKey.SETTINGS_FAQ);
            autoConvertPawntoQueenOnBtnText.text = localizationService.Get(LocalizationKey.ON_TEXT);
            autoConvertPawntoQueenOffBtnText.text = localizationService.Get(LocalizationKey.OFF_TEXT);

            //Set Button Listeners
            manageSubscriptionBtn.onClick.AddListener(OnManageSubscriptionButtonClicked);
            upgradeToPremiumBtn.onClick.AddListener(OnUpgradeToPremiumButtonClicked);
            chatOnDiscordBtn.onClick.AddListener(OnChatOnDiscordButtonClicked);
            restorePurchaseBtn.onClick.AddListener(OnRestorePurchaseButtonClicked);
            termsOfUseBtn.onClick.AddListener(OnTermsOfUseButtonClicked);
            privacyPolicyBtn.onClick.AddListener(OnPrivacyPolicyButtonClicked);
            audioOffButton.onClick.AddListener(OnAudioOffButtonClicked);
            audioOnButton.onClick.AddListener(OnAudioOnButtonClicked);
            personalisedAdsOffBtn.onClick.AddListener(OnPersonalizedAdsOffButtonClicked);
            personalisedAdsOnBtn.onClick.AddListener(OnPersonalizedAdsOnButtonClicked);
            FAQBtn.onClick.AddListener(OnFAQButtonClicked);
            autoConvertPawntoQueenOffBtn.onClick.AddListener(OnAutoConvertPawntoQueenOffButtonClicked);
            autoConvertPawntoQueenOnBtn.onClick.AddListener(OnAutoConvertPawntoQueenOffButtonClicked);

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
            RefreshAudioButtons();
            RefreshPersonalisedAdsToggleButtons();
            RefreshAutoConvertPawntoQueenButtons();
            //settingsChanged = playerModel.autoPromotionToQueen;
        }

        public void SetSubscriptionPrice()
        {
            var storeItem = metaDataModel.store.items[key];

            if (storeItem == null)
                return;

            var isSubscriber = playerModel.HasSubscription();

            string subscriptionInfo = localizationService.Get(LocalizationKey.SETTINGS_ACCOUNT_INFO);
            string subscriptionRenewDate = localizationService.Get(LocalizationKey.SETTINGS_ACCOUNT_RENEW);
            
            //priceText.text = isSubscriber ? subscriptionRenewDate.Replace("(date)", playerModel.renewDate) : subscriptionInfo;
           
            upgradeToPremiumBtn.gameObject.SetActive(!isSubscriber);
            manageSubscriptionBtn.gameObject.SetActive(isSubscriber && !playerModel.isPremium);
            //priceText.gameObject.SetActive((isSubscriber && !playerModel.isPremium) || !isSubscriber);
        }

        void OnManageSubscriptionButtonClicked()
        {
            audioService.PlayStandardClick();
            manageSubscriptionButtonClickedSignal.Dispatch();
        }

        void OnUpgradeToPremiumButtonClicked()
        {
            upgradeToPremiumButtonClickedSignal.Dispatch();
            audioService.PlayStandardClick();
        }

        void OnChatOnDiscordButtonClicked()
        {
            hAnalyticsService.LogEvent("clicked", "settings", "", "link_discord");
            audioService.PlayStandardClick();
            Application.OpenURL(metaDataModel.appInfo.chatOnDiscordURL);
        }

        //Personalised Ads Button
        private void RefreshPersonalisedAdsToggleButtons()
        {
            var personalisedAdsStatus = adsService.GetAdsConsent();
            personalisedAdsOffBtn.gameObject.SetActive(!personalisedAdsStatus);
            personalisedAdsOnBtn.gameObject.SetActive(personalisedAdsStatus);
        }

        private void OnPersonalizedAdsOffButtonClicked()
        {
            audioService.PlayStandardClick();
            adsService.CollectSensitiveData(true);
            RefreshPersonalisedAdsToggleButtons();
            hAnalyticsService.LogEvent("turn_on", "settings", "", "personalised_ads");
        }

        private void OnPersonalizedAdsOnButtonClicked()
        {
            audioService.PlayStandardClick();
            adsService.CollectSensitiveData(false);
            RefreshPersonalisedAdsToggleButtons();
            hAnalyticsService.LogEvent("turn_off", "settings", "", "personalised_ads");
        }

        void OnRestorePurchaseButtonClicked()
        {
            restorePurchaseButtonClickedSignal.Dispatch();
            audioService.PlayStandardClick();
        }

        void OnTermsOfUseButtonClicked()
        {
            hAnalyticsService.LogEvent("terms_clicked", "settings", "", "terms");
            audioService.PlayStandardClick();
            Application.OpenURL(metaDataModel.appInfo.termsOfUseURL);
        }

        void OnPrivacyPolicyButtonClicked()
        {
            hAnalyticsService.LogEvent("clicked", "settings", "", "privacy_policy");
            audioService.PlayStandardClick();
            Application.OpenURL(metaDataModel.appInfo.privacyPolicyURL);
        }

        void OnFAQButtonClicked()
        {
            audioService.PlayStandardClick();
            Application.OpenURL(metaDataModel.appInfo.faqURL);
        }

        public void Show()
        {
            showBottomNavSignal.Dispatch(false);
            gameObject.SetActive(true);

#if UNITY_IOS
            var canChangeAdsConsent = HAds.CanChangeAdsConsent();
            var selectedColor = canChangeAdsConsent ? Colors.WHITE : Colors.DISABLED_WHITE;
            personalisedAdsOffBtn.interactable = canChangeAdsConsent;
            personalisedAdsOffText.color = selectedColor;
            personalisedAdsOffBall.color = selectedColor;
#endif
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            applySettingsSignal.Dispatch();
        }

        public void OnBackButtonClicked()
        {

        }

        private void OnAudioOffButtonClicked()
        {
            audioService.ToggleAudio(true);
            audioService.PlayStandardClick();
            RefreshAudioButtons();
            hAnalyticsService.LogEvent("turn_on", "settings", "", "sounds");
        }

        private void OnAudioOnButtonClicked()
        {
            audioService.ToggleAudio(false);
            RefreshAudioButtons();
            hAnalyticsService.LogEvent("turn_off", "settings", "", "sounds");
        }

        private void RefreshAudioButtons()
        {
            audioOffButton.gameObject.SetActive(!audioService.IsAudioOn());
            audioOnButton.gameObject.SetActive(audioService.IsAudioOn());
        }

        private void OnAutoConvertPawntoQueenOffButtonClicked()
        {
            //playerModel.autoPromotionToQueen = !playerModel.autoPromotionToQueen;
            preferencesModel.autoPromotionToQueen = !preferencesModel.autoPromotionToQueen;
            audioService.PlayStandardClick();
            RefreshAutoConvertPawntoQueenButtons();
        }

        private void OnAutoConvertPawntoQueenOnButtonClicked()
        {
            audioService.PlayStandardClick();
        }

        private void RefreshAutoConvertPawntoQueenButtons()
        {
            //autoConvertPawntoQueenOffBtn.gameObject.SetActive(!playerModel.autoPromotionToQueen);
            //autoConvertPawntoQueenOnBtn.gameObject.SetActive(playerModel.autoPromotionToQueen);

            autoConvertPawntoQueenOffBtn.gameObject.SetActive(!preferencesModel.autoPromotionToQueen);
            autoConvertPawntoQueenOnBtn.gameObject.SetActive(preferencesModel.autoPromotionToQueen);
        }
    }
}
