/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using System.Collections.Generic;
using TMPro;
using System;

namespace TurboLabz.InstantGame
{
    [CLSCompliant(false)]
    public class StatsView : View
    {
        // Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }

        // Scene references
        //Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }
        [Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public ITournamentsModel tournamentsModel { get; set; }

        [Inject] public ChangeUserDetailsSignal changeUserDetailsSignal { get; set; }
        [Inject] public ShowBottomNavSignal showBottomNavSignal { get; set; }

        public Text appVersion;

        public Text onlineTitle;
        public Text onlineWinPct;
        public Text onlineWon;
        public Text onlineLost;
        public Text onlineDrawn;
        public Text onlineTotal;

        public Text onlineWinPctVal;
        public Text onlineWonVal;
        public Text onlineLostVal;
        public Text onlineDrawnVal;
        public Text onlineTotalVal;

        public Text computerTitle;
        public Text legendGold;
        public Text legendSilver;

        public Image[] stars;
        public Sprite noStar;
        public Sprite silverStar;
        public Sprite goldStar;

        public Button backButton;

        public TMP_InputField playerProfileNameInputField;
        public Button nameEditBtn;

        public Text tagLabel;
        public Text playerTag;
        public Text country;
        public Text playingSince;
        public Button shareBtn;
        public Button copyTagBtn;
        public Button profilePicBtn;
        public Image copiedToClipboardBg;
        public Text copiedToClipboardText;
        public Texture2D logo;

        public Button supportButton;
        public Button settingsButton;

        public Image playerTitleImg;
        public Text currentTrophies;

        public GameObject uiBlocker;
        public GameObject processingUi;
 
        public Signal settingsButtonClickedSignal = new Signal();
        public Signal supportButtonClicked = new Signal();

        [Header("Name Confirm Dialog")]
        public GameObject nameConfirmDlg;
        public Button nameConfirmDlgYesBtn;
        public Button nameConfirmDlgNoBtn;
        public Text nameConfirmDlgYesBtnText;
        public Text nameConfirmDlgTitleText;
        public Text nameConfirmDlgSubheadingText;

        [Header("Profile Pic Change Dialog")]
        public GameObject picUpdateDlg;
        public Button takePhotoBtn;
        public Button choosePhotoBtn;
        public Button closePhotoBtn;
        public Text takePhotoText;
        public Text choosePhotoText;
        public Text phototitleTxt;

        [Header("Open Settings Dialog")]
        public GameObject openSettingsDlg;
        public Button openPhotoSettingsBtn;
        public Button closeSettingsDlgBtn;
        public Text openSettingsBtnText;
        public Text openSettingsDlgTitleText;
        public Text openSettingsDlgSubheadingText;

        [Header("Links")]
        public Button restorePurchaseBtn;
        public Text restorePurchaseText;

        public Button termsOfUseBtn;
        public Text termsOfUseText;

        public Button privacyPolicyBtn;
        public Text privacyPolicyText;

        public Signal restorePurchaseButtonClickedSignal = new Signal();

        public void Init()
        {
            onlineTitle.text = localizationService.Get(LocalizationKey.STATS_ONLINE_TITLE);
            onlineWinPct.text = localizationService.Get(LocalizationKey.STATS_ONLINE_WIN_PCT);
            onlineWon.text = localizationService.Get(LocalizationKey.STATS_ONLINE_WON);
            onlineLost.text = localizationService.Get(LocalizationKey.STATS_ONLINE_LOST);
            onlineDrawn.text = localizationService.Get(LocalizationKey.STATS_ONLINE_DRAWN);
            onlineTotal.text = localizationService.Get(LocalizationKey.STATS_ONLINE_TOTAL);
            computerTitle.text = localizationService.Get(LocalizationKey.STATS_COMPUTER_TITLE);
            legendGold.text = localizationService.Get(LocalizationKey.STATS_LEGEND_GOLD);
            legendSilver.text = localizationService.Get(LocalizationKey.STATS_LEGEND_SILVER);
            tagLabel.text = localizationService.Get(LocalizationKey.STATS_TAG);

            takePhotoText.text = localizationService.Get(LocalizationKey.STATS_TAKE_PHOTO);
            choosePhotoText.text = localizationService.Get(LocalizationKey.STATS_CHOOSE_PHOTO);
            phototitleTxt.text = localizationService.Get(LocalizationKey.STATS_PHOTO_TITLE);

            openSettingsDlgTitleText.text = localizationService.Get(LocalizationKey.STATS_OPEN_SETTINGS_TITLE);
            openSettingsDlgSubheadingText.text = localizationService.Get(LocalizationKey.STATS_OPEN_SETTINGS_SUBTITLE);
            openSettingsBtnText.text = localizationService.Get(LocalizationKey.STATS_OPEN_SETTINGS);

            playerProfileNameInputField.transform.gameObject.SetActive(false);

            nameEditBtn.onClick.AddListener(nameEditBtnClicked);
            playerProfileNameInputField.onEndEdit.AddListener(OnEditNameSubmit);

            nameConfirmDlgYesBtn.onClick.AddListener(nameConfirmDlgYesBtnClicked);
            nameConfirmDlgNoBtn.onClick.AddListener(nameConfirmDlgNoBtnClicked);
            nameConfirmDlg.SetActive(false);

            //closePhotoBtn.onClick.AddListener(CloseProfilePicDialog);
            closeSettingsDlgBtn.onClick.AddListener(closeSettingsDlgBtnClicked);

            supportButton.onClick.AddListener(OnSupportButtonClicked);
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);

            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].sprite = noStar;
            }

