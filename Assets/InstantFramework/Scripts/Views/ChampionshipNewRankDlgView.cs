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

        public override void Init()
        {
            scrollRectChampionship = scrollView;
            championshipBarsPool = new GameObjectsPool(championshipLeaderboardPlayerBarPrefab, 50);

            continueButton.onClick.AddListener(() => continueBtnClickedSignal.Dispatch());

            waitForOneRealSecond = new WaitForSecondsRealtime(1f);
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

            congratulationsGraphic.SetActive(newRank);
            newRankTxtGraphic.SetActive(newRank);

            rankGraphic.SetActive(!newRank);
            yourRankTxtGraphic.SetActive(!newRank);

            base.Show();
            gameObject.SetActive(true);

            StartCoroutine(CountdownTimer());
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
                timerText.text = timeLeftText;
            }
            else
            {
                timerText.text = "0:00";
            }
        }
    }
}