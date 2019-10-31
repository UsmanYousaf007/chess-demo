/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl ;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using System.Collections.Generic;
using TMPro;
using System;

namespace TurboLabz.InstantGame
{
    public class StatsView : View
    {
        // Services
        [Inject] public ILocalizationService localizationService { get; set; }

        // Scene references
        //Models
        [Inject] public IPlayerModel playerModel { get; set; }

        [Inject] public ChangeUserDetailsSignal changeUserDetailsSignal { get; set; }

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

        public Button privacyPolicy;
        public Button restorePurchases;
        public Text appVersion;

        public Button backButton;

        public TMP_InputField playerProfileNameInputField;
        public Button nameEditBtn;

        public Signal restorePurchasesSignal = new Signal();

        [Header("Name Confirm Dialog")]
        public GameObject nameConfirmDlg;
        public Button nameConfirmDlgYesBtn;
        public Button nameConfirmDlgNoBtn;
        public Text nameConfirmDlgYesBtnText;
        public Text nameConfirmDlgTitleText;
        public Text nameConfirmDlgSubheadingText;

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

            playerProfileNameInputField.transform.gameObject.SetActive(false);

            nameEditBtn.onClick.AddListener(nameEditBtnClicked);
            playerProfileNameInputField.onEndEdit.AddListener(OnEditNameSubmit);

            nameConfirmDlgYesBtn.onClick.AddListener(nameConfirmDlgYesBtnClicked);
            nameConfirmDlgNoBtn.onClick.AddListener(nameConfirmDlgNoBtnClicked);
            nameConfirmDlg.SetActive(false);


            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].sprite = noStar;
            }

            privacyPolicy.onClick.AddListener(OnPrivacyPolicyClicked);

            #if UNITY_IOS
            restorePurchases.gameObject.SetActive(true);
            restorePurchases.onClick.AddListener(OnRestorePurchasesClicked);
            #else
            restorePurchases.gameObject.SetActive(false);
            #endif
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

            appVersion.text = "v" + Application.version;
        }

        public void Show() 
        { 
            gameObject.SetActive(true);
            playerProfileNameInputField.transform.gameObject.SetActive(false);
            nameConfirmDlg.SetActive(false);

            nameEditBtn.gameObject.SetActive(false);

            if (string.IsNullOrEmpty(playerModel.editedName))
            {
                nameEditBtn.gameObject.SetActive(true);
            }

            //nameEditBtn.gameObject.SetActive(true);
        }

        public void Hide()
        { 
            gameObject.SetActive(false);
        }

        private void OnPrivacyPolicyClicked()
        {
            Application.OpenURL("https://turbolabz.com/privacy-policy/");
        }

        private void OnRestorePurchasesClicked()
        {
            restorePurchasesSignal.Dispatch();
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
            nameConfirmDlgTitleText.text = "Change name to "+ newName + "?";
            nameConfirmDlgSubheadingText.text = "You can change your name once. Are you sure to change your name?";
            nameConfirmDlgYesBtnText.text = "YES";

            nameConfirmDlg.SetActive(true);
        }

        void nameConfirmDlgYesBtnClicked()
        {
            var newName = playerProfileNameInputField.text;
            changeUserDetailsSignal.Dispatch(newName);
            nameConfirmDlg.SetActive(false);

            nameEditBtn.gameObject.SetActive(false);
        }

        void nameConfirmDlgNoBtnClicked()
        {
            nameConfirmDlg.SetActive(false); 
        }
    }
}
