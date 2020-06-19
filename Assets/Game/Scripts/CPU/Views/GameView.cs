/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-06 17:45:03 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;

using strange.extensions.mediation.impl;

using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using UnityEngine.UI;
using TurboLabz.Chess;
using TurboLabz.InstantGame;

namespace TurboLabz.CPU
{
    public partial class GameView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }

        [Inject] public ShowAdSignal showAdSignal { get; set; }
        [Inject] public ShowRewardedAdSignal showRewardedAdSignal { get; set; }

        [Inject] public IPlayerModel playerModel { get; set; }

        [Header("Main View")]
        public Camera chessboardCamera;
        public GameObject uiBlocker;
        public GameObject chessboardBlocker;
        public GameObject playerInfoPanel;
        public GameObject opponentInfoPanel;

        private bool menuButtonWasActive;

        public void Show()
        {
            gameObject.SetActive(true);
            OnParentShowResults();
            OnParentShowPromotions();
            OnParentShowCapturedPieces();
            OnParentShowScore();
            OnParentShowClock();
            //OnParentShowNotation();
            OnParentShowMatchInfo();
            OnParentShowMenu();
            OnParentShowHint();
            OnParentShowInfo();
            OnParentShowSafeMove();
            OnParentShowHindsight();
            OnParentShowStep();
            OnParentShowAdBanner();
            EnableSafeButton();

            showAdOnBack = false;
        }

        public void Hide()
        { 
            gameObject.SetActive(false);
            uiBlocker.SetActive(false);
            chessboardBlocker.SetActive(false);
            OnParentHideChessboard();
            OnParentHideClickAndDrag();
            OnParentHideAdBanner();
            appInfoModel.gameMode = GameMode.NONE;

            if (hideHintCR != null)
                StopCoroutine(hideHintCR);
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

        private void StashMenuButton()
        {
            if (IsMenuButtonActive())
            {
                menuButtonWasActive = true;
            }

            DisableMenuButton();
        }

        private void PopMenuButton()
        {
            if (menuButtonWasActive)
            {
                EnableMenuButton();
            }
        }
    }
}
