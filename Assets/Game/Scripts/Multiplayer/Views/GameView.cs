/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;

using strange.extensions.mediation.impl;

using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using UnityEngine.UI;
using TurboLabz.Chess;
using TurboLabz.InstantGame;
using System.Collections;
using System;

namespace TurboLabz.Multiplayer
{
    [CLSCompliant(false)]
    public partial class GameView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }

        [Inject] public ShowAdSignal showAdSignal { get; set; }
        [Inject] public ShowRewardedAdSignal showRewardedAdSignal { get; set; }
        [Inject] public ShowBottomNavSignal showBottomNavSignal { get; set; }

        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IAdsSettingsModel adsSettingsModel { get; set; }


        [Header("Main View")]
        public Camera chessboardCamera;
        public GameObject uiBlocker;
        public GameObject chessboardBlocker;
        public GameObject playerInfoPanel;
        public GameObject opponentInfoPanel;
        public Text opponentConnectionMonitorLabel;
        public GameObject logoObject;

		[Header("Match Status")]
		public GameObject friendlyObject;
		public GameObject rankedObject;
		public Text matchTypeText;
		public GameObject matchTypeObject;
        public Image powerModeImage;

		[HideInInspector] public bool isLongPlay;
        [HideInInspector] public bool isRankedGame;
        //[HideInInspector] public bool isTenMinGame;
        //[HideInInspector] public bool isOneMinGame;
        //[HideInInspector] public bool isThirtyMinGame;
        [HideInInspector] GameTimeMode gameTimeMode;

        private bool menuButtonWasActive;
        Coroutine opponentConnectionMonitorCR;
        Coroutine opponentAutoResignCR;

        
        [Inject] public ForceUpdateFriendOnlineStatusSignal forceUpdateFriendOnlineStatusSignal { get; set; }

        public void Show()
        {
            showBottomNavSignal.Dispatch(false);
            gameObject.SetActive(true);
            OnParentShowResults();
            OnParentShowPromotions();
            OnParentShowCapturedPieces();
            OnParentShowScore();
            OnParentShowClock();
            OnParentShowMenu();
            OnParentShowWifi();
            OnParentShowAccept();
            OnParentShowBotBar();
            OnParentShowSafeMove();
            OnParentShowHint();
            OnParentShowHindsight();
            OnParentShowInfo();
            OnParentShowAdBanner();
            OnParentShowSpecialHint();
            EnableSafeButton();
            ShowViewBoardResultsPanel(false);
            OnShowLogo();
        }

        public void Hide()
        { 
            gameObject.SetActive(false);
            uiBlocker.SetActive(false);
            chessboardBlocker.SetActive(false);
            OnParentHideChessboard();
            OnParentHideClickAndDrag();
            opponentId = null;
            HideChallengeSent();
            OnParentHideAdBanner();
            HideOpponentConnectionMonitor();
            FlashClocks(false);
            FindMatchTimeoutEnable(false);
            appInfoModel.gameMode = GameMode.NONE;
            HideSynchMovesDlg();
        }

		public void SetMatchType()
		{
			matchTypeObject.SetActive(true);

            if (isRankedGame)
            {
                rankedObject.SetActive(true);
                friendlyObject.SetActive(false);
                matchTypeText.text = localizationService.Get(LocalizationKey.LONG_PLAY_RANKED);
            }
            else
            {
                friendlyObject.SetActive(true);
                rankedObject.SetActive(false);
                matchTypeText.text = localizationService.Get(LocalizationKey.LONG_PLAY_FRIENDLY);
            }
		}

        public bool IsVisible()
        {
            return gameObject.activeSelf;
        }

        private void OnApplicationFocus(bool focus)
        {
            OnApplicationFocusClickAndDrag(focus);
        }

        public void EnableModalBlocker(float curtainAlpha = Colors.UI_BLOCKER_DARK_ALPHA)
        {
            uiBlocker.SetActive(true);
            chessboardBlocker.SetActive(true);

            Image uiBlockerImage = uiBlocker.GetComponent<Image>();
            Color c = uiBlockerImage.color;
            c.a = curtainAlpha;
            uiBlockerImage.color = c;
        }

        public void DisableModalBlocker()
        {
            uiBlocker.SetActive(false);
            chessboardBlocker.SetActive(false);
        }

        private void DisableLabel(Text label)
        {
            SetLabelAlpha(label, Colors.DISABLED_TEXT_ALPHA);
        }

        private void EnableLabel(Text label)
        {
            SetLabelAlpha(label, Colors.ENABLED_TEXT_ALPHA);
        }

        private void SetLabelAlpha(Text label, float alpha)
        {
            Color oldColor = label.color;
            label.color = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
        }

        private IEnumerator OpponentConnectionMonitorCR()
        {
            yield return new WaitForSecondsRealtime(6);
            opponentConnectionMonitorLabel.gameObject.SetActive(false);
            matchTypeObject.SetActive(true);
        }

        IEnumerator AutoResignCountdown(int startTimer)
        {
            int countdownTimer = startTimer;
            while (countdownTimer >= 0) {
                opponentConnectionMonitorLabel.text = "Auto resign in " + countdownTimer + " secs";
                yield return new WaitForSeconds(1);
                countdownTimer--;
            }
            yield return null;
        }

        public void EnableOpponentConnectionMonitor(bool isEnable, int timer)
        {
            //matchTypeObject.SetActive(false);
            if (opponentConnectionMonitorCR != null)
            {
                routineRunner.StopCoroutine(opponentConnectionMonitorCR);
                opponentConnectionMonitorCR = null;
            }

            if (opponentAutoResignCR != null)
            {
                StopCoroutine(opponentAutoResignCR);
                opponentAutoResignCR = null;
            }

            if (isEnable)
            {
                if (timer <= opponentTimer.TotalSeconds)
                {
                    opponentConnectionMonitorLabel.gameObject.SetActive(true);
                    opponentAutoResignCR = StartCoroutine(AutoResignCountdown(timer));

                    PublicProfile publicProfile = matchInfoModel.activeMatch.opponentPublicProfile;
                    ProfileVO pvo = new ProfileVO();
                    pvo.playerPic = publicProfile.profilePicture;
                    pvo.playerName = publicProfile.name;
                    pvo.eloScore = publicProfile.eloScore;
                    pvo.countryId = publicProfile.countryId;
                    pvo.playerId = publicProfile.playerId;
                    pvo.avatarColorId = publicProfile.avatarBgColorId;
                    pvo.avatarId = publicProfile.avatarId;
                    pvo.isOnline = false;
                    pvo.isActive = publicProfile.isActive;
                    pvo.activity = null;
                    pvo.isPremium = publicProfile.isSubscriber;
                    forceUpdateFriendOnlineStatusSignal.Dispatch(pvo);
                }
            }
            else
            {
                opponentConnectionMonitorCR = routineRunner.StartCoroutine(OpponentConnectionMonitorCR());
                PublicProfile publicProfile = matchInfoModel.activeMatch.opponentPublicProfile;
                ProfileVO pvo = new ProfileVO();
                pvo.playerPic = publicProfile.profilePicture;
                pvo.playerName = publicProfile.name;
                pvo.eloScore = publicProfile.eloScore;
                pvo.countryId = publicProfile.countryId;
                pvo.playerId = publicProfile.playerId;
                pvo.avatarColorId = publicProfile.avatarBgColorId;
                pvo.avatarId = publicProfile.avatarId;
                pvo.isOnline = publicProfile.isOnline;
                pvo.isActive = publicProfile.isActive;
                pvo.activity = null;
                pvo.isPremium = publicProfile.isSubscriber;
                forceUpdateFriendOnlineStatusSignal.Dispatch(pvo);
            }
        }

        private void HideOpponentConnectionMonitor()
        {
            if (opponentConnectionMonitorCR != null)
            {
                routineRunner.StopCoroutine(opponentConnectionMonitorCR);
                opponentConnectionMonitorCR = null;
            }

            if (opponentAutoResignCR != null)
            {
                StopCoroutine(opponentAutoResignCR);
                opponentAutoResignCR = null;
            }

            opponentConnectionMonitorLabel.gameObject.SetActive(false);
        }

        private void OnShowLogo()
        {
            bool logoActive = false;

            if (playerModel.HasRemoveAds() || adsSettingsModel.isBannerEnabled == false)
                logoActive = true;

            logoObject.SetActive(logoActive);

        }
    }
}
