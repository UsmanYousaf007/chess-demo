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

namespace TurboLabz.Multiplayer
{
    public partial class GameView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IAdsService adsService { get; set; }

        [Header("Main View")]
        public Camera chessboardCamera;
        public GameObject uiBlocker;
        public GameObject chessboardBlocker;
        public GameObject playerInfoPanel;
        public GameObject opponentInfoPanel;

        [HideInInspector]
        public bool isLongPlay;

        private bool menuButtonWasActive;
        private bool showAd;

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
        }

        public void Hide()
        { 
            gameObject.SetActive(false);
            uiBlocker.SetActive(false);
            chessboardBlocker.SetActive(false);
            OnParentHideChessboard();
            OnParentHideClickAndDrag();
        }

        public bool IsVisible()
        {
            return gameObject.activeSelf;
        }

        private void OnApplicationFocus(bool focus)
        {
            OnApplicationFocusClickAndDrag(focus);
        }

        private void EnableModalBlocker(bool showAlphaCurtain = true)
        {
            uiBlocker.SetActive(true);
            chessboardBlocker.SetActive(true);

            Image uiBlockerImage = uiBlocker.GetComponent<Image>();
            Color c = uiBlockerImage.color;
            c.a = showAlphaCurtain ? 50f/255f : 1f/255f; // Todo: These should be constants somewhere
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
    }
}
