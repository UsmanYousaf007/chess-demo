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
        [SerializeField] private Text championshipTypeText;

        private WaitForSecondsRealtime waitForOneRealSecond;
        private long endTimeUTCSeconds;
        private JoinedTournamentData _joinedTournament;

        private string challengeId;
        private bool playerWins;
        private float duration;
        private bool dataPopulated = false;

        public IServerClock serverClock;
        public Signal<string, bool> continueButtonClickedSignal = new Signal<string, bool>();
        public Signal<Action, bool> schedulerSubscription = new Signal<Action, bool>();

        [Inject] public ITournamentsModel tournamentsModel { get; set; }

        public override void Init()
        {
            scrollRectChampionship = scrollView;
            championshipBarsPool = new GameObjectsPool(championshipLeaderboardPlayerBarPrefab, 50);

            continueButton.onClick.AddListener(OnContinueButtonClicked);

            waitForOneRealSecond = new WaitForSecondsRealtime(1f);

            UIDlgManager.Setup(gameObject);
            target = 1.0f;
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

            if(!dataPopulated)
            {
                ShowProcessing();
            }

            continueButton.GetComponent<CanvasGroup>().alpha = 0;
            SchedulerCallback();
            UIDlgManager.Show(gameObject, Colors.BLUR_BG_BRIGHTNESS_NORMAL).Then(() => schedulerSubscription.Dispatch(SchedulerCallback, true));
            Invoke("AnimateContinueButton", 0.75f);
        }

        private void OnBgBlurComplete()
        {
            UIDlgManager.Show(gameObject, Colors.BLUR_BG_BRIGHTNESS_NORMAL, true).Then(() => schedulerSubscription.Dispatch(SchedulerCallback, true));
            Invoke("AnimateContinueButton", 0.75f);
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
            dataPopulated = false;
            schedulerSubscription.Dispatch(SchedulerCallback, false);
        }

        public override void UpdateView(string playerId, JoinedTournamentData joinedTournament)
        {
            _joinedTournament = joinedTournament;
            championshipTypeText.text = _joinedTournament.DisplayName.ToUpper();
            base.UpdateView(playerId, joinedTournament);
            dataPopulated = true;
            base.UpdateLeagueTitle(playerModel, tournamentsModel);
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

        //IEnumerator CountdownTimer()
        //{
        //    while (gameObject.activeInHierarchy)
        //    {
        //        UpdateTime();
        //        yield return waitForOneRealSecond;
        //    }

        //    yield return null;
        //}

        public void SchedulerCallback()
        {
            long timeLeft = endTimeUTCSeconds - serverClock.currentTimestamp/1000;
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

        public void ShowProcessing()
        {
            pleaseWaitPanel.SetActive(true);
        }
    }
}