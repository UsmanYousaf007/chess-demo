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
using DG.Tweening;
using System.Collections;

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

        public RectTransform gems;
        public TextMeshProUGUI textGems;
        public Transform startPivot;
        public Transform endPivot;

        [Tooltip("Color to fade from")]
        [SerializeField]
        private Color StartColor = Color.white;

        //Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IRewardsSettingsModel rewardsSettingsModel { get; set; }
        [Inject] public IMetaDataModel metaDataModel { get; set; }

        //Signals
        public Signal showRegularAdsBtnClickedSignal = new Signal();
        public Signal acceptAndCollectBtnClickedSignal = new Signal();
        public Signal onGDPRDlgClosedSignal = new Signal();

        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public GDPRDlgClosedSignal gdprDlgClosedSignal { get; set; }

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IBackendService backendService { get; set; }

        public void InitOnce()
        {
            showRegularAdsBtn.onClick.AddListener(OnShowRegularAdsBtnClicked);
            acceptAndCollectBtn.onClick.AddListener(OnAcceptAndCollectBtnClicked);

            noteText.text = "I hereby consent to the usage and disclosure of my personal data (including device information, advertising ID and my preferences) to​ <link=" +
                GDPRTranslationsProvider.AD_PARTNERS_LINK + "><b><u>advertising network companies</u></b></link> for the purpose of serving tailored advertisements to me in the game.";

            withdrawText.text = "I understand that I can withdraw this consent at any time within the game settings, as also described in our​ <link=" +
                GDPRTranslationsProvider.PRIVACY_POLICY_LINK + "><b><u>Privacy Policy</u></b></link>.";

            noteText.gameObject.AddComponent<Hyperlink>();
            withdrawText.gameObject.AddComponent<Hyperlink>();
        }

        public void OnServerDataAvailable()
        {
            gemsCount.text = rewardsSettingsModel.personalisedAdsGemReward + " Gems";
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

        public void OnAcceptAndCollectBtnClickedPostProcessing()
        {
            acceptAndCollectBtn.interactable = false;
            showRegularAdsBtn.interactable = false;
            //GemsAddedAnimation();
        }

        private void GemsAddedAnimation()
        {
            textGems.text = "+" + rewardsSettingsModel.personalisedAdsGemReward;
            audioService.Play(audioService.sounds.SFX_REWARD_UNLOCKED);
            textGems.color = StartColor;
            textGems.gameObject.transform.position = startPivot.position;
            gems.gameObject.SetActive(true);
            StartCoroutine(GemsAddedCR());
        }

        IEnumerator GemsAddedCR()
        {
            yield return new WaitForSeconds(0.05f);

            textGems.DOFade(0f, 4.5f);
            textGems.transform.DOMoveY(endPivot.position.y, 4.5f);

            yield return new WaitForSeconds(4.5f);

            gems.gameObject.SetActive(false);
            onGDPRDlgClosedSignal.Dispatch();
        }

    }
}
