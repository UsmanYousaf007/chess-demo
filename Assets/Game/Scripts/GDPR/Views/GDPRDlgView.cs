/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using System;
using TMPro;
using HUFEXT.GenericGDPR.Runtime.Utils;

namespace TurboLabz.InstantFramework
{
    [CLSCompliant(false)]
    public class GDPRDlgView : View
    {
        public Button showRegularAdsBtn;
        public Text showRegularAdsText;

        public Button acceptAndCollectBtn;
        public TMP_Text acceptAndCollectText;

        public TMP_Text noteText;
        public TMP_Text withdrawText;

        public Text gemsCount;

        //Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IRewardsSettingsModel rewardsSettingsModel { get; set; }
        [Inject] public IMetaDataModel metaDataModel { get; set; }

        //Signals
        public Signal showRegularAdsBtnClickedSignal = new Signal();
        public Signal acceptAndCollectBtnClickedSignal = new Signal();

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IBackendService backendService { get; set; }


        public void InitOnce()
        {
            showRegularAdsBtn.onClick.AddListener(OnShowRegularAdsBtnClicked);
            acceptAndCollectBtn.onClick.AddListener(OnAcceptAndCollectBtnClicked);
        }

        public void OnServerDataAvailable()
        {
            gemsCount.text = rewardsSettingsModel.personalisedAdsGemReward + " Gems";
            noteText.text = "I hereby consent to the usage and disclosure of my personal data (including device information and my preferences) to <link=" +
            GDPRTranslationsProvider.AD_PARTNERS_LINK + "><b><u>advertising network companies</u></b></link> for the purpose of serving targeted advertisements to me in the game.";

            withdrawText.text = " I understand that I can withdraw this consent at any time within the game Settings, as also described in our <link=" +
           GDPRTranslationsProvider.PRIVACY_POLICY_LINK + "><b><u>Privacy Policy</u></b></link>.";
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnShowRegularAdsBtnClicked()
        {
            showRegularAdsBtnClickedSignal.Dispatch();
        }

        public void OnAcceptAndCollectBtnClicked()
        {
            acceptAndCollectBtnClickedSignal.Dispatch();
        }
    }
}
