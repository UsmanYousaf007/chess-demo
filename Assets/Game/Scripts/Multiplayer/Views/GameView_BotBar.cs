/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine.UI;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using UnityEngine;
using strange.extensions.signal.impl;
using System.Collections;

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        public Signal ShowShareDialogSignal = new Signal();

        [Header("Bot Bar")]
        public Text backToFriendsLabel;
        public Button backToFriendsButton;

        public Button exitButton;

        public Button shareScreenButton;
        public Texture2D logo;
        private bool showAdOnBack;

        public GameObject chatInputContainer;

        public Signal analysisExitButtonClickedSignal = new Signal();

        public void InitBotBar()
        {
            backToFriendsLabel.text = localizationService.Get(LocalizationKey.IN_GAME_BACK);
            backToFriendsButton.onClick.AddListener(OnBackToFriendsClicked);
            exitButton.onClick.AddListener(OnExitButtonClicked);
        }

        void OnParentShowBotBar()
        {
            backToFriendsButton.gameObject.SetActive(false);
            backToFriendsLabel.gameObject.SetActive(false);
            showAdOnBack = false;
            exitButton.interactable = true;
        }

        void UpdateBotBar()
        {
            backToFriendsButton.gameObject.SetActive(isLongPlay);
            backToFriendsLabel.gameObject.SetActive(isLongPlay);
        }

        void SetGameAnalysisBottomBar(bool val)
        {
            chatInputContainer.SetActive(!val);
            exitButton.gameObject.SetActive(val);
            menuButton.gameObject.SetActive(!val);
        }

        void OnBackToFriendsClicked()
        {
            if (appInfoModel.isReconnecting != DisconnectStates.FALSE)
            {
                return;
            }

            if (matchInfoModel.activeChallengeId == null)
            {
                resultsDialogOpenedSignal.Dispatch();
            }
            else if (matchInfoModel.activeMatch == null || matchInfoModel.activeMatch.endGameResult != EndGameResult.NONE)
            {
                resultsDialogOpenedSignal.Dispatch();
            }
            else
            {
                OnBackToLobby();
            }
        }

        void OnBackToLobby()
        {
            if (showAdOnBack)
            {
                showAdOnBack = false;
            }
            
            backToLobbySignal.Dispatch(); 
        }

        public void OpenShareDialog()
        {
            StartCoroutine(DispatchShareSignal());
        }

        public IEnumerator DispatchShareSignal()
        {
            yield return new WaitForSeconds(.25f);
            ShowShareDialogSignal.Dispatch();
        }

        void OnExitButtonClicked()
        {
            //resultsDialogOpenedSignal.Dispatch();
            audioService.PlayStandardClick();
            analysisExitButtonClickedSignal.Dispatch();
            exitButton.interactable = false;
            //ShowWeeklyChampionshipResults();
        }
    }
}