            copyTagBtn.onClick.AddListener(OnCopyTagClicked);

            restorePurchaseText.text = localizationService.Get(LocalizationKey.SUBSCRIPTION_DLG_RESTORE_PURCHASE);
            termsOfUseText.text = localizationService.Get(LocalizationKey.SUBSCRIPTION_DLG_TERMS_AND_SERVICES);
            privacyPolicyText.text = localizationService.Get(LocalizationKey.SUBSCRIPTION_DLG_PRIVACY_POLICY);

            restorePurchaseBtn.onClick.AddListener(OnRestorePurchaseButtonClicked);
            termsOfUseBtn.onClick.AddListener(OnTermsOfUseButtonClicked);
            privacyPolicyBtn.onClick.AddListener(OnPrivacyPolicyButtonClicked);

            appVersion.text = "v " + Application.version;
        }

        public void Show()
        {
            showBottomNavSignal.Dispatch(false);
            gameObject.SetActive(true);
            playerProfileNameInputField.transform.gameObject.SetActive(false);
            nameConfirmDlg.SetActive(false);
            nameEditBtn.gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void closeSettingsDlgBtnClicked()
        {
            openSettingsDlg.SetActive(false);
        }

        private void OnSupportButtonClicked()
        {
            audioService.PlayStandardClick();
            supportButtonClicked.Dispatch();
        }

        private void OnSettingsButtonClicked()
        {
            audioService.PlayStandardClick();
            settingsButtonClickedSignal.Dispatch();
        }

        public void UpdateView(StatsVO vo)
        {
            List<int> pset = vo.stats[0].performance;

            for (int i = 0; i < pset.Count; i++)
            {
                if (pset[i] == 1)
                {
                    stars[i].sprite = silverStar;
                }
                else if (pset[i] == 2)
                {
                    stars[i].sprite = goldStar;
                }
                else
                {
                    stars[i].sprite = noStar;
                }
            }

            /// Update online stats
            onlineWinPctVal.text = vo.onlineWinPct + " %";
            onlineWonVal.text = vo.onlineWon.ToString();
            onlineLostVal.text = vo.onlineLost.ToString();
            onlineDrawnVal.text = vo.onlineDrawn.ToString();
            onlineTotalVal.text = vo.onlineTotal.ToString();
            playerTag.text = vo.tag;
            country.text = Flags.GetCountry(vo.country);
            playingSince.text = string.Format("Playing since, {0}", vo.playingSince);

            currentTrophies.text = playerModel.trophies2.ToString();

            LeagueTierIconsContainer.LeagueAsset leagueAssets = tournamentsModel.GetLeagueSprites(playerModel.league.ToString());
            if (leagueAssets != null)
            {
                playerTitleImg.sprite = leagueAssets.nameImg;
                playerTitleImg.SetNativeSize();
            }
        }

        private void nameEditBtnClicked()
        {
            playerProfileNameInputField.text = playerModel.name;
            playerProfileNameInputField.transform.gameObject.SetActive(true);
            playerProfileNameInputField.ActivateInputField();
            TouchScreenKeyboard.Open(playerModel.name, TouchScreenKeyboardType.Default, false, false, false);
        }

        void OnEditNameSubmit(string text)
        {
            if ((playerProfileNameInputField.text.Length == 0) ||
                String.IsNullOrWhiteSpace(playerProfileNameInputField.text) ||
                Equals(playerProfileNameInputField.text, playerModel.name))
            {
                playerProfileNameInputField.transform.gameObject.SetActive(false);
                return;
            }

            playerProfileNameInputField.transform.gameObject.SetActive(false);

            OpenConfirmDialog();

        }

        void OpenConfirmDialog()
        {
            var newName = playerProfileNameInputField.text;
            nameConfirmDlgTitleText.text = "Change name to " + newName + "?";
            nameConfirmDlgSubheadingText.text = "Other players can view your name. Please use a friendly name and have fun.";
            nameConfirmDlgYesBtnText.text = "Yes";

            nameConfirmDlg.SetActive(true);
        }


        public void OpenProfilePicDialog()
        {
            picUpdateDlg.SetActive(true);
        }

        public void CloseProfilePicDialog()
        {
            picUpdateDlg.SetActive(false);
        }

        public void OpenSettingsDialog()
        {
            openSettingsDlg.SetActive(true);
        }

        public void CloseSettingsDialog()
        {
            openSettingsDlg.SetActive(false);
        }

        public bool IsVisible()
        {
            return gameObject.activeSelf;
        }

        void nameConfirmDlgYesBtnClicked()
        {
            var newName = playerProfileNameInputField.text;
            changeUserDetailsSignal.Dispatch(newName);
            nameConfirmDlg.SetActive(false);
        }

        void nameConfirmDlgNoBtnClicked()
        {
            nameConfirmDlg.SetActive(false);
        }

        void OnCopyTagClicked()
        {
            var textEditor = new TextEditor
            {
                text = playerTag.text
            };
            textEditor.SelectAll();
            textEditor.Copy();

            if (!copiedToClipboardBg.gameObject.activeSelf)
            {
                copiedToClipboardBg.gameObject.SetActive(true);
                ResetCopiedToClipBoardText();
                Invoke("FadeCopiedToClipBoardText", 3.3f);
            }
        }

        void FadeCopiedToClipBoardText()
        {
            copiedToClipboardBg.CrossFadeAlpha(0.0f, 1.0f, true);
            copiedToClipboardText.CrossFadeAlpha(0.0f, 1.0f, true);
            Invoke("DisableCopiedToClipBoardText", 1.0f);
        }

        void DisableCopiedToClipBoardText()
        {
            copiedToClipboardBg.gameObject.SetActive(false);
        }

        void ResetCopiedToClipBoardText()
        {
            copiedToClipboardBg.CrossFadeAlpha(1.0f, 0.3f, true);
            copiedToClipboardText.CrossFadeAlpha(1.0f, 0.3f, true);
        }

        public void ShowProcessing(bool show, bool showProcessingUi)
        {
            processingUi.SetActive(showProcessingUi);
            uiBlocker.SetActive(show);
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
    }
}
