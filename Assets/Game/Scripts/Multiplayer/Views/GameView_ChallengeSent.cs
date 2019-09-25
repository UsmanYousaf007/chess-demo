/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using TurboLabz.InstantGame;
using DG.Tweening;
using System.Collections;

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        [Inject] public IRoutineRunner routineRunner { get; set; }

        [Header("Challenge Sent Dlg")]
        public Button challengeSentBackButton;
        public GameObject challengeSentDialog;

        Coroutine showChallengeSentCR;

        public Signal challengeSentBackToLobbyButtonSignal = new Signal();

        public void InitChallengeSent()
        {
            challengeSentBackButton.onClick.AddListener(OnChallengeSentBackButtonClicked);
            challengeSentDialog.SetActive(false);
        }

        public void ShowChallengeSent()
        {
            EnableModalBlocker(Colors.UI_BLOCKER_LIGHT_ALPHA);
            showChallengeSentCR = routineRunner.StartCoroutine(ShowChallengeSentCR());
        }

        public void ShowChallengeSentDlg()
        {
            EnableModalBlocker(Colors.UI_BLOCKER_DARK_ALPHA);
            challengeSentDialog.SetActive(true);
            DisableMenuButton();
        }

        public void HideChallengeSent()
        {
            if (showChallengeSentCR != null)
            {
                routineRunner.StopCoroutine(showChallengeSentCR);
                showChallengeSentCR = null;
            }
            DisableModalBlocker();
            challengeSentDialog.SetActive(false);
            EnableMenuButton();
        }

        void OnChallengeSentBackButtonClicked()
        {
            challengeSentBackToLobbyButtonSignal.Dispatch();
        }

        private IEnumerator ShowChallengeSentCR()
        {
            yield return new WaitForSecondsRealtime(1);
            ShowChallengeSentDlg();
        }
    }
}
