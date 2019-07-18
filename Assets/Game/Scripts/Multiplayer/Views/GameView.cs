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

namespace TurboLabz.Multiplayer
{
    public partial class GameView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        [Inject] public ShowAdSignal showAdSignal { get; set; }

        [Header("Main View")]
        public Camera chessboardCamera;
        public GameObject uiBlocker;
        public GameObject chessboardBlocker;
        public GameObject playerInfoPanel;
        public GameObject opponentInfoPanel;
        public Text opponentConnectionMonitorLabel;

		[Header("Match Status")]
		public GameObject friendlyObject;
		public GameObject rankedObject;
		public Text matchTypeText;
		public GameObject matchTypeObject;


		[HideInInspector] public bool isLongPlay;
        [HideInInspector] public bool isRankedGame;

		

		private bool menuButtonWasActive;
        Coroutine opponentConnectionMonitorCR;

        public void Show()
        {
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
            OnParentShowChat();
            OnParentShowSafeMove();
            OnParentShowHint();
            OnParentShowHindsight();
            OnParentShowInfo();
            OnParentShowAdBanner();
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
        }

		public void SetMatchType()
		{
			if (isLongPlay)
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
		}

        public bool IsVisible()
        {
            return gameObject.activeSelf;
        }

        private void OnApplicationFocus(bool focus)
        {
            OnApplicationFocusClickAndDrag(focus);
        }

        private void EnableModalBlocker(float curtainAlpha = Colors.UI_BLOCKER_LIGHT_ALPHA)
        {
            uiBlocker.SetActive(true);
            chessboardBlocker.SetActive(true);

            Image uiBlockerImage = uiBlocker.GetComponent<Image>();
            Color c = uiBlockerImage.color;
            c.a = curtainAlpha;
            uiBlockerImage.color = c;
        }

        private void DisableModalBlocker()
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
        }

        public void EnableOpponentConnectionMonitor(bool isEnable)
        {
            if (opponentConnectionMonitorCR != null)
            {
                routineRunner.StopCoroutine(opponentConnectionMonitorCR);
                opponentConnectionMonitorCR = null;
            }

            if (isEnable)
            {
                opponentConnectionMonitorLabel.gameObject.SetActive(true);
            }
            else
            {
                opponentConnectionMonitorCR = routineRunner.StartCoroutine(OpponentConnectionMonitorCR());
            }
        }

        private void HideOpponentConnectionMonitor()
        {
            if (opponentConnectionMonitorCR != null)
            {
                routineRunner.StopCoroutine(opponentConnectionMonitorCR);
                opponentConnectionMonitorCR = null;
            }
            opponentConnectionMonitorLabel.gameObject.SetActive(false);
        }
    }
}
