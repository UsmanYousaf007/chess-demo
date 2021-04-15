/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using UnityEngine.UI;

using TMPro;
using System;
using DG.Tweening;
using TurboLabz.TLUtils;
using System.Collections;
using TurboLabz.InstantGame;
using System.Collections.Generic;
using strange.extensions.signal.impl;
using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    [CLSCompliant(false)]
    public class ChampionshipNewRankDlgView : ChampionshipResultDlgView
    {
        // Set in Inspector
        [SerializeField] private GameObject congratulationsGraphic;
        [SerializeField] private GameObject rankGraphic;
        [SerializeField] private GameObject newRankTxtGraphic;
        [SerializeField] private GameObject yourRankTxtGraphic;
        [SerializeField] private TextMeshProUGUI timerText;

        private WaitForSecondsRealtime waitForOneRealSecond;
        private long endTimeUTCSeconds;
        private JoinedTournamentData _joinedTournament;

        private string challengeId;
        private bool playerWins;
        private float duration;

        //[Inject] public ShowAdSignal showAdSignal { get; set; }
        public Signal<string, bool> continueButtonClickedSignal = new Signal<string, bool>();

        [Inject] public IMetaDataModel metaDataModel { get; set; }

        public override void Init()
        {
            scrollRectChampionship = scrollView;
            championshipBarsPool = new GameObjectsPool(championshipLeaderboardPlayerBarPrefab, 50);

            continueButton.onClick.AddListener(OnContinueButtonClicked);

            waitForOneRealSecond = new WaitForSecondsRealtime(1f);

            UIDlgManager.Setup(gameObject);
        }

        public void Show(JoinedTournamentData joinedTournament, bool newRank = false)
        {
            _joinedTournament = joinedTournament;
            Show(newRank);
        }

        public void Show(bool newRank = false)
        {
            if (_joinedTournament != null)
            {
                endTimeUTCSeconds = _joinedTournament.endTimeUTCSeconds;
            }

            continueButton.GetComponent<CanvasGroup>().alpha = 0;
            UIDlgManager.Show(gameObject, Colors.BLUR_BG_BRIGHTNESS_NORMAL, true).Then(()=> StartCoroutine(CountdownTimer()));
            Invoke("AnimateContinueButton", 1f);
            metaDataModel.ShowChampionshipNewRankDialog = false;
        }

        private void AnimateContinueButton()
        {
            continueButton.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
        }

        private void UpdateScrollViewChampionship(float value)
        {
            GetScrollView().verticalNormalizedPosition = value;
        }

        public override void Hide()
        {
            base.Hide();
            gameObject.SetActive(false);
            StopCoroutine(CountdownTimer());
        }

        public override void UpdateView(string playerId, JoinedTournamentData joinedTournament)
        {
            _joinedTournament = joinedTournament;
            base.UpdateView(playerId, joinedTournament);
        }

        public void UpdateView(string _challengeId, bool _playerWins, float _duration)
        {
            challengeId = _challengeId;
            playerWins = _playerWins;
            duration = _duration;
        }

        public override void OnContinueButtonClicked()
        {
            continueButtonClickedSignal.Dispatch(challengeId, playerWins);
        }

        IEnumerator CountdownTimer()
        {
            while (gameObject.activeInHierarchy)
            {
                UpdateTime();
                yield return waitForOneRealSecond;
            }

            yield return null;
        }

        public void UpdateTime()
        {
            long timeLeft = endTimeUTCSeconds - DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (timeLeft > 0)
            {
                timeLeft--;
                var timeLeftText = TimeUtil.FormatTournamentClock(TimeSpan.FromMilliseconds(timeLeft * 1000));
                timerText.text = "Ends In " + timeLeftText;
            }
            else
            {
                timerText.text = "Ends In 0:00";
            }
        }

        public void ScaleInDialogue(float duration)
        {
            gameObject.transform.DOScale(Vector3.one, duration).SetEase(Ease.OutBack);
        }
    }
}